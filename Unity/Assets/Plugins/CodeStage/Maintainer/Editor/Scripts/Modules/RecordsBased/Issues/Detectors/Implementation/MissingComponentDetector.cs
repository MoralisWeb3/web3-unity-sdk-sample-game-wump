#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;
	using Tools;
#if UNITY_2019_1_OR_NEWER
	using UnityEditor;
#endif

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once ClassNeverInstantiated.Global since it's used from TypeCache
	internal class MissingComponentDetector : IssueDetector, 
		IGameObjectBeginIssueDetector
#if !UNITY_2019_1_OR_NEWER
		, IGameObjectEndIssueDetector
#endif
	{
		public static MissingComponentDetector Instance { get; private set; }
		
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.Component,
				DetectorKind.Defect,
				IssueSeverity.Error,
				"Missing Component", 
				"Search for the missing Components on the Game Objects or Scriptable Objects.");
		}}

		public Type[] AssetTypes { get { return null; } } // we are checking all assets
		public Type[] ComponentTypes { get { return null; } } // we are checking all components

#if !UNITY_2019_1_OR_NEWER
		private int missingComponentsCount;
#endif
		
#if !UNITY_2019_2_OR_NEWER
		static MissingComponentDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new MissingComponentDetector());
		}
#endif

		public MissingComponentDetector()
		{
			Instance = this;
		}
		
		public bool AssetHasIssue(DetectorResults results, AssetLocation location)
		{
			if (location.Asset.Type != null)
				return false;

			if (!CSAssetTools.IsAssetScriptableObjectWithMissingScript(location.Asset.Path))
				return false;

			if (results != null)
			{
				var record = UnityObjectAssetIssueRecord.Create(this, IssueKind.MissingComponent, location);
				results.Add(record);
			}

			return true;
		}

		public void GameObjectBegin(DetectorResults results, GameObjectLocation location)
		{
#if !UNITY_2019_1_OR_NEWER
			missingComponentsCount = 0;
#else
			var missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(location.GameObject);
			var issue = TryGenerateIssue(missingCount, location);
			if (issue != null)
				results.Add(issue);
#endif
		}
		
#if !UNITY_2019_1_OR_NEWER
		public void TrackMissingComponent(ComponentLocation location)
		{
			if (location.Component != null) 
				return;

			missingComponentsCount++;
		}

		public void GameObjectEnd(DetectorResults results, GameObjectLocation location)
		{
			var issue = TryGenerateIssue(missingComponentsCount, location);
			if (issue != null)
				results.Add(issue);
		}
#endif

		private IssueRecord TryGenerateIssue(int missingCount, GameObjectLocation location)
		{
			if (missingCount == 0)
				return null;

			var narrow = location.Narrow();
			narrow.ComponentOverride(null, null, -1);
			
			var record = GameObjectIssueRecord.ForComponent(this, Issues.IssueKind.MissingComponent,  narrow);
			if (missingCount > 1)
			{
				record.Header = string.Format("{0} missing components", missingCount);
			}
			
			return record;
		}
	}
}