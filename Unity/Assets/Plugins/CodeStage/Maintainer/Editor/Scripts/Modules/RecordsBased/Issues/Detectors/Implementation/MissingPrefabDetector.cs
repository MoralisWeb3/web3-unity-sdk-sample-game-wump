#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using Core.Scan;
	using Tools;
	
#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once ClassNeverInstantiated.Global since it's used from TypeCache
	internal class MissingPrefabDetector : IssueDetector, IGameObjectBeginIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.GameObject,
				DetectorKind.Defect,
				IssueSeverity.Error,
				"Missing Prefab", 
				"Search for instances of Prefabs which were removed from project.");
		}}
		
		public bool IssueFound { get; private set; }
		
#if !UNITY_2019_2_OR_NEWER
		static MissingPrefabDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new MissingPrefabDetector());
		}
#endif
		
		public void GameObjectBegin(DetectorResults results, GameObjectLocation location)
		{
			IssueFound = false;
			if (!CSPrefabTools.IsMissingPrefabInstance(location.GameObject)) 
				return;

			IssueFound = true;
			var issue = GameObjectIssueRecord.ForGameObject(this, IssueKind.MissingPrefab, location);
			results.Add(issue);
		}
	}
}