#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using Cleaner;
	using EditorCommon.Tools;
	using Settings;
	using Tools;
	using Filters;
	using UnityEditor;
	using UnityEngine;
	
	internal partial class CleanerTab
	{
		protected override void DrawLeftColumnHeader()
		{
			base.DrawLeftColumnHeader();

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Space(10);
					if (UIHelpers.ImageButton("1. Scan Project", CSEditorIcons.Search))
					{
						EditorApplication.delayCall += StartSearch;
					}
					GUILayout.Space(5);
					if (UIHelpers.ImageButton("2. Delete selected garbage", CSIcons.Delete))
					{
						EditorApplication.delayCall += StartClean;
					}
					GUILayout.Space(10);
				}
				GUILayout.Space(10);
			}
		}
		
		protected override void DrawLeftColumnBody()
		{
			using (new GUILayout.VerticalScope(/*UIHelpers.panelWithBackground*/))
			{
#if  UNITY_2019_3_OR_NEWER
				if (!BuildReportAnalyzer.IsReportExists())
				{
					EditorGUILayout.HelpBox("No build data found: search will be more accurate if you'll make a build", MessageType.Warning);
				}
#endif
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					using (new GUILayout.VerticalScope(/*UIHelpers.panelWithButtonBackground*/))
					{
						GUILayout.Label("<b><size=16>Settings</size></b>", UIHelpers.richLabel);
						UIHelpers.Separator();
					}
					GUILayout.Space(10);
				}

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);

					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(10);

						if (UIHelpers.ImageButton("Filters (" + ProjectSettings.Cleaner.GetFiltersCount() + ")", CSIcons.Filter))
						{
							CleanerFiltersWindow.Create();
						}

						GUILayout.Space(10);
						GUILayout.Label("Treat as used:");
						ProjectSettings.Cleaner.scenesSelection = (ProjectCleanerSettings.ScenesSelection)EditorGUILayout.EnumPopup(ProjectSettings.Cleaner.scenesSelection);
						GUILayout.Space(10);

						EditorGUI.BeginChangeCheck();
						ProjectSettings.Cleaner.useTrashBin = EditorGUILayout.ToggleLeft(new GUIContent("Use Trash Bin (slows cleanup)", "All deleted items will be moved to Trash if selected. Otherwise items will be deleted permanently.\nPlease note: dramatically reduces removal performance when enabled!"), ProjectSettings.Cleaner.useTrashBin);
						if (EditorGUI.EndChangeCheck())
						{
							if (!ProjectSettings.Cleaner.useTrashBin && !UserSettings.Cleaner.trashBinWarningShown)
							{
								EditorUtility.DisplayDialog(ProjectCleaner.ModuleName, "Please note, in case of not using Trash Bin, files will be removed permanently, without possibility to recover them in case of mistake.\nAuthor is not responsible for any damage made due to the module usage!\nThis message shows only once.", "Dismiss");
								UserSettings.Cleaner.trashBinWarningShown = true;
							}
						}

						ProjectSettings.Cleaner.rescanAfterContextIgnore = EditorGUILayout.ToggleLeft(new GUIContent("Rescan after new context ignore", "Project scan will be automatically started after you add any new ignore from the results more button context menu.\nProject scan is necessary to automatically exclude all referenced items from garbage too."), ProjectSettings.Cleaner.rescanAfterContextIgnore);
						ProjectSettings.Cleaner.ignoreEditorAssets = EditorGUILayout.ToggleLeft(
							new GUIContent("Ignore Editor assets",
								"Ignore (i.e. treat as used) all assets placed at Editor-only " +
								"special folders 'Gizmos', 'Editor', 'Editor Default Resources' " +
								"and all assets placed next to the Editor-only Assembly Definitions " +
								"and all their child folders."),
							ProjectSettings.Cleaner.ignoreEditorAssets);

						GUILayout.Space(5);
						GUILayout.Label("<b><size=12>Search for:</size></b>", UIHelpers.richLabel);
						UIHelpers.Separator();
						using (new GUILayout.HorizontalScope())
						{
							ProjectSettings.Cleaner.findUnreferencedAssets = EditorGUILayout.ToggleLeft(new GUIContent("Unused assets", "Search for unreferenced assets in project."), ProjectSettings.Cleaner.findUnreferencedAssets, GUILayout.Width(105));
						}
						using (new GUILayout.HorizontalScope())
						{
							ProjectSettings.Cleaner.findEmptyFolders = EditorGUILayout.ToggleLeft(new GUIContent("Empty folders", "Search for all empty folders in project."), ProjectSettings.Cleaner.findEmptyFolders, GUILayout.Width(100));
							GUI.enabled = ProjectSettings.Cleaner.findEmptyFolders;
							EditorGUI.BeginChangeCheck();
							ProjectSettings.Cleaner.findEmptyFoldersAutomatically = EditorGUILayout.ToggleLeft(new GUIContent("Autoclean", "Perform empty folders clean automatically on every scripts reload."), ProjectSettings.Cleaner.findEmptyFoldersAutomatically, GUILayout.Width(100));
							if (EditorGUI.EndChangeCheck())
							{
								if (ProjectSettings.Cleaner.findEmptyFoldersAutomatically)
									EditorUtility.DisplayDialog(ProjectCleaner.ModuleName, "In case you're having thousands of folders in your project this may hang Unity for few additional secs on every scripts reload.\n" + Maintainer.DataLossWarning, "Understood");
							}
							GUI.enabled = true;
						}

						GUILayout.Space(10);

						if (UIHelpers.ImageButton("Reset", "Resets settings to defaults.", CSIcons.Restore))
						{
							ProjectSettings.Cleaner.Reset();
						}

						GUILayout.Space(10);
					}

					GUILayout.Space(10);
				}
			}

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
				{
					GUILayout.Label("<b><size=16>Stats</size></b>", UIHelpers.richLabel);
					UIHelpers.Separator();
					GUILayout.Space(10);
					DrawStatsBody();
				}
				GUILayout.Space(10);
			}
		}
		
		private void DrawStatsBody()
		{
			using (new GUILayout.HorizontalScope(UIHelpers.panelWithBackground))
			{
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Space(5);
					if (resultsStats == null)
					{
						GUILayout.Label("N/A");
					}
					else
					{
						GUILayout.Space(5);
						GUILayout.Label("Physical size");
						UIHelpers.Separator();
						GUILayout.Label("Total found: " + CSEditorTools.FormatBytes(resultsStats.totalSize));
						GUILayout.Label("Selected: " + CSEditorTools.FormatBytes(resultsStats.selectedSize));
					}

					GUILayout.Space(5);
				}
				GUILayout.Space(10);
			}
		}
	}
}