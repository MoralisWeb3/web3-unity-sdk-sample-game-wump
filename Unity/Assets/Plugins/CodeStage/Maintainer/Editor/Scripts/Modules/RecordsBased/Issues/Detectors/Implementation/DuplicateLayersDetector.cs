#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using Core;
	using Core.Scan;

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class DuplicateLayersDetector : IssueDetector, ISettingsAssetBeginIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.ProjectSettings,
				DetectorKind.Defect,
				IssueSeverity.Info,
				"Duplicate Layers", 
				"Search for the duplicate Layers and Sorting Layers at the 'Tags and Layers' Project Settings.");
		}}
		
		public AssetSettingsKind SettingsKind { get { return AssetSettingsKind.TagManager; } }
		
#if !UNITY_2019_2_OR_NEWER
		static DuplicateLayersDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new DuplicateLayersDetector());
		}
#endif
		
		public void AssetBegin(DetectorResults results, AssetLocation location)
		{
			var issue = SettingsChecker.CheckTagsAndLayers(this, location);
			if (issue != null)
			{
				issue.HeaderPostfix = "at the 'Tags and Layers' settings";
				results.Add(issue);
			}
		}
	}
}