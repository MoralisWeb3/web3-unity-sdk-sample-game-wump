#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using Core;
	using System;
	using UI;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
#if UNITY_2021_2_OR_NEWER
	using UnityEditor.SceneManagement;
#else
	using UnityEditor.Experimental.SceneManagement;
#endif

	internal static class CSSelectionTools
	{
		public static bool RevealAndSelectFileAsset(string assetPath)
		{
			var instanceId = CSAssetTools.GetMainAssetInstanceID(assetPath);
			if (AssetDatabase.Contains(instanceId))
			{
				Selection.activeObject = null;
				EditorApplication.delayCall += () =>
				{
					Selection.activeInstanceID = instanceId;
				};
				
				return true;
			}

			return false;
		}

		public static bool RevealAndSelectSubAsset(string assetPath, string name, long objectId)
		{
			var targetAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
			if (targetAssets == null || targetAssets.Length == 0) return false;

			foreach (var targetAsset in targetAssets)
			{
				if (!AssetDatabase.IsSubAsset(targetAsset)) continue;
				if (targetAsset is GameObject || targetAsset is Component) continue;
				if (!string.Equals(targetAsset.name, name, StringComparison.OrdinalIgnoreCase)) continue;

				var assetId = CSObjectTools.GetUniqueObjectId(targetAsset);
				if (assetId != objectId) continue;

				Selection.activeInstanceID = targetAsset.GetInstanceID();
				return true;
			}

			return false;
		}

		public static void RevealAndSelectReferencingEntry(string assetPath, ReferencingEntryData referencingEntry)
		{
			if (!string.IsNullOrEmpty(assetPath) && 
			    (referencingEntry.location == Location.SceneLightingSettings ||
			    referencingEntry.location == Location.SceneNavigationSettings))
			{
				var sceneOpenResult = CSSceneTools.OpenSceneWithSavePrompt(assetPath);
				if (!sceneOpenResult.success)
				{
					Debug.LogError(Maintainer.ErrorForSupport("Can't open scene " + assetPath));
					MaintainerWindow.ShowNotification("Can't show it properly");
					return;
				}
			}

			switch (referencingEntry.location)
			{
				case Location.ScriptAsset:
				case Location.UnityObjectAsset:

					if (!RevealAndSelectFileAsset(assetPath))
					{
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					break;
				case Location.PrefabAssetObject:
					if (!RevealAndSelectSubAsset(assetPath, referencingEntry.transformPath,
						referencingEntry.objectId))
					{
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					break;
				case Location.PrefabAssetGameObject:
				case Location.SceneGameObject:

					if (!RevealAndSelectGameObject(assetPath, referencingEntry.transformPath,
						referencingEntry.objectId, referencingEntry.componentId))
					{
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					break;

				case Location.SceneLightingSettings:

					if (!CSMenuTools.ShowSceneSettingsLighting())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Lighting settings!"));
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					break;

				case Location.SceneNavigationSettings:

					if (!CSMenuTools.ShowSceneSettingsNavigation())
					{
						Debug.LogError(Maintainer.ErrorForSupport("Can't open Navigation settings!"));
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					break;

				case Location.NotFound:
				case Location.Invisible:
					break;

				case Location.TileMap:

					if (!RevealAndSelectGameObject(assetPath, referencingEntry.transformPath,
						referencingEntry.objectId, referencingEntry.componentId))
					{
						MaintainerWindow.ShowNotification("Can't show it properly");
					}

					// TODO: open tile map editor window?

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static bool RevealAndSelectGameObject(string assetPath, string transformPath, long objectId, long componentId)
		{
			var enclosingAssetType =  AssetDatabase.GetMainAssetTypeAtPath(assetPath);
			if (enclosingAssetType == CSReflectionTools.sceneAssetType 
				|| string.IsNullOrEmpty(assetPath) 
				|| assetPath == CSPathTools.UntitledScenePath)
			{
				return RevealAndSelectGameObjectInScene(assetPath, transformPath, objectId, componentId);
			}

			return RevealAndSelectGameObjectInPrefab(assetPath, transformPath, objectId, componentId);
		}

		public static CSSceneTools.OpenSceneResult OpenSceneForReveal(string path)
		{
			var result = CSSceneTools.OpenScene(path);
			if (result.success)
			{
				CSSceneTools.CloseUntitledSceneIfNotDirty();

				if (CSEditorTools.lastRevealSceneOpenResult != null)
				{
					if (CSSceneTools.IsOpenedSceneNeedsToBeClosed(CSEditorTools.lastRevealSceneOpenResult, path, true))
					{
						if (CSEditorTools.lastRevealSceneOpenResult.scene.isDirty)
						{
							if (!CSSceneTools.SaveDirtyScenesWithPrompt(
								new[] { CSEditorTools.lastRevealSceneOpenResult.scene }))
							{
								return new CSSceneTools.OpenSceneResult();
							}
						}
					}

					CSSceneTools.CloseOpenedSceneIfNeeded(CSEditorTools.lastRevealSceneOpenResult, path, true);
				}

				CSEditorTools.lastRevealSceneOpenResult = result;
			}

			return result;
		}

		public static bool TryFoldAllComponentsExceptId(long componentId)
		{
			var tracker = CSEditorTools.GetActiveEditorTrackerForSelectedObject();
			if (tracker == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't get active tracker."));
				return false;
			}

			tracker.RebuildIfNecessary();

			var editors = tracker.activeEditors;
			if (editors.Length > 1)
			{
				var targetFound = false;
				var skipCount = 0;

				for (var i = 0; i < editors.Length; i++)
				{
					var editor = editors[i];
					var editorTargetType = editor.target.GetType();
					if (editorTargetType == CSReflectionTools.assetImporterType ||
					    editorTargetType == CSReflectionTools.gameObjectType)
					{
						skipCount++;
						continue;
					}

					if (i - skipCount == componentId)
					{
						targetFound = true;

						/* known corner cases when editor can't be set to visible via tracker */

						if (editor.serializedObject.targetObject is ParticleSystemRenderer)
						{
							var renderer = (ParticleSystemRenderer)editor.serializedObject.targetObject;
							var ps = renderer.GetComponent<ParticleSystem>();
							componentId = CSComponentTools.GetComponentIndex(ps);
						}

						break;
					}
				}

				if (componentId != -1 && !targetFound)
				{
					return false;
				}

				SwitchTrackersVisibility(tracker, editors, componentId, skipCount);

				// workaround for bug when tracker selection gets reset after scene open
				// (e.g. revealing TMP component in new scene)
				EditorApplication.delayCall += () =>
				{
					EditorApplication.delayCall += () =>
					{
						try
						{
							SwitchTrackersVisibility(tracker, editors, componentId, skipCount);
						}
						catch (Exception)
						{
							// ignored
						}
					};
				};
			}

			return true;
		}

		private static void SwitchTrackersVisibility(ActiveEditorTracker tracker, Editor[] editors, long componentId, int skipCount)
		{
			for (var i = 1; i < editors.Length; i++)
			{
				if (componentId != -1)
					tracker.SetVisible(i, i - skipCount != componentId ? 0 : 1);
				else
					tracker.SetVisible(i, editors[i].target != null ? 0 : 1);
			}

			var inspectorWindow2 = CSEditorTools.GetInspectorWindow();
			if (inspectorWindow2 != null)
			{
				inspectorWindow2.Repaint();
			}
		}

		private static bool RevealAndSelectGameObjectInScene(string path, string transformPath, long objectId, long componentId)
		{
			Scene targetScene;

			if (!string.IsNullOrEmpty(path))
			{
				var openResult = OpenSceneForReveal(path);
				if (!openResult.success) 
					return false;

				targetScene = openResult.scene;
			}
			else
			{
				targetScene = CSSceneTools.GetUntitledScene();
			}

			if (!targetScene.IsValid())
			{
				Debug.LogError(Maintainer.ErrorForSupport("Target scene is not valid or not found! Scene path: " + path + ", looked for ObjectID " + objectId + "!"));
				return false;
			}

			var target = CSObjectTools.FindGameObjectInScene(targetScene, objectId, transformPath);

			if (target == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Couldn't find target Game Object " + transformPath + " at " + path + " with ObjectID " + objectId + "!"));
				return false;
			}

			// workaround for a bug when Unity doesn't expand hierarchy in scene
			EditorApplication.delayCall += () =>
			{
				EditorGUIUtility.PingObject(target);
			};

			SelectGameObject(target, true);

			var enclosingAssetInstanceId = CSAssetTools.GetMainAssetInstanceID(path);
			EditorApplication.delayCall += () =>
			{
				EditorGUIUtility.PingObject(enclosingAssetInstanceId);
			};

			return TryFoldAllComponentsExceptId(componentId);
		}

		private static bool RevealAndSelectGameObjectInPrefab(string path, string transformPath, long objectId, long componentId)
		{
			/*Debug.Log("LOOKING FOR objectId " + objectId);
			Debug.Log("path " + path);*/

			bool prefabStageOpened;
			var prefabRoot = CSPrefabTools.OpenPrefabAndReturnRoot(path, out prefabStageOpened);
			if (prefabRoot == null)
				return false;
			
			var target = CSObjectTools.FindChildGameObjectRecursive(prefabRoot.transform, objectId, prefabRoot.transform.name, transformPath);

			EditorApplication.delayCall += () =>
			{
				SelectGameObject(target, false);
				EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));

				EditorApplication.delayCall += () =>
				{
					TryFoldAllComponentsExceptId(componentId);
				};
			};

			return true;
		}

		public static void SelectGameObject(GameObject go, bool inScene)
		{
			if (inScene)
			{
				Selection.activeTransform = go == null ? null : go.transform;
			}
			else
			{
				Selection.activeGameObject = go;
			}
		}
	}
}