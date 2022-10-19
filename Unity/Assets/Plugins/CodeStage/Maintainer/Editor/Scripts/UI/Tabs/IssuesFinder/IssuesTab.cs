#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System;
	using System.IO;
	using System.Linq;
	using Cleaner;
	using Core;
	using Core.Scan;
	using EditorCommon.Tools;
	using Issues;
	using Settings;
	using Tools;
	using Filters;

	using UnityEditor;
	using UnityEngine;

	internal partial class IssuesTab : RecordsTab<IssueRecord>
	{
		protected override string CaptionName
		{
			get { return IssuesFinder.ModuleName; }
		}

		protected override Texture CaptionIcon
		{
			get { return CSIcons.Issue; }
		}

		public IssuesTab(MaintainerWindow maintainerWindow) : base(maintainerWindow)
		{
		}

		protected override IssueRecord[] LoadLastRecords()
		{
			var loadedRecords = SearchResultsStorage.IssuesSearchResults;

			if (loadedRecords == null)
			{
				loadedRecords = new IssueRecord[0];
			}

			return loadedRecords;
		}

		protected override RecordsTabState GetState()
		{
			return UserSettings.Issues.tabState;
		}

		protected override void ApplySorting()
		{
			base.ApplySorting();

			switch (UserSettings.Issues.sortingType)
			{
				case IssuesSortingType.Unsorted:
					break;
				case IssuesSortingType.ByIssueType:
					filteredRecords = UserSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderBy(RecordsSortings.issueRecordByType).ThenBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordByType).ThenBy(RecordsSortings.issueRecordByPath).ToArray();
					break;
				case IssuesSortingType.BySeverity:
					filteredRecords = UserSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordBySeverity).ThenBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderBy(RecordsSortings.issueRecordBySeverity).ThenBy(RecordsSortings.issueRecordByPath).ToArray();
					break;
				case IssuesSortingType.ByPath:
					filteredRecords = UserSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordByPath).ToArray();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void SaveSearchResults()
		{
			SearchResultsStorage.IssuesSearchResults = GetRecords();
		}

		protected override string GetModuleName()
		{
			return IssuesFinder.ModuleName;
		}

		protected override void DrawPagesRightHeader()
		{
			base.DrawPagesRightHeader();

			GUILayout.Label("Sorting:", GUILayout.ExpandWidth(false));

			EditorGUI.BeginChangeCheck();
			UserSettings.Issues.sortingType = (IssuesSortingType)EditorGUILayout.EnumPopup(UserSettings.Issues.sortingType, GUILayout.Width(100));
			if (EditorGUI.EndChangeCheck())
			{
				ApplySorting();
			}

			EditorGUI.BeginChangeCheck();
			UserSettings.Issues.sortingDirection = (SortingDirection)EditorGUILayout.EnumPopup(UserSettings.Issues.sortingDirection, GUILayout.Width(80));
			if (EditorGUI.EndChangeCheck())
			{
				ApplySorting();
			}
		}

		protected override void DrawRecord(IssueRecord record, int recordIndex)
		{
			// hide fixed records
			if (record.fixResult != null && record.fixResult.Success) return;

			using (new GUILayout.VerticalScope())
			{
				if (recordIndex > 0 && recordIndex < filteredRecords.Length) UIHelpers.Separator();

				using (new GUILayout.HorizontalScope())
				{
					DrawRecordCheckbox(record);
					DrawExpandCollapseButton(record);
					DrawSeverityIcon(record.Severity);

					if (record.compactMode)
					{
						DrawRecordButtons(record, recordIndex);
						GUILayout.Label(record.GetCompactLine(), UIHelpers.richLabel);
					}
					else
					{
						GUILayout.Space(5);
						GUILayout.Label(record.GetHeader(), UIHelpers.richLabel);
					}

					if (record.LocationGroup == LocationGroup.PrefabAsset)
					{
						GUILayout.Space(3);
						UIHelpers.Icon(CSEditorIcons.Prefab, "Issue found in the Prefab.");
					}
				}

				if (!record.compactMode)
				{
					UIHelpers.Separator();
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Space(5);
						GUILayout.Label(record.GetBody(), UIHelpers.richLabel);
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Space(5);
						DrawRecordButtons(record, recordIndex);
					}
					GUILayout.Space(3);
				}
			}
		}

		protected override string GetReportFileNamePart()
		{
			return "Issues";
		}

		protected override void AfterClearRecords()
		{
			SearchResultsStorage.IssuesSearchResults = null;
		}

		private void StartSearch()
		{
			window.RemoveNotification();
			IssuesFinder.StartSearch(true);
			window.Focus();
		}

		private void StartFix()
		{
			window.RemoveNotification();
			IssuesFinder.StartFix();
			window.Focus();
		}

		private void DrawRecordButtons(IssueRecord record, int recordIndex)
		{
			DrawShowButtonIfPossible(record);
			DrawFixButton(record, recordIndex);

			if (!record.compactMode)
			{
				DrawCopyButton(record);
				DrawHideButton(record, recordIndex);
			}

			var objectIssue = record as GameObjectIssueRecord;
			if (objectIssue != null)
			{
				DrawMoreButton(objectIssue);
			}

#if UNITY_2019_1_OR_NEWER
			var shaderIssue = record as ShaderIssueRecord;
			if (shaderIssue != null)
			{
				DrawMoreButton(shaderIssue);
			}
#endif
		}

		private void DrawFixButton(IssueRecord record, int recordIndex)
		{
			GUI.enabled = record.IsFixable;

			var label = "Fix";
			var hint = "Automatically fixes issue (not available for this issue yet).";

			if (record.Kind == IssueKind.MissingComponent)
			{
				label = "Remove";
				hint = "Removes missing component.";
			}
			else if (record.Kind == IssueKind.MissingReference)
			{
				label = "Reset";
				hint = "Resets missing reference to default None value.";
			}

			if (UIHelpers.RecordButton(record, label, hint, CSIcons.AutoFix))
			{
				var fixResult = record.Fix(false);
				if (fixResult != null && fixResult.Success)
				{
					DeleteRecords(new[] { recordIndex });

					var notificationExtra = "";

					if (record.LocationGroup == LocationGroup.PrefabAsset || record.LocationGroup == LocationGroup.Asset)
					{
						AssetDatabase.SaveAssets();
					}

					MaintainerWindow.ShowNotification("Issue successfully fixed!" + notificationExtra);
				}
				else
				{
					var notificationText = "Could not fix the issue!";
					if (fixResult != null && !string.IsNullOrEmpty(fixResult.ErrorText))
					{
						notificationText = fixResult.ErrorText;
					}
					MaintainerWindow.ShowNotification(notificationText);
				}
			}

			GUI.enabled = true;
		}

		private void DrawHideButton(IssueRecord record, int recordIndex)
		{
			if (UIHelpers.RecordButton(record, "Hide", "Hides this issue from the results list.\nUseful when you fixed issue and wish to hide it away.", CSIcons.Hide))
			{
				DeleteRecords(new []{ recordIndex });
			}
		}

		private void DrawMoreButton(AssetIssueRecord record)
		{
			var menu = new GenericMenu();
			if (!string.IsNullOrEmpty(record.Path) && record.Path != CSPathTools.UntitledScenePath)
			{
				menu.AddItem(new GUIContent("Ignore/Full Path"), false, () =>
				{
					if (!CSFilterTools.IsValueMatchesAnyFilter(record.Path, ProjectSettings.Issues.pathIgnoresFilters))
					{
						var newFilter = FilterItem.Create(record.Path, FilterKind.Path);
						ArrayUtility.Add(ref ProjectSettings.Issues.pathIgnoresFilters, newFilter);

						ApplyNewIgnoreFilter(newFilter);

						MaintainerWindow.ShowNotification("Ignore added: " + record.Path);
						IssuesFiltersWindow.Refresh();
					}
					else
					{
						MaintainerWindow.ShowNotification("Already added to the ignores!");
					}
				});

				var dir = Directory.GetParent(record.Path);
				if (!CSPathTools.IsAssetsRootPath(dir.FullName))
				{
					menu.AddItem(new GUIContent("Ignore/Parent Folder"), false, () =>
					{
						var dirPath = CSPathTools.EnforceSlashes(dir.ToString());

						if (!CSFilterTools.IsValueMatchesAnyFilter(dirPath, ProjectSettings.Issues.pathIgnoresFilters))
						{
							var newFilter = FilterItem.Create(dirPath, FilterKind.Directory);
							ArrayUtility.Add(ref ProjectSettings.Issues.pathIgnoresFilters, newFilter);

							ApplyNewIgnoreFilter(newFilter);

							MaintainerWindow.ShowNotification("Ignore added: " + dirPath);
							CleanerFiltersWindow.Refresh();
						}
						else
						{
							MaintainerWindow.ShowNotification("Already added to the ignores!");
						}
					});
				}
			}

			var objectIssue = record as GameObjectIssueRecord;
			if (objectIssue != null)
			{
				if (!string.IsNullOrEmpty(objectIssue.componentName))
				{
					menu.AddItem(new GUIContent("Ignore/\"" + objectIssue.componentName + "\" Component" ), false, () =>
					{
						if (!CSFilterTools.IsValueMatchesAnyFilter(objectIssue.componentName, ProjectSettings.Issues.componentIgnoresFilters))
						{
							var newFilter = FilterItem.Create(objectIssue.componentName, FilterKind.Type);
							ArrayUtility.Add(ref ProjectSettings.Issues.componentIgnoresFilters, newFilter);

							ApplyNewIgnoreFilter(newFilter);

							MaintainerWindow.ShowNotification("Ignore added: " + objectIssue.componentName);
							CleanerFiltersWindow.Refresh();
						}
						else
						{
							MaintainerWindow.ShowNotification("Already added to the ignores!");
						}
					});
				}
			}
			
			if (menu.GetItemCount() == 0)
				return;
			
			if (!UIHelpers.RecordButton(record, "Shows menu with additional actions for this record.", CSIcons.More))
				return;

			menu.ShowAsContext();
		}

		private void DrawSeverityIcon(IssueSeverity severity)
		{
			DrawSeverityIcon(severity, GUI.color);
		}

		private void DrawSeverityIcon(IssueSeverity severity, Color color)
		{
			var icon = GetSeverityIcon(severity);
			var iconArea = EditorGUILayout.GetControlRect(false, 16, GUILayout.Width(16));
			var iconRect = new Rect(iconArea);

			GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleAndCrop, true, 0, color, 0, 0);
		}

		private Texture GetSeverityIcon(IssueSeverity severity)
		{
			Texture result;
			switch (severity)
			{
#if UNITY_2019_3_OR_NEWER
				case IssueSeverity.Error:
					result = CSEditorIcons.Error;
					break;
				case IssueSeverity.Warning:
					result = CSEditorIcons.Warn;
					break;
				case IssueSeverity.Info:
					result = CSEditorIcons.Info;
					break;
				default:
					throw new ArgumentOutOfRangeException();
#else
				case IssueSeverity.Error:
					result = CSEditorIcons.ErrorSmall;
					break;
				case IssueSeverity.Warning:
					result = CSEditorIcons.WarnSmall;
					break;
				case IssueSeverity.Info:
					result = CSEditorIcons.InfoSmall;
					break;
				default:
					throw new ArgumentOutOfRangeException();
#endif
			}

			return result;
		}
	}

	internal enum SettingsSearchSection : byte
	{
		All,
		Common,
		Neatness,
	}
}