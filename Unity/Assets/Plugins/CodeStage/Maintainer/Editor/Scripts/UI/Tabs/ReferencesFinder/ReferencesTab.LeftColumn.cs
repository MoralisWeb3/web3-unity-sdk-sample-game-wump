#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System;
	using EditorCommon.Tools;
	using References;
	using Settings;
	using UnityEditor;
	using UnityEngine;

	internal partial class ReferencesTab
	{
		protected override void DrawLeftColumnHeader()
		{
			using (new GUILayout.VerticalScope())
			{
				GUILayout.Space(10);

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					GUILayout.Label("<size=16><b>Search scope</b></size>", UIHelpers.richLabel);
					GUILayout.FlexibleSpace();

					using (new GUILayout.VerticalScope())
					{	
						GUILayout.Space(6);
						if (GUILayout.Button(CSEditorIcons.Help, UIHelpers.BuiltinIconButtonStyle))
						{
							// TODO: update
							EditorUtility.DisplayDialog(ReferencesFinder.ModuleName + " scopes help",
								"Use " + projectTab.Caption.text + " scope to figure out where any specific asset is referenced in whole project.\n\n" +
								"Use " + hierarchyTab.Caption.text + " scope to figure out where any specific Game Object or component is referenced in active scene or opened prefab.",
								"OK");
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

				GUILayout.Space(10);

				EditorGUI.BeginChangeCheck();
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(10);
					currentTab = (ReferenceFinderTab)GUILayout.SelectionGrid((int)currentTab, tabsCaptions, 1,
						GUILayout.Height(56), GUILayout.ExpandWidth(true));
					GUILayout.Space(10);
				}

				if (EditorGUI.EndChangeCheck())
				{
					UserSettings.Instance.referencesFinder.selectedTab = currentTab;
					Refresh(false);
				}

				switch (currentTab)
				{
					case ReferenceFinderTab.Project:
						projectTab.DrawLeftColumnHeader();
						break;
					case ReferenceFinderTab.Scene:
						hierarchyTab.DrawLeftColumnHeader();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			base.DrawLeftColumnHeader();
		}

		protected override void DrawLeftColumnBody()
		{
			using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				switch (currentTab)
				{
					case ReferenceFinderTab.Project:
						projectTab.DrawSettings();
						break;
					case ReferenceFinderTab.Scene:
						hierarchyTab.DrawSettings();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}