#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using References;
	using UnityEngine;
	using Settings;
	using System;
	using EditorCommon.Tools;
	using UnityEditor;

	public enum ReferenceFinderTab
	{
		Project = 0,
		Scene = 1,
	}

	internal partial class ReferencesTab : TwoColumnsTab
	{
		[NonSerialized]
		private ReferenceFinderTab currentTab;

		[NonSerialized]
		private readonly GUIContent[] tabsCaptions;

		[NonSerialized]
		private readonly ProjectReferencesTab projectTab;

		[NonSerialized]
		private readonly HierarchyReferencesTab hierarchyTab;

		protected override string CaptionName
		{
			get { return ReferencesFinder.ModuleName; }
		}

		protected override Texture CaptionIcon
		{
			get { return CSEditorIcons.Search; }
		}

		public ReferencesTab(MaintainerWindow window) : base(window)
		{
			if (projectTab == null)
				projectTab = new ProjectReferencesTab(window);

			if (hierarchyTab == null)
				hierarchyTab = new HierarchyReferencesTab(window);

			if (tabsCaptions == null)
			{
				tabsCaptions = new[] { projectTab.Caption, hierarchyTab.Caption };
			}
		}

		public override void Refresh(bool newData)
		{
			base.Refresh(newData);

			currentTab = UserSettings.Instance.referencesFinder.selectedTab;

			switch (currentTab)
			{
				case ReferenceFinderTab.Project:
					projectTab.Refresh(newData);
					break;
				case ReferenceFinderTab.Scene:
					hierarchyTab.Refresh(newData);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override bool DrawRightColumnCenter()
		{
			switch (currentTab)
			{
				case ReferenceFinderTab.Project:
					projectTab.DrawRightColumn();
					break;
				case ReferenceFinderTab.Scene:
					hierarchyTab.DrawRightColumn();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return true;
		}

		protected override void DrawRightColumnBottom()
		{
			switch (currentTab)
			{
				case ReferenceFinderTab.Project:
					projectTab.DrawFooter();
					break;
				case ReferenceFinderTab.Scene:
					hierarchyTab.DrawFooter();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
