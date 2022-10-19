#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using System.Globalization;

	using UnityEditor;
	using UnityEngine;

	using Core;

	internal static class CSEditorTools
	{
		private static readonly string[] sizes = { "B", "KB", "MB", "GB" };
		private static TextInfo textInfo;

		internal static CSSceneTools.OpenSceneResult lastRevealSceneOpenResult;

		public static string FormatBytes(double bytes)
		{
			var order = 0;

			while (bytes >= 1024 && order + 1 < 4)
			{
				order++;
				bytes = bytes / 1024;
			}

			return string.Format("{0:0.##} {1}", bytes, sizes[order]);
		}
		
		public static string GetFullTransformPath(Transform transform, Transform stopAt = null)
		{
			var path = transform.name;
			while (transform.parent != null)
			{
				transform = transform.parent;
				if (transform == stopAt) break;
				path = transform.name + "/" + path;
			}
			return path;
		}

		public static EditorWindow GetInspectorWindow()
		{
			if (CSReflectionTools.inspectorWindowType == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't find UnityEditor.InspectorWindow type!"));
				return null;
			}

			var inspectorWindow = EditorWindow.GetWindow(CSReflectionTools.inspectorWindowType);
			if (inspectorWindow == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't get an InspectorWindow!"));
				return null;
			}

			return inspectorWindow;
		}

		public static ActiveEditorTracker GetActiveEditorTrackerForSelectedObject()
		{
			var inspectorWindow = GetInspectorWindow();
			if (inspectorWindow == null) return null;
			if (CSReflectionTools.inspectorWindowType == null) return null;

			inspectorWindow.Repaint();

			ActiveEditorTracker result = null;

			var trackerProperty = CSReflectionTools.GetPropertyInfo(CSReflectionTools.inspectorWindowType, "tracker");
			if (trackerProperty != null)
			{
				result = (ActiveEditorTracker)trackerProperty.GetValue(inspectorWindow, null);
			}
			else
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't get ActiveEditorTracker from the InspectorWindow!"));
			}

			return result;
		}

		public static bool RevealInSettings(AssetSettingsKind settingsKind, string path = null)
		{
			var result = true;

			switch (settingsKind)
			{
				case AssetSettingsKind.AudioManager:
				case AssetSettingsKind.ClusterInputManager:
				case AssetSettingsKind.InputManager:
				case AssetSettingsKind.NavMeshAreas:
				case AssetSettingsKind.NavMeshLayers:
				case AssetSettingsKind.NavMeshProjectSettings:
				case AssetSettingsKind.NetworkManager:
					break;

				case AssetSettingsKind.Undefined:
					Debug.LogWarning(Maintainer.ConstructLog("Can't open settings of kind Undefined Oo"));
					result = false;
					break;
				case AssetSettingsKind.DynamicsManager:
					if (!CSMenuTools.ShowProjectSettingsPhysics())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Physics Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.EditorBuildSettings:
					if (!CSMenuTools.ShowEditorBuildSettings())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open EditorBuildSettings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.EditorSettings:
					if (!CSMenuTools.ShowEditorSettings())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Editor Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.GraphicsSettings:
					if (!CSMenuTools.ShowProjectSettingsGraphics())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open GraphicsSettings!"));
						result = false;
					}
					break;

				case AssetSettingsKind.Physics2DSettings:
					if (!CSMenuTools.ShowProjectSettingsPhysics2D())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Physics2D Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.ProjectSettings:
					if (!CSMenuTools.ShowProjectSettingsPlayer())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Player Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.PresetManager:
					if (!CSMenuTools.ShowProjectSettingsPresetManager())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Preset Manager!"));
						result = false;
					}
					break;
				case AssetSettingsKind.QualitySettings:
					break;
				case AssetSettingsKind.TagManager:
					if (!CSMenuTools.ShowProjectSettingsTagsAndLayers())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Tags and Layers Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.TimeManager:
					break;
				case AssetSettingsKind.UnityAdsSettings:
					break;
				case AssetSettingsKind.UnityConnectSettings:
					break;
				case AssetSettingsKind.VFXManager:
					if (!CSMenuTools.ShowProjectSettingsVFX())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open VFX Settings!"));
						result = false;
					}
					break;
				case AssetSettingsKind.UnknownSettingAsset:
					if (!string.IsNullOrEmpty(path)) EditorUtility.RevealInFinder(path);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}

		public static string NicifyName(string name)
		{
			var nicePropertyName = ObjectNames.NicifyVariableName(name);
			if (textInfo == null) textInfo = new CultureInfo("en-US", false).TextInfo;
			return textInfo.ToTitleCase(nicePropertyName);
		}
	}
}