#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Core.Extension;
	using Core.Scan;
	using Detectors;
	using Routines;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Does looks for different project issues at Issues Finder.
	/// </summary>
	internal static class IssuesFinderDetectors
	{
		internal static ExtensibleModule<IIssueDetector> detectors;
		private static List<IIssueDetector> enabledDetectors;
		/*private static ExtensibleModule<IPropertyScanner<DetectorResults>> propertyScanners;*/
		
		private static IAssetBeginIssueDetector[] assetIssueDetectors;
		private static ISettingsAssetBeginIssueDetector[] settingsAssetIssueDetectors;
		private static ISceneBeginIssueDetector[] sceneBeginIssueDetectors;
		private static IGameObjectBeginIssueDetector[] goBeginIssueDetectors;
		private static IComponentBeginIssueDetector[] componentBeginIssueDetectors;
		private static IPropertyIssueDetector[] propertyIssueDetectors;
		private static IUnityEventIssueDetector[] unityEventIssueDetectors;
		private static IComponentEndIssueDetector[] componentEndIssueDetectors;
		private static IGameObjectEndIssueDetector[] goEndIssueDetectors;
		private static ISceneEndIssueDetector[] sceneEndIssueDetectors;
		
		private static MissingPrefabDetector missingPrefabDetector;

		private static GenericLocation currentLocation;
		private static DetectorResults foundIssues;

		private static readonly List<IPropertyIssueDetector> ActivePropertyIssueDetectors =
			new List<IPropertyIssueDetector>();
		
		private static UnityEventIssuesScanner unityEventScanner;

		private static bool missingComponentDetectorEnabled;
		
#if UNITY_2019_2_OR_NEWER
		static IssuesFinderDetectors()
		{
			detectors = new ExtensibleModule<IIssueDetector>(new IssueDetectorComparer());
			//propertyScanners = new ExtensibleModule<IPropertyScanner<DetectorResults>>();

			// TODO: call sync detectors here if settings exist
		}
#else
		// gets called from internal parsers
		internal static void AddInternalDetector(IIssueDetector detector)
		{
			if (detectors == null)
				detectors = new ExtensibleModule<IIssueDetector>(new IssueDetectorComparer());

			detectors.AddInternal(detector); 
		}
		
		/*internal static void AddInternalPropertyScanner(IPropertyScanner<DetectorResults> scanner)
		{
			if (propertyScanners == null)
				propertyScanners = new ExtensibleModule<IPropertyScanner<DetectorResults>>();

			propertyScanners.AddInternal(scanner);
		}*/
#endif
		
		public static void Init(List<IssueRecord> issues)
		{
			if (foundIssues == null)
				foundIssues = new DetectorResults();
			foundIssues.Set(ref issues);
			
			InitDetectors();
			
			currentLocation = new GenericLocation();
		}

		private static void InitDetectors()
		{
			missingComponentDetectorEnabled = false;
			
			enabledDetectors = new List<IIssueDetector>(detectors.extensions.Count);
			foreach (var extension in detectors.extensions)
			{
				if (extension.Enabled)
				{
					enabledDetectors.Add(extension);
					
					if (extension is MissingComponentDetector) // unique detector, affects asset 
						missingComponentDetectorEnabled = true;
				}
			}
			
			// --------------------------
			// * non-standard detectors *
			// --------------------------
			
			// unique detector, affects game object tree skip
			missingPrefabDetector = GetFirstTypeFromCollection<MissingPrefabDetector>(true);
			
			// getting all standard detectors (both internal and external)
			assetIssueDetectors = GetTypesFromCollection<IAssetBeginIssueDetector>();
			settingsAssetIssueDetectors = GetTypesFromCollection<ISettingsAssetBeginIssueDetector>();
			sceneBeginIssueDetectors = GetTypesFromCollection<ISceneBeginIssueDetector>();
			goBeginIssueDetectors = GetTypesFromCollection<IGameObjectBeginIssueDetector>();
			componentBeginIssueDetectors = GetTypesFromCollection<IComponentBeginIssueDetector>();
			propertyIssueDetectors = GetTypesFromCollection<IPropertyIssueDetector>();
			unityEventIssueDetectors = GetTypesFromCollection<IUnityEventIssueDetector>();
			componentEndIssueDetectors = GetTypesFromCollection<IComponentEndIssueDetector>();
			goEndIssueDetectors = GetTypesFromCollection<IGameObjectEndIssueDetector>();
			sceneEndIssueDetectors = GetTypesFromCollection<ISceneEndIssueDetector>();
			
			//var unityEventListeners = FilterPropertyIssueDetectors<IUnityEventScanListener<DetectorResults>>();
			if (unityEventIssueDetectors.Length > 0)
			{
				if (unityEventScanner == null)
					unityEventScanner = new UnityEventIssuesScanner();
				unityEventScanner.RegisterScanListeners(unityEventIssueDetectors);
			}
			else
			{
				unityEventScanner = null;
			}
		}

		#region Scene

		public static void SceneBegin(AssetInfo asset)
		{
			currentLocation.AssetBegin(LocationGroup.Scene, asset);
			CheckSceneBeginIssues();
		}

		public static void SceneEnd(AssetInfo asset)
		{
			CheckSceneEndIssues();
			currentLocation.AssetEnd();
		}
		
		#endregion

		#region Prefab Asset

		public static void StartPrefabAsset(AssetInfo asset)
		{
			currentLocation.AssetBegin(LocationGroup.PrefabAsset, asset);
		}

		public static void EndPrefabAsset(AssetInfo asset)
		{
			currentLocation.AssetEnd();
		}
		
		#endregion

		#region Game Object (both from Scenes and Prefab Assets)
		
		public static bool StartGameObject(GameObject target, bool inPrefabInstance, out bool skipTree)
		{
			skipTree = false;

			if (!ProjectSettings.Issues.touchInactiveGameObjects)
			{
				if (currentLocation.Group == LocationGroup.Scene)
				{
					if (!target.activeInHierarchy) 
						return false;
				}
				else
				{
					if (!target.activeSelf) 
						return false;
				}
			}

			currentLocation.GameObjectBegin(target);

			if (inPrefabInstance)
			{
				if (missingPrefabDetector != null)
				{
					missingPrefabDetector.GameObjectBegin(foundIssues, currentLocation);
					if (missingPrefabDetector.IssueFound)
					{
						skipTree = true;
						currentLocation.GameObjectEnd();
						return false;
					}
				}
			}

			CheckGameObjectBeginIssues();

			return true;
		}

		public static void EndGameObject()
		{
			CheckGameObjectEndIssues();
			currentLocation.GameObjectEnd();
		}

		#endregion
		
		#region Game Object's Component
		
		public static void ProcessComponent(Component component, int orderIndex)
		{
			currentLocation.ComponentBegin(component, orderIndex);
			
#if !UNITY_2019_1_OR_NEWER
			if (missingComponentDetectorEnabled)
				MissingComponentDetector.Instance.TrackMissingComponent(currentLocation);
#endif
			// skipping missing components (we detected them earlier)
			if (component == null) 
				return;

			// skipping components we don't see in Inspector
			if ((component.hideFlags & HideFlags.HideInInspector) != 0)
				return;

			// optional disabled components skip
			if (!ProjectSettings.Issues.touchDisabledComponents && EditorUtility.GetObjectEnabled(component) == 0)
				return;

			// skipping ignored components
			if (CSFilterTools.HasEnabledFilters(ProjectSettings.Issues.componentIgnoresFilters) &&
				CSFilterTools.IsValueMatchesAnyFilterOfKind(currentLocation.ComponentName,
					ProjectSettings.Issues.componentIgnoresFilters, FilterKind.Type))
			{
				currentLocation.ComponentEnd();
				return;
			}

			CheckComponentBeginIssues();
			
			FilterOutActivePropertyIssueDetectors(currentLocation);
			if (ActivePropertyIssueDetectors.Count > 0)
			{
				var initialInfo = new SerializedObjectTraverseInfo(component);
				CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
				{
					currentLocation.PropertyBegin(property);
					
					CheckComponentPropertyIssues();

					if (unityEventScanner != null)
					{
						if (CSReflectionTools.IsPropertyIsSubclassOf<UnityEventBase>(property))
						{
							unityEventScanner.Property(foundIssues, currentLocation);
							info.skipChildren = true;
						}
					}

					currentLocation.PropertyEnd();
				});
			}

			CheckComponentEndIssues();

			//Debug.Log("ProcessComponent: " + target.name + ":" + component);
		}

		#endregion

		#region Assets (including settings assets)

		public static void ProcessAsset(AssetInfo asset)
		{
			currentLocation.AssetBegin(LocationGroup.Asset, asset);

			// skipping missing components on assets and record that if detector is enabled
			var argument = missingComponentDetectorEnabled ? foundIssues : null;
			if (MissingComponentDetector.Instance.AssetHasIssue(argument, currentLocation))
			{
				currentLocation.AssetEnd();
				return;
			}
			
			if (asset.Kind == AssetKind.Settings)
				CheckSettingsAssetIssues();
			else
				CheckAssetIssues();
			
			currentLocation.AssetEnd();
		}

		#endregion
		
		private static void CheckGameObjectBeginIssues()
		{
			foreach (var detector in goBeginIssueDetectors)
			{
				detector.GameObjectBegin(foundIssues, currentLocation);
			}
		}

		private static void CheckGameObjectEndIssues()
		{
			foreach (var detector in goEndIssueDetectors)
			{
				detector.GameObjectEnd(foundIssues, currentLocation);
			}
		}
		
		private static void CheckComponentBeginIssues()
		{
			foreach (var detector in componentBeginIssueDetectors)
			{
				if (!DetectorSupportsType(detector.ComponentTypes, currentLocation.ComponentType))
					continue;

				detector.ComponentBegin(foundIssues, currentLocation);
			}
		}
		
		private static void CheckComponentPropertyIssues()
		{
			foreach (var detector in propertyIssueDetectors)
			{
				detector.Property(foundIssues, currentLocation);
			}
		}
		
		private static void CheckComponentEndIssues()
		{
			foreach (var detector in componentEndIssueDetectors)
			{
				if (!DetectorSupportsType(detector.ComponentTypes, currentLocation.ComponentType))
					continue;
				
				detector.ComponentEnd(foundIssues, currentLocation);
			}
		}
		
		private static void CheckSettingsAssetIssues()
		{
			foreach (var detector in settingsAssetIssueDetectors)
			{
				// detector with SettingsKind == AssetSettingsKind.Undefined will scan all assets
				if (detector.SettingsKind != AssetSettingsKind.Undefined &&
					detector.SettingsKind != currentLocation.Asset.SettingsKind)
					continue;
				
				detector.AssetBegin(foundIssues, currentLocation);
			}
		}

		private static void CheckAssetIssues()
		{
			foreach (var detector in assetIssueDetectors)
			{
				if (!DetectorSupportsType(detector.AssetTypes, currentLocation.Asset.Type))
					continue;
				
				detector.AssetBegin(foundIssues, currentLocation);
			}
		}
		
		private static void CheckSceneBeginIssues()
		{
			foreach (var detector in sceneBeginIssueDetectors)
			{
				detector.SceneBegin(foundIssues, currentLocation);
			}
		}
		
		private static void CheckSceneEndIssues()
		{
			foreach (var detector in sceneEndIssueDetectors)
			{
				detector.SceneEnd(foundIssues, currentLocation);
			}
		}
		
		private static bool DetectorSupportsType(Type[] detectorTypes, Type target)
		{
			if (detectorTypes == null)
				return true;

			if (target == null)
				return false;

			var result = false;
			foreach (var detectorType in detectorTypes)
			{
				if (detectorType.IsAssignableFrom(target))
				{
					result = true;
					break;
				}
			}

			return result;
		}
		
		private static void FilterOutActivePropertyIssueDetectors(ComponentLocation location)
		{
			ActivePropertyIssueDetectors.Clear();
			
			foreach (var propertyIssueDetector in propertyIssueDetectors)
			{
				var depth = propertyIssueDetector.GetPropertyScanDepth(location);
				if (depth != PropertyScanDepth.None)
				{
					ActivePropertyIssueDetectors.Add(propertyIssueDetector);
				}
			}
		}
		
		/*private static List<T> FilterPropertyIssueDetectors<T>()
		{
			var result = new List<T>();
			foreach (var item in propertyIssueDetectors)
			{
				if (item is T)
					result.Add((T)item);
			}

			return result;
		}*/
		
		private static T GetFirstTypeFromCollection<T>(bool removeFromCollection = false)
		{
			T result = default(T);
			var types = GetTypesFromCollection<T>(removeFromCollection);
			
			if (types.Length > 0)
				result = types[0];
			
			if (types.Length > 1)
				Debug.LogWarning(Maintainer.ConstructLog("Found more than 1 type " + typeof(T) + " at the collection."));

			return result;
		}
		
		private static T[] GetTypesFromCollection<T>(bool removeFromCollection = false)
		{ 
			var result = new List<T>();
			var count = enabledDetectors.Count - 1;
			for (var i = count; i >= 0; i--)
			{
				var extension = enabledDetectors[i];
				if (extension is T)
				{
					result.Add((T)extension);
					if (removeFromCollection)
						enabledDetectors.Remove(extension);
				}
			}
			return result.ToArray();
		}

		private class IssueDetectorComparer : IComparer<IIssueDetector>
		{
			public int Compare(IIssueDetector x, IIssueDetector y)
			{
				if (ReferenceEquals(x, y))
					return 0;

				if (ReferenceEquals(x, null))
					return 0;

				if (ReferenceEquals(y, null))
					return 0;

				// by severity from more to less
				var result = ((int)y.Info.Severity).CompareTo((int)x.Info.Severity);
				
				// if severity equals, by id - from a to z
				return result != 0 ? result : string.Compare(x.Id, y.Id, StringComparison.Ordinal);
			}
		}
	}
}
