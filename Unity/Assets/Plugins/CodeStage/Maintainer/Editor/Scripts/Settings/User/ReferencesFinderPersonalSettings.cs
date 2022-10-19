#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using UI;
	using Core;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// References Finder module settings saved in Library or UserSettings (since Unity 2020.1) folder.
	/// </summary>
	/// Contains user-specific settings for this module.
	/// See IDE hints for all list.
	[Serializable]
	public class ReferencesFinderPersonalSettings
	{
		[SerializeField]
		private bool deepProjectSearch;

		/// <summary>
		/// Includes more items into the Project References search, like fields with [HideInInspector] attribute or hidden system fields.
		/// </summary>
		/// Can reduce search performance and produce confusing results in some scenarios.
		/// Changing this setting will trigger Exact References Entries cleanup.
		public bool DeepProjectSearch
		{
			get
			{
				return deepProjectSearch;
			}
			set
			{
				deepProjectSearch = value;
				AssetsMap.ResetReferenceEntries();
			}
		}
		
		/// <summary>
		/// Includes more items into the Hierarchy References search, like fields with [HideInInspector] attribute or hidden system fields.
		/// </summary>
		/// Can reduce search performance and produce confusing results in some scenarios.
		[field:SerializeField]
		public bool DeepHierarchySearch { get; set; }
		
		public bool showAssetsWithoutReferences;

		[FormerlySerializedAs("selectedFindClearsResults")]
		public bool selectedFindClearsProjectResults;
		public bool clearHierarchyResults;

		public bool fullProjectScanWarningShown;

		[SerializeField] internal TreeViewState projectReferencesTreeViewState;
		[SerializeField] internal MultiColumnHeaderState projectReferencesTreeHeaderState;
		[SerializeField] internal TreeViewState hierarchyReferencesTreeViewState;
		[SerializeField] internal MultiColumnHeaderState sceneReferencesTreeHeaderState;
		[SerializeField] internal string splitterState;

		public ReferenceFinderTab selectedTab;
	}
}