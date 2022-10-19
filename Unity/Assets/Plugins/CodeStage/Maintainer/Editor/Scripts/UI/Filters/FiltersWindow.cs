#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI.Filters
{
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEngine;
	using Settings;

	internal abstract class FiltersWindow : EditorWindow
	{
		internal delegate void TabChangeCallback(int newTab);

		private static bool needToRepaint;

		private event TabChangeCallback TabChangedCallback;

		private TabBase[] tabs;
		private GUIContent[] tabsCaptions;
        private TabBase currentTab;
        private int currentTabIndex;

		private Event currentEvent;
		private EventType currentEventType;

		protected void Init(string caption, TabBase[] windowTabs, int initialTab, TabChangeCallback tabChangeCallback)
		{
			titleContent = new GUIContent(caption + " Filters");

			minSize = new Vector2(650f, 300f);

			TabChangedCallback = tabChangeCallback;

			if (windowTabs != null && windowTabs.Length > 0)
			{
				tabs = windowTabs;

				currentTabIndex = tabs.Length > initialTab ? initialTab : 0;

				currentTab = tabs[currentTabIndex];
				currentTab.Show(this);

				tabsCaptions = new GUIContent[tabs.Length];
				UpdateTabCaptions();
			}
			else
			{
				Debug.LogError(Maintainer.ConstructLog("No tabs were passed to the Filters Window!"));
			}
		}

		internal void UpdateTabCaptions()
		{
			for (var i = 0; i < tabs.Length; i++)
			{
				tabs[i].UpdateCaption();
				tabsCaptions[i] = tabs[i].caption;
			}
		}

		protected abstract void InitOnEnable();
		protected abstract void UnInitOnDisable();

		protected virtual void OnGUI()
		{
			UIHelpers.SetupStyles();

			currentEvent = Event.current;
			currentEventType = currentEvent.type;

			EditorGUI.BeginChangeCheck();
			{
				EditorGUI.BeginChangeCheck();
				currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabsCaptions, UIHelpers.richButton, GUILayout.Height(21));
				if (EditorGUI.EndChangeCheck())
				{
					RemoveNotification();
				}
				currentTab = tabs[currentTabIndex];
			}
			if (EditorGUI.EndChangeCheck())
			{
				currentTab.Show(this);
				if (TabChangedCallback != null)
				{
					TabChangedCallback.Invoke(currentTabIndex);
				}
			}

			currentTab.currentEvent = currentEvent;
			currentTab.currentEventType = currentEventType;

			currentTab.ProcessDrags();
			currentTab.Draw();
		}

		[DidReloadScripts]
		private static void OnScriptsRecompiled()
		{
			needToRepaint = true;
		}

		private void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;
			InitOnEnable();
		}

		private void OnDisable()
		{
			ProjectSettings.Save();
			UnInitOnDisable();
		}

		private void OnInspectorUpdate()
		{
			if (needToRepaint)
			{
				needToRepaint = false;
				Repaint();
			}
		}
	}
}