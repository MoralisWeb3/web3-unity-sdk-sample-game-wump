#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System;
	using System.Collections.Generic;
	using EditorCommon.Tools;
	using Filters;
	using Issues;
	using Issues.Detectors;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	internal partial class IssuesTab
	{
		private static SortedDictionary<IssueGroup, IList<IIssueDetector>> detectorsGroups;
		private bool drawDetectors;
		
		
		protected override void DrawLeftColumnHeader()
		{
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

					if (UIHelpers.ImageButton("2. Fix fixable selected issues", CSIcons.AutoFix))
					{
						EditorApplication.delayCall += StartFix;
					}
					GUILayout.Space(10);
				}
				GUILayout.Space(10);
			}
		}
		
		protected override void DrawLeftColumnBody()
		{
			// -----------------------------------------------------------------------------
			// filtering settings
			// -----------------------------------------------------------------------------

			DrawWhereSection(); 

			GUILayout.Space(5);
			DrawWhatSectionLegacy(ref leftColumnScrollPosition);
			//DrawWhatSection(ref leftColumnScrollPosition);
			GUILayout.Space(10);

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);
				if (UIHelpers.ImageButton("Reset Settings", "Resets settings to defaults.", CSIcons.Restore))
				{
					ProjectSettings.Issues.Reset();
					UserSettings.Issues.Reset();
				}
				GUILayout.Space(10);
			}
			GUILayout.Space(10);
		}

		private void DrawWhereSection(/*ref Vector2 settingsSectionScrollPosition*/)
		{
			// -----------------------------------------------------------------------------
			// where to look
			// -----------------------------------------------------------------------------

			using (new GUILayout.VerticalScope(/*UIHelpers.panelWithBackground*/))
			{
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label("<b><size=16>Where</size></b>", UIHelpers.richLabel);
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

						if (UIHelpers.ImageButton("Filters (" + ProjectSettings.Issues.GetFiltersCount() + ")", CSIcons.Filter))
						{
							IssuesFiltersWindow.Create();
						}

						GUILayout.Space(10);

						using (new GUILayout.HorizontalScope())
						{
							ProjectSettings.Issues.lookInScenes = EditorGUILayout.ToggleLeft(new GUIContent("Scenes",
									"Uncheck to exclude all scenes from search or select filtering level:\n\n" +
									"All Scenes: all project scenes with respect to configured filters.\n" +
									"Included Scenes: scenes included via Manage Filters > Scene Includes.\n" +
									"Current Scene: currently opened scene including any additional loaded scenes."),
								ProjectSettings.Issues.lookInScenes, GUILayout.Width(70));
							GUI.enabled = ProjectSettings.Issues.lookInScenes;
							ProjectSettings.Issues.scenesSelection = (IssuesFinderSettings.ScenesSelection)EditorGUILayout.EnumPopup(ProjectSettings.Issues.scenesSelection);
							GUI.enabled = true;
						}

						ProjectSettings.Issues.lookInAssets = EditorGUILayout.ToggleLeft(new GUIContent("File assets", "Uncheck to exclude all file assets like prefabs, ScriptableObjects and such from the search. Check readme for additional details."), ProjectSettings.Issues.lookInAssets);
						ProjectSettings.Issues.lookInProjectSettings = EditorGUILayout.ToggleLeft(new GUIContent("Project Settings", "Uncheck to exclude project settings file assets like PlayerSettings and such from the search."), ProjectSettings.Issues.lookInProjectSettings);

						UIHelpers.Separator(5);

						var canScanGamObjects = ProjectSettings.Issues.lookInScenes || ProjectSettings.Issues.lookInAssets;
						GUI.enabled = canScanGamObjects;
						var scanGameObjects = UIHelpers.ToggleFoldout(ref ProjectSettings.Issues.scanGameObjects, ref UserSettings.Issues.scanGameObjectsFoldout, new GUIContent("Game Objects", "Specify if you wish to look for GameObjects issues."), GUILayout.Width(110));
						GUI.enabled = scanGameObjects && canScanGamObjects;
						if (UserSettings.Issues.scanGameObjectsFoldout)
						{
							UIHelpers.IndentLevel();
							ProjectSettings.Issues.touchInactiveGameObjects = EditorGUILayout.ToggleLeft(new GUIContent("Inactive Game Objects", "Uncheck to exclude all inactive Game Objects from the search."), ProjectSettings.Issues.touchInactiveGameObjects);
							ProjectSettings.Issues.touchDisabledComponents = EditorGUILayout.ToggleLeft(new GUIContent("Disabled Components", "Uncheck to exclude all disabled Components from the search."), ProjectSettings.Issues.touchDisabledComponents);
							UIHelpers.UnIndentLevel();
						}

						GUI.enabled = true;
					}

					GUILayout.Space(10);
				}

				GUILayout.Space(10);
			}
		}

		private void DrawWhatSectionLegacy(ref Vector2 settingsSectionScrollPosition)
		{
			// -----------------------------------------------------------------------------
			// what to look for
			// -----------------------------------------------------------------------------

			using (new GUILayout.VerticalScope(/*UIHelpers.panelWithBackground*/))
			{
				DrawSettingsSearchSectionHeader(SettingsSearchSection.All, "<b><size=16>What</size></b>");
				settingsSectionScrollPosition = GUILayout.BeginScrollView(settingsSectionScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);

					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(10);

						// -----------------------------------------------------------------------------
						// Defect Issues
						// -----------------------------------------------------------------------------

						ProjectSettings.Issues.MissingReferences = EditorGUILayout.ToggleLeft(new GUIContent("Missing references", "Search for any missing references in Components, Project Settings, Scriptable Objects, and so on."), ProjectSettings.Issues.MissingReferences);

#if UNITY_2019_1_OR_NEWER
						ProjectSettings.Issues.ShadersWithErrors = EditorGUILayout.ToggleLeft(new GUIContent("Shaders errors", "Search for shaders with compilation errors."), ProjectSettings.Issues.ShadersWithErrors);
#endif
						UIHelpers.Separator(5);

						// -----------------------------------------------------------------------------
						// Game Object Issues
						// -----------------------------------------------------------------------------

						using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground))
						{
							UIHelpers.Foldout(ref UserSettings.Issues.gameObjectsFoldout, "<b>Game Object Issues</b>");
						}

						if (UserSettings.Issues.gameObjectsFoldout)
						{
							GUILayout.Space(-2);
							UIHelpers.IndentLevel();
							if (DrawSettingsFoldoutSectionHeader(SettingsSearchSection.Common, ref UserSettings.Issues.commonFoldout))
							{
								UIHelpers.IndentLevel();
								ProjectSettings.Issues.MissingComponents = EditorGUILayout.ToggleLeft(new GUIContent("Missing components", "Search for the missing components on the Game Objects."), ProjectSettings.Issues.MissingComponents);
								ProjectSettings.Issues.MissingPrefabs = EditorGUILayout.ToggleLeft(new GUIContent("Missing prefabs", "Search for instances of prefabs which were removed from project."), ProjectSettings.Issues.MissingPrefabs);
								ProjectSettings.Issues.DuplicateComponents = EditorGUILayout.ToggleLeft(new GUIContent("Duplicate components", "Search for the multiple instances of the same component with same values on the same object."), ProjectSettings.Issues.DuplicateComponents);
								ProjectSettings.Issues.InconsistentTerrainData = EditorGUILayout.ToggleLeft(new GUIContent("Inconsistent Terrain Data", "Search for Game Objects where Terrain and TerrainCollider have different Terrain Data."), ProjectSettings.Issues.InconsistentTerrainData);
								UIHelpers.UnIndentLevel();
							}

							if (DrawSettingsFoldoutSectionHeader(SettingsSearchSection.Neatness, ref UserSettings.Issues.neatnessFoldout))
							{
								UIHelpers.IndentLevel();
								ProjectSettings.Issues.InvalidLayers = EditorGUILayout.ToggleLeft(new GUIContent("Objects with unnamed layers", "Search for GameObjects with unnamed layers."), ProjectSettings.Issues.InvalidLayers);
								ProjectSettings.Issues.HugePositions = EditorGUILayout.ToggleLeft(new GUIContent("Objects with huge positions", "Search for GameObjects with huge world positions (> |100 000| on any axis)."), ProjectSettings.Issues.HugePositions);
								UIHelpers.UnIndentLevel();
							}
							UIHelpers.UnIndentLevel();
							GUILayout.Space(5);
						}

						GUI.enabled = true;

						// -----------------------------------------------------------------------------
						// Project Settings Issues
						// -----------------------------------------------------------------------------

						using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground))
						{
							UIHelpers.Foldout(ref UserSettings.Issues.projectSettingsFoldout, "<b>Project Settings Issues</b>");
						}

						if (UserSettings.Issues.projectSettingsFoldout)
						{
							UIHelpers.IndentLevel();
							ProjectSettings.Issues.DuplicateLayers = EditorGUILayout.ToggleLeft(new GUIContent("Duplicate Layers", "Search for the duplicate layers and sorting layers at the 'Tags and Layers' Project Settings."), ProjectSettings.Issues.DuplicateLayers);
							UIHelpers.UnIndentLevel();
						}
						GUI.enabled = true;

						GUILayout.Space(10);
					}
					GUILayout.Space(10);
				}
				GUILayout.EndScrollView();
			}
		}
		
		private void DrawWhatSection(ref Vector2 settingsSectionScrollPosition)
		{
			// change vars affecting OnGUI layout at the EventType.Layout
			// to avoid errors while painting OnGUI
			if (Event.current.type == EventType.Layout)
			{
				drawDetectors = true;
				if (detectorsGroups == null)
				{
					if (IssuesFinderDetectors.detectors == null)
						drawDetectors = false;
					else
						FillDetectorsGroups(IssuesFinderDetectors.detectors.extensions);
				}
			}
			
			// -----------------------------------------------------------------------------
			// what to look for
			// -----------------------------------------------------------------------------

			using (new GUILayout.VerticalScope(/*UIHelpers.panelWithBackground*/))
			{
				DrawSettingsSearchSectionHeader(SettingsSearchSection.All, "<b><size=16>What</size></b>");
				settingsSectionScrollPosition = GUILayout.BeginScrollView(settingsSectionScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);

					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(10);

						if (!drawDetectors)
							GUILayout.Label("Waiting for detectors...");
						else
							DrawDetectors(); 

						GUILayout.Space(10);
					}
					GUILayout.Space(10);
				}
				GUILayout.EndScrollView();
			}
		}

		private void DrawDetectors()
		{
			foreach (var detectorsGroup in detectorsGroups)
			{
				if (detectorsGroup.Key != IssueGroup.Global)
				{
					if (!DrawGroupFoldout(detectorsGroup.Key, detectorsGroup.Value))
						continue;
					
					UIHelpers.IndentLevel();
					GUILayout.Space(5);
				}
				
				foreach (var detector in detectorsGroup.Value)
				{
					DrawDetectorSetting(detector);
				}

				if (detectorsGroup.Key != IssueGroup.Global)
					UIHelpers.UnIndentLevel();
				
				GUILayout.Space(5);
			}
		}

		private bool DrawGroupFoldout(IssueGroup group, IList<IIssueDetector> detectorsInGroup)
		{
			bool foldout;
			using (new GUILayout.HorizontalScope(UIHelpers.inspectorTitlebar))
			{
				var allEnabled = true;
				var allDisabled = true;

				foreach (var detector in detectorsInGroup)
				{
					if (!detector.Enabled)
						allEnabled = false;
					else
						allDisabled = false;
				}

				var mixedSelection = !(allEnabled || allDisabled);
				
				var guiContent = new GUIContent(" " + CSEditorTools.NicifyName(group.ToString()) + " Issues",
					GetGroupIcon(group));
				foldout = UserSettings.Issues.GetGroupFoldout(group);
				var toggle = mixedSelection || allEnabled;
				bool toggleChanged;
				bool foldoutChanged;

				UIHelpers.ToggleFoldout(ref toggle, mixedSelection, ref foldout, 
					out toggleChanged, out foldoutChanged,
					guiContent,
					UIHelpers.richFoldout,
					GUILayout.Width(195));
				
				if (toggleChanged)
					ProjectSettings.Issues.SwitchDetectors(detectorsInGroup, toggle);
				
				if (foldoutChanged)
					UserSettings.Issues.SetGroupFoldout(group, foldout);
			}

			return foldout;
		}

		private void DrawDetectorSetting(IIssueDetector detector)
		{
			using (var change = new EditorGUI.ChangeCheckScope())
			{
				using (new GUILayout.HorizontalScope())
				{
					var enabled = detector.Enabled;
					
					//DrawSeverityIcon(detector.Info.Severity, CSColorTools.DimmedColor);
					
					var originalLabelWidth = EditorGUIUtility.labelWidth;
					var originalFieldWidth = EditorGUIUtility.fieldWidth;
					EditorGUIUtility.labelWidth = 1;
					EditorGUIUtility.fieldWidth = 1;
					
					var guiContent = detector.Info.GetGUIContent();
					var value = EditorGUILayout.ToggleLeft(guiContent, enabled, GUILayout.ExpandWidth(true));
					
					EditorGUIUtility.fieldWidth = originalFieldWidth;
					EditorGUIUtility.labelWidth = originalLabelWidth;
					
					if (change.changed)
						detector.Enabled = value;
					
					DrawSeverityIcon(detector.Info.Severity, CSColorTools.BrightGreyDimmed);
					
					GUILayout.Space(2);
				}
			}
		}

		private void FillDetectorsGroups(IList<IIssueDetector> detectors)
		{
			if (detectorsGroups != null)
				return;
			
			detectorsGroups = new SortedDictionary<IssueGroup, IList<IIssueDetector>>();
			
			foreach (var detector in detectors)
			{
				if (!detectorsGroups.ContainsKey(detector.Info.Group))
				{
					detectorsGroups.Add(detector.Info.Group, new List<IIssueDetector>());
				}
				detectorsGroups[detector.Info.Group].Add(detector);
			}
		}

		private bool DrawSettingsFoldoutSectionHeader(SettingsSearchSection section, ref bool foldout)
		{
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				foldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(true, GUILayout.Width(100)), foldout, ObjectNames.NicifyVariableName(section.ToString()), true, UIHelpers.richFoldout);
				GUILayout.FlexibleSpace();

				if (UIHelpers.IconButton(CSIcons.SelectAll, "Select all"))
				{
					SettingsSectionGroupSwitch(section, true);
				}

				if (UIHelpers.IconButton(CSIcons.SelectNone, "Clear selection"))
				{
					SettingsSectionGroupSwitch(section, false);
				}

				GUILayout.Space(5);
			}

			/*if (foldout)
			{
				UIHelpers.Separator();
			}*/

			return foldout;
		}

		private void DrawSettingsSearchSectionHeader(SettingsSearchSection section, string caption)
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);

				GUILayout.Label(caption, UIHelpers.richLabel, GUILayout.Width(100));

				GUILayout.FlexibleSpace();
				using (new GUILayout.VerticalScope())
				{
					//GUILayout.Space(-3);
					using (new GUILayout.HorizontalScope())
					{
						if (UIHelpers.IconButton(CSIcons.SelectAll, "Select all"))
						{
							SettingsSectionGroupSwitch(section, true);
						}

						if (UIHelpers.IconButton(CSIcons.SelectNone, "Clear selection"))
						{
							SettingsSectionGroupSwitch(section, false);
						}
					}
				}
				GUILayout.Space(10);
			}

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(10);
				UIHelpers.Separator();
				GUILayout.Space(10);
			}
		}

		private void SettingsSectionGroupSwitch(SettingsSearchSection section, bool enable)
		{
			switch (section)
			{
				case SettingsSearchSection.Common:
					ProjectSettings.Issues.SwitchCommon(enable);
					break;
				case SettingsSearchSection.Neatness:
					ProjectSettings.Issues.SwitchNeatness(enable);
					break;
				case SettingsSearchSection.All:
					ProjectSettings.Issues.SwitchAllIssues(enable);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private Texture GetGroupIcon(IssueGroup group)
		{
			switch (group)
			{
				case IssueGroup.Asset:
					return CSIcons.Reveal;
				case IssueGroup.GameObject:
					return CSEditorIcons.GameObject;
				case IssueGroup.Component:
					return CSEditorIcons.Script;
				case IssueGroup.ProjectSettings:
					return CSEditorIcons.Settings;
				default:
					return null;
			}
		}
	}
}