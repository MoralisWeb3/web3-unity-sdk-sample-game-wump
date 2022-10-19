#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using Core.Scan;
	using UnityEngine;

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class InvalidLayerDetector : IssueDetector, IGameObjectBeginIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.GameObject,
				DetectorKind.Defect,
				IssueSeverity.Info,
				"Invalid Layer",
				"Search for Game Objects with invalid (empty) Layers.");
		}}
		
#if !UNITY_2019_2_OR_NEWER
		static InvalidLayerDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new InvalidLayerDetector());
		}
#endif
		
		public void GameObjectBegin(DetectorResults results, GameObjectLocation location)
		{
			var layerIndex = location.GameObject.layer;
			
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(layerIndex))) 
				return;

			var issue = GameObjectIssueRecord.ForGameObject(this, Issues.IssueKind.UnnamedLayer, location);
			issue.HeaderPostfix = "(index: " + layerIndex + ")";
			results.Add(issue);
		}
	}
}