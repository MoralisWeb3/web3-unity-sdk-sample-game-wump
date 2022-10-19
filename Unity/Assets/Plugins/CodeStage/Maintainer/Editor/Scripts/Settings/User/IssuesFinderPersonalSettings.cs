#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#pragma warning disable 0414

namespace CodeStage.Maintainer.Settings
{
	using System;
	using Cleaner;
	using Issues;
	using Issues.Detectors;
	using UI;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Issues Finder module settings saved in Library or UserSettings (since Unity 2020.1) folder.
	/// </summary>
	/// Contains user-specific settings for this module.
	/// See IDE hints for all list.
	[Serializable]
	public class IssuesFinderPersonalSettings
	{
		public int filtersTabIndex = 0;

		[SerializeField] internal RecordsTabState tabState = new RecordsTabState();
		/* sorting */
		[SerializeField] internal IssuesSortingType sortingType = IssuesSortingType.BySeverity;
		[SerializeField] internal SortingDirection sortingDirection = SortingDirection.Ascending;

		[Serializable]
		public class DetectorsGroupFoldout
		{
			public IssueGroup group;
			public bool expanded = true;
		}
		
		[SerializeField] internal DetectorsGroupFoldout[] groupsFoldouts;
		
		[SerializeField] internal bool scanGameObjectsFoldout;
		[SerializeField] internal bool gameObjectsFoldout;
		[SerializeField] internal bool commonFoldout;
		[SerializeField] internal bool neatnessFoldout;
		[SerializeField] internal bool projectSettingsFoldout;

		public IssuesFinderPersonalSettings()
		{
			Reset();
		}
		
		internal bool GetGroupFoldout(IssueGroup group)
		{
			if (groupsFoldouts == null)
				groupsFoldouts = new DetectorsGroupFoldout[0];
			
			foreach (var foldout in groupsFoldouts)
			{
				if (foldout.group == group)
					return foldout.expanded;
			}

			var newSetting = new DetectorsGroupFoldout { group = group, expanded = true };
			ArrayUtility.Add(ref groupsFoldouts, newSetting);

			return false;
		}
		
		internal void SetGroupFoldout(IssueGroup group, bool value)
		{
			if (groupsFoldouts == null)
				groupsFoldouts = new DetectorsGroupFoldout[0];
			
			foreach (var foldout in groupsFoldouts)
			{
				if (foldout.group != group)
					continue;

				foldout.expanded = value;
				return;
			}
			
			var newSetting = new DetectorsGroupFoldout { group = group, expanded = value };
			ArrayUtility.Add(ref groupsFoldouts, newSetting);
		}
		
		internal void Reset()
		{
			gameObjectsFoldout = true;
			commonFoldout = false;
			neatnessFoldout = false;
			projectSettingsFoldout = true;
		}
	}
}