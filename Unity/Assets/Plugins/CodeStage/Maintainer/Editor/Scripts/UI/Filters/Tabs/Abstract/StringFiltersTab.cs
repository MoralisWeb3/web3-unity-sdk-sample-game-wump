#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI.Filters
{
	using System;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	using Core;
	using EditorCommon.Tools;
	using Tools;

	internal abstract class StringFiltersTab : TabBase
	{
		private const string AddFilterTextControl = "AddFilterText";

		internal delegate void SaveFiltersCallback(FilterItem[] filters);
		internal delegate FilterItem[] GetDefaultsCallback();

		protected FilterItem[] filters;
		protected SaveFiltersCallback saveFiltersCallback;

		protected bool focusAddFilterValueText;
		protected int focusEditFilterValueTextIndex = -1;
		
		protected string customCaption;

		private readonly GetDefaultsCallback defaultsCallback;

		private string newItemText = "";
		private bool newItemEnabled = true;
		private FilterKind newItemKind = FilterKind.Path;
		private bool newItemIgnoreCase;
		private string tabName;

		protected StringFiltersTab(FilterType filterType, FilterItem[] filters, SaveFiltersCallback saveFiltersCallback, GetDefaultsCallback defaultsCallback = null) :base(filterType)
		{
			this.filters = filters;
			this.saveFiltersCallback = saveFiltersCallback;
			this.defaultsCallback = defaultsCallback;
		}

		internal override void Show(FiltersWindow hostingWindow)
		{
			base.Show(hostingWindow);

			newItemText = "";
			focusAddFilterValueText = true;
		}

		internal override void UpdateCaption()
		{
			if (string.IsNullOrEmpty(tabName))
				tabName = string.IsNullOrEmpty(customCaption) ? caption.text : customCaption;
			
			var label = tabName + CSColorTools.WrapString(" " + filterType, filterType == FilterType.Includes);

			if (filters.Length > 0)
			{
				label += " <b>(" + filters.Length + ")</b>";
			}

			caption.text = label;
		}

		protected override void DrawTabContents()
		{
			DrawTabHeader();

			GUILayout.Space(5);
			UIHelpers.Separator();
			GUILayout.Space(5);

			DrawAddItemSection();

			GUILayout.Space(5);
			UIHelpers.Separator();
			GUILayout.Space(5);

			DrawFiltersList();
		}

		protected virtual void DrawAddItemSection()
		{
			EditorGUILayout.LabelField(GetAddNewLabel());
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(6);
				GUI.SetNextControlName("AddButton");

				var enterPressedWhileEditingNewFilterValue = currentEvent.isKey && 
				           Event.current.type == EventType.KeyDown && 
				           GUI.GetNameOfFocusedControl() == AddFilterTextControl &&
				           (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);

				if (UIHelpers.IconButton(CSIcons.Plus, "Adds custom filter to the list.") || enterPressedWhileEditingNewFilterValue)
				{
					if (string.IsNullOrEmpty(newItemText))
					{
						window.ShowNotification(new GUIContent("You can't add an empty filter!"));
					}
					else if (newItemText.IndexOf('*') != -1)
					{
						window.ShowNotification(new GUIContent("Masks are not supported!"));
					}
					else
					{
						if (newItemKind == FilterKind.Extension && !newItemText.StartsWith("."))
						{
							newItemText = "." + newItemText;
						}

						if (CheckNewItem(ref newItemText))
						{
							if (CSFilterTools.TryAddNewItemToFilters(ref filters, FilterItem.Create(newItemEnabled, newItemText, newItemKind, newItemIgnoreCase)))
							{
								SaveChanges();
								newItemText = "";
								GUI.FocusControl("AddButton");
								focusAddFilterValueText = true;
							}
							else
							{
								window.ShowNotification(new GUIContent("This filter already exists in the list!"));
							}
						}
					}
				}
				
				if (enterPressedWhileEditingNewFilterValue)
				{
					currentEvent.Use();
					currentEvent.Use();
				}
				
				newItemEnabled = EditorGUILayout.Toggle(GUIContent.none, newItemEnabled, GUILayout.Width(14));
				newItemKind = DrawFilterKindDropdown(newItemKind);
				newItemIgnoreCase = DrawFilterIgnoreCaseToggle(newItemIgnoreCase);
				GUILayout.Space(5);

				GUI.SetNextControlName(AddFilterTextControl);
				newItemText = EditorGUILayout.TextField(newItemText);
				if (focusAddFilterValueText)
				{
					EditorGUI.FocusTextInControl(AddFilterTextControl);
					focusAddFilterValueText = false;
				}
			}
		}

		protected virtual void DrawFiltersList()
		{
			if (filters == null) return;

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			for (var i = 0; i < filters.Length; i++)
			{
				var filter = filters[i];
				using (new GUILayout.HorizontalScope(UIHelpers.panelWithBackground))
				{
					if (UIHelpers.IconButton(CSIcons.Minus, "Removes filter from the list."))
					{
						ArrayUtility.Remove(ref filters, filter);
						SaveChanges();
					}

					using (var scope = new EditorGUI.ChangeCheckScope())
					{
						filter.enabled = EditorGUILayout.Toggle(GUIContent.none, filter.enabled, GUILayout.Width(14));

						if (!filter.enabled)
						{
							GUI.enabled = false;
						}

						filter.kind = DrawFilterKindDropdown(filter.kind);
						filter.ignoreCase = DrawFilterIgnoreCaseToggle(filter.ignoreCase);

						if (scope.changed)
						{
							SaveChanges();
						}
					}
					
					if (filter.isEditingValue)
					{
						var enterPressedWhileEditingFilterValue = IsKeyPressedWhileInControl(KeyCode.Return, "FilterEdit" + i);
						var escPressedWhileEditingFilterValue = IsKeyPressedWhileInControl(KeyCode.Escape, "FilterEdit" + i);

						if (escPressedWhileEditingFilterValue)
						{
							focusAddFilterValueText = true;
							filter.isEditingValue = false;
						}
						
						if (UIHelpers.IconButton(CSIcons.Check, "Apply new value") || enterPressedWhileEditingFilterValue)
						{
							focusAddFilterValueText = true;
							filter.isEditingValue = false;
							SaveChanges();
						}

						if (enterPressedWhileEditingFilterValue)
						{
							currentEvent.Use();
							currentEvent.Use();
						}

						GUI.SetNextControlName("FilterEdit" + i);
						filter.value = EditorGUILayout.TextField(filter.value);
						
						if (focusEditFilterValueTextIndex > -1)
						{
							EditorGUI.FocusTextInControl("FilterEdit" + focusEditFilterValueTextIndex);
							focusEditFilterValueTextIndex = -1;
						}
					}
					else
					{
						if (UIHelpers.IconButton(CSIcons.Edit, "Edit filter value") || 
						    GUILayout.Button(filter.value, UIHelpers.richLabel))
						{
							focusEditFilterValueTextIndex = i;
							filter.isEditingValue = true;
						}
					}

					GUI.enabled = true;
				}
			}

			GUILayout.EndScrollView();

			if (filters.Length > 0)
			{
				if (UIHelpers.ImageButton("Clear All " + caption.text, "Removes all added filters from the list.", CSIcons.Clear))
				{
					var cleanCaption = Regex.Replace(caption.text, @"<[^>]*>", string.Empty);
					if (EditorUtility.DisplayDialog("Clearing the " + cleanCaption + " list",
						"Are you sure you wish to clear all the filters in the " + cleanCaption + " list?",
						"Yes", "No"))
					{
						Array.Resize(ref filters, 0);
						SaveChanges();
					}
				}
			}

			if (defaultsCallback != null)
			{
				if (UIHelpers.ImageButton("Reset to Defaults", CSIcons.Restore))
				{
					filters = defaultsCallback();
					SaveChanges();
				}
			}
		}

		protected void SaveChanges()
		{
			if (saveFiltersCallback != null)
			{
				saveFiltersCallback(filters);
			}

			window.UpdateTabCaptions();
		}

		protected virtual string GetAddNewLabel()
		{
			return "Add new filter to the list:";
		}

		protected virtual FilterKind DrawFilterKindDropdown(FilterKind kind)
		{
			return kind;
		}

		protected virtual bool DrawFilterIgnoreCaseToggle(bool ignore)
		{
			return ignore;
		}

		protected abstract void DrawTabHeader();

		protected abstract bool CheckNewItem(ref string newItem);
		
		private bool IsKeyPressedWhileInControl(KeyCode keyCode, string controlId)
		{
			return currentEvent.isKey && Event.current.type == EventType.KeyDown &&
			       GUI.GetNameOfFocusedControl() == controlId && currentEvent.keyCode == keyCode;
		}
	}
}