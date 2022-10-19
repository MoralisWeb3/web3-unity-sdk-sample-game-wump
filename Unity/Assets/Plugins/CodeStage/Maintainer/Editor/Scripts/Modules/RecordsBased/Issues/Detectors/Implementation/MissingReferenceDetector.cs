#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core;
	using Core.Scan;
	using Routines;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;
	using Object = UnityEngine.Object;
	
#if !UNITY_2019_2_OR_NEWER
	[InitializeOnLoad]
#endif
	// ReSharper disable once ClassNeverInstantiated.Global since it's used from TypeCache
	internal class MissingReferenceDetector : IssueDetector, 
		ISceneBeginIssueDetector,
		IAssetBeginIssueDetector, 
		ISettingsAssetBeginIssueDetector,
		IPropertyIssueDetector,
		IUnityEventIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.Global,
				DetectorKind.Defect,
				IssueSeverity.Warning,
				"Missing reference", 
				"Search for any missing references in Components, Project Settings, Scriptable Objects, and so on.");
		}}
		
		public Type[] AssetTypes { get { return null; } } // we check all types including nulls!
		public AssetSettingsKind SettingsKind { get { return AssetSettingsKind.Undefined; } }  // to check all settings

		private UnityEventIssuesScanner unityEventScanner;
		
#if !UNITY_2019_2_OR_NEWER
		static MissingReferenceDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new MissingReferenceDetector());
		}
#endif
		
		public void AssetBegin(DetectorResults results, AssetLocation location)
		{
			if (location.Asset.Kind == AssetKind.Settings)
			{
				var issues = SettingsChecker.CheckSettingsAssetForMissingReferences(this, location);
				results.Add(issues);
			}
			else
			{
				ProcessNonSettingsAsset(results, location);
			}
		}

		public void SceneBegin(DetectorResults results, AssetLocation location)
		{
			var issues = SettingsChecker.CheckSceneSettingsForMissingReferences(this, location);
			results.Add(issues);
		}
		
		public PropertyScanDepth GetPropertyScanDepth(ComponentLocation location)
		{
			return PropertyScanDepth.VisibleOnly;
		}

		public void Property(DetectorResults results, PropertyLocation location)
		{
			var issue = TryDetectSerializedPropertyIssue(location);
			results.Add(issue);
		}

		public void UnityEventProperty(DetectorResults results, PropertyLocation location, UnityEventScanPhase phase)
		{
			switch (phase)
			{
				case UnityEventScanPhase.Begin:
				case UnityEventScanPhase.Calls:
				case UnityEventScanPhase.CallMethodName:
				case UnityEventScanPhase.ArgumentType:
				case UnityEventScanPhase.CallMode:
					break;
				case UnityEventScanPhase.CallTarget:
					if (CSSerializedPropertyTools.IsPropertyHasMissingReference(location.Property))
						results.Add(CreateIssue(location));
					break;
				case UnityEventScanPhase.InvalidListener:
					results.Add(CreateIssue(location));
					break;
				default:
					throw new ArgumentOutOfRangeException("phase", phase, null);
			}
		}

		private void ProcessNonSettingsAsset(DetectorResults results, AssetLocation location)
		{
			var assetType = location.Asset.Type;
			if (assetType == null)
			{
				// skip
			}
			else if (assetType == CSReflectionTools.materialType)
			{
				var target = AssetDatabase.LoadAssetAtPath<Material>(location.Asset.Path);
				ProcessMaterial(results, location, target);
			}
			else if (assetType == CSReflectionTools.monoScriptType)
			{
				var target = AssetDatabase.LoadAssetAtPath<MonoScript>(location.Asset.Path);
				ProcessMonoScript(results, location, target);
			}
			else // for the rest supported asset types 
			{
				var target = AssetDatabase.LoadMainAssetAtPath(location.Asset.Path);
				if (target == null)
					return; // skip
				ProcessUnityObjectAsset(results, location, target);
			}
		}

		#region Unity Object Asset (generic)
		private void ProcessUnityObjectAsset(DetectorResults results, AssetLocation location, Object unityObject)
		{
			var narrow = location.Narrow();
			narrow.ComponentOverride(unityObject.GetType().Name);

			if (unityEventScanner == null)
			{
				unityEventScanner = new UnityEventIssuesScanner();
				unityEventScanner.RegisterScanListeners(new IUnityEventScanListener<DetectorResults>[]{this});
			}

			var initialInfo = new SerializedObjectTraverseInfo(unityObject);
			CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
			{
				narrow.PropertyOverride(property);
				
				// specific Unity Event processing
				if (CSReflectionTools.IsPropertyIsSubclassOf<UnityEventBase>(property))
				{
					unityEventScanner.Property(results, narrow);
					info.skipChildren = true;
				}
				else // rest properties
				{
					var issue = TryDetectSerializedPropertyIssue(narrow);
					if (issue != null)
						results.Add(issue);
				}
			});
		}
		#endregion
		
		#region Material
		private void ProcessMaterial(DetectorResults results, AssetLocation location, Material target)
		{
			var componentTraverseInfo = new SerializedObjectTraverseInfo(target);
			CSTraverseTools.TraverseMaterialTexEnvs(componentTraverseInfo,
				(traverseInfo, texEnv, texEnvName, texEnvTexture) =>
				{
					// checking if material's shader really have this property
					// to avoid checking another shader leftovers
					if (!target.HasProperty(texEnvName))
						return;

					var narrow = location.Narrow();
					narrow.ComponentOverride("Material");
					narrow.PropertyOverride(texEnvTexture, texEnvName);
					var issue = TryDetectSerializedPropertyIssue(narrow);
					if (issue != null)
						results.Add(issue);
				});
		}
		#endregion
		
		#region Mono Script

		private void ProcessMonoScript(DetectorResults results, AssetLocation location, MonoScript target)
		{
			var componentTraverseInfo = new SerializedObjectTraverseInfo(target, false);
			CSTraverseTools.TraverseObjectProperties(componentTraverseInfo, (info, sp) =>
			{
				var propertyName = CSTraverseTools.TryGetMonoScriptDefaultPropertyName(sp);
				if (string.IsNullOrEmpty(propertyName))
					return;

				var narrow = location.Narrow();
				narrow.ComponentOverride("MonoScript");
				narrow.PropertyOverride(sp, propertyName);
				var issue = TryDetectSerializedPropertyIssue(narrow);
				if (issue != null)
					results.Add(issue);
			});
		}
		#endregion
		
		private IssueRecord TryDetectSerializedPropertyIssue(PropertyLocation location)
		{
			if (!CSSerializedPropertyTools.IsPropertyHasMissingReference(location.Property)) 
				return null;

			return CreateIssue(location);
		}
		
		private IssueRecord CreateIssue(PropertyLocation location)
		{
			IssueRecord record;

			if (location.Group != LocationGroup.Asset)
				record = GameObjectIssueRecord.ForProperty(this, Issues.IssueKind.MissingReference, location);
			else
				record = UnityObjectAssetIssueRecord.Create(this, Issues.IssueKind.MissingReference, location);

			return record;
		}
	}
}