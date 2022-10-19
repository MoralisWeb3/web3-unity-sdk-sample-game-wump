#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI.Filters
{
	using UnityEditor;
	using UnityEngine;
	using Core;
	using EditorCommon.Tools;
	using Tools;

	internal class PathFiltersTab : StringFiltersTab
	{
		private readonly bool showNotice;
		private readonly string headerExtra;

		internal PathFiltersTab(FilterType filterType, string headerExtra, FilterItem[] filtersList, bool showNotice, SaveFiltersCallback saveCallback, GetDefaultsCallback defaultsCallback = null) : base(filterType, filtersList, saveCallback, defaultsCallback)
		{
			this.headerExtra = headerExtra;
			this.showNotice = showNotice;
			
			caption = new GUIContent("Path", CSEditorIcons.Folder);
		}

		internal override void ProcessDrags()
		{
			if (currentEventType != EventType.DragUpdated && currentEventType != EventType.DragPerform) return;

			var paths = DragAndDrop.paths;
			
			if (paths != null && paths.Length > 0)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (currentEventType == EventType.DragPerform)
				{
					var needToSave = false;
					var needToShowWarning = false;

					foreach (var path in paths)
					{
						var added = CSFilterTools.TryAddNewItemToFilters(ref filters, FilterItem.Create(path, FilterKind.Path));
						needToSave |= added;
						needToShowWarning |= !added;
					}

					if (needToSave)
					{
						SaveChanges();
					}

					if (needToShowWarning)
					{
						window.ShowNotification(new GUIContent("One or more of the dragged items already present in the list!"));
					}

					DragAndDrop.AcceptDrag();
				}
			}
			Event.current.Use();
		}

		protected override void DrawTabHeader()
		{
			EditorGUILayout.LabelField("Here you may specify full or partial paths to <b>" + CSColorTools.WrapString("include", "ignore", filterType == FilterType.Includes) + "</b>.\n" +
										"You may drag & drop files and folders to this window directly from the Project Browser.",
										UIHelpers.richWordWrapLabel);

			EditorGUILayout.LabelField("Only Extension filter type matches whole word. Extension filter value should start with dot (.dll).", UIHelpers.richWordWrapLabel);

			if (!string.IsNullOrEmpty(headerExtra))
			{
				EditorGUILayout.LabelField(headerExtra, EditorStyles.wordWrappedLabel);
			}

			if (showNotice)
			{
				EditorGUILayout.Space();
				var oldRich = EditorStyles.helpBox.richText;
				var oldSize = EditorStyles.helpBox.fontSize;

				EditorStyles.helpBox.richText = true;
				EditorStyles.helpBox.fontSize = 12;
				EditorGUILayout.HelpBox("<b>Note:</b> If you have both Includes and Ignores added, first Includes are applied, then Ignores are applied to the included paths.", MessageType.Info, true);

				EditorStyles.helpBox.richText = oldRich;
				EditorStyles.helpBox.fontSize = oldSize;
			}
		}

		protected override bool CheckNewItem(ref string newItem)
		{
			newItem = CSPathTools.EnforceSlashes(newItem);
			return true;
		}

		protected override FilterKind DrawFilterKindDropdown(FilterKind kind)
		{
			var enumNames = new [] { FilterKind.Path.ToString(), FilterKind.Directory.ToString(), FilterKind.FileName.ToString(), FilterKind.Extension.ToString() };
			return (FilterKind) EditorGUILayout.Popup((int) kind, enumNames, GUILayout.Width(80));
		}
		
		protected override bool DrawFilterIgnoreCaseToggle(bool ignore)
		{
			return GUILayout.Toggle(ignore, "Ignore Case", GUILayout.Width(90));
		}
	}
}
