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
	using UnityEngine;

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class HugePositionDetector : IssueDetector, IGameObjectBeginIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.GameObject,
				DetectorKind.Neatness,
				IssueSeverity.Info,
				"Huge position", 
				"Search for Game Objects with huge world transform positions (> |100 000| on any axis).");
		}}
		
#if !UNITY_2019_2_OR_NEWER
		static HugePositionDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new HugePositionDetector());
		}
#endif
		
		public void GameObjectBegin(DetectorResults results, GameObjectLocation location)
		{
			if (!IsTransformHasHugePosition(location.GameObject.transform))
				return;

			var narrow = location.Narrow();
			narrow.ComponentOverride(CSReflectionTools.transformType, "Transform", 0);
			narrow.PropertyOverride("Position");
			var issue = GameObjectIssueRecord.ForProperty(this, Issues.IssueKind.HugePosition, narrow);
			results.Add(issue);
		}

		private static bool IsTransformHasHugePosition(Transform transform)
		{
			var position = transform.position;
			return Math.Abs(position.x) > 100000f || Math.Abs(position.y) > 100000f || Math.Abs(position.z) > 100000f;
		}
	}
}