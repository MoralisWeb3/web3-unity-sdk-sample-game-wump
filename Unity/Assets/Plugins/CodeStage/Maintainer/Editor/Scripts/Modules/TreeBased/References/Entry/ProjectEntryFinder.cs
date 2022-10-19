#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References.Entry
{
	using Core;
	using Settings;
	using System;
	using System.Collections.Generic;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	internal static class ProjectEntryFinder
	{
		public static bool FillProjectScopeReferenceEntries(List<AssetConjunctions> conjunctionInfoList, ProcessObjectReferenceHandler processReferenceCallback)
		{
			var canceled = false;
			EntryFinder.currentScope = EntryFinderScope.Project;
			EntryFinder.currentProcessReferenceCallback = processReferenceCallback;

			var count = conjunctionInfoList.Count;
		
#if !UNITY_2020_1_OR_NEWER
			var updateStep = Math.Max(count / ProjectSettings.UpdateProgressStep, 1);
#endif

			for (var i = 0; i < count; i++)
			{
				if (
#if !UNITY_2020_1_OR_NEWER
					(i < 10 || i % updateStep == 0) && 
#endif
				    EditorUtility.DisplayCancelableProgressBar(
						string.Format(ReferencesFinder.ProgressCaption, 2, ReferencesFinder.PhasesCount), string.Format(ReferencesFinder.ProgressText, "Filling reference details", i + 1, count),
						(float)i / count))
				{
					canceled = true;
					break;
				}

				var assetConjunctions = conjunctionInfoList[i];
				ProjectScopeReferencesFinder.currentAssetConjunctions = assetConjunctions;

				ProcessProjectScopeTargets(assetConjunctions);
				SetReferencingEntries(assetConjunctions.conjunctions);
			}

			return canceled;
		}

		private static void ProcessProjectScopeTargets(AssetConjunctions assetConjunctions)
		{
			var asset = assetConjunctions.asset;
			var type = asset.Type;
			var path = asset.Path;

			if (type == CSReflectionTools.gameObjectType)
			{
				ProcessPrefab(path);
			}
			else if (type == CSReflectionTools.sceneAssetType)
			{
				ProcessSceneForProjectLevelReferences(path, assetConjunctions.conjunctions);
			}
			else if (type == CSReflectionTools.monoScriptType)
			{
				ProcessScriptAsset(path);
			}
			else if (type != null && (type == CSReflectionTools.monoBehaviourType ||
									  type.BaseType == CSReflectionTools.scriptableObjectType ||
									  type.BaseType == CSReflectionTools.objectType))
			{
				ProcessUnityObjectAsset(path);
			}
		}
		
		private static void SetReferencingEntries(List<TreeConjunction> conjunctions)
		{
			foreach (var conjunction in conjunctions)
			{
				var referencedAtInfo = conjunction.referencedAtInfo;

				if (referencedAtInfo.entries == null || referencedAtInfo.entries.Length == 0) 
				{
					var newEntry = new ReferencingEntryData
					{
						location = Location.NotFound,
						prefixLabel = "No exact reference place found."
					};

					if (!UserSettings.References.DeepProjectSearch)
						newEntry.prefixLabel += " Try enabling Deep Search.";

					var referencedAtAssetInfo = referencedAtInfo as ReferencedAtAssetInfo;

					if (referencedAtAssetInfo != null &&
					    referencedAtAssetInfo.assetInfo.Type == CSReflectionTools.sceneAssetType)
					{
						var sceneSpecificEntry = new ReferencingEntryData
						{
							location = Location.NotFound,
							prefixLabel = "Please try to remove all missing prefabs/scripts (if any) and re-save scene, it may cleanup junky dependencies."
						};

						referencedAtInfo.entries = new[] {newEntry, sceneSpecificEntry};
					}
					else if (referencedAtAssetInfo != null &&
					         referencedAtAssetInfo.assetInfo.Type == CSReflectionTools.gameObjectType)
					{
						var prefabSpecificEntry = new ReferencingEntryData
						{
							location = Location.NotFound,
							prefixLabel =
								"Please try to re-Apply prefab explicitly, this may clean up junky dependencies."
						};

						referencedAtInfo.entries = new[] {newEntry, prefabSpecificEntry};
					}
					else
					{
						referencedAtInfo.entries = new[] {newEntry};
					}

					if (ReferencesFinder.debugMode)
					{
						if (conjunction.referencedAsset != null)
						{
							Debug.LogWarning(Maintainer.ConstructLog(
								"Couldn't determine where exactly this asset is referenced: " +
								conjunction.referencedAsset.Path, ReferencesFinder.ModuleName));
						}
					}
				}

				foreach (var targetTreeElement in conjunction.treeElements)
				{
					targetTreeElement.referencingEntries = referencedAtInfo.entries;
				}
			}
		}
		
		private static void ProcessPrefab(string path)
		{
			var prefabRootGameObject = CSPrefabTools.GetPrefabAssetRoot(path);
			if (prefabRootGameObject == null)
				return;
			
			EntryFinder.currentLocation = Location.PrefabAssetGameObject;
			CSTraverseTools.TraversePrefabGameObjects(prefabRootGameObject, true, false, EntryFinder.OnGameObjectTraverse);

			// specific cases handling for main asset -----------------------------------------------------

			/*var importSettings = AssetImporter.GetAtPath(path) as ModelImporter;
			if (importSettings == null) return;

			var settings = new EntryAddSettings { suffix = "| Model Importer: RIG > Source" };
			TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, prefabRootGameObject, importSettings.sourceAvatar, settings);

			for (var i = 0; i < importSettings.clipAnimations.Length; i++)
			{
				var clipAnimation = importSettings.clipAnimations[i];
				settings.suffix = "| Model Importer: Animations [" + clipAnimation.name + "] > Mask";
				TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, prefabRootGameObject, clipAnimation.maskSource, settings);
			}*/

			var allObjectsInPrefab = AssetDatabase.LoadAllAssetsAtPath(path);

			foreach (var objectOnPrefab in allObjectsInPrefab)
			{
				if (objectOnPrefab == null) continue;
				if (objectOnPrefab is GameObject || objectOnPrefab is Component) continue;

				EntryFinder.currentLocation = Location.PrefabAssetObject;

				var addSettings = new EntryAddSettings();

				EntryFinder.TraverseObjectProperties(objectOnPrefab, objectOnPrefab, addSettings);

				/*if (AssetDatabase.IsMainAsset(objectOnPrefab))
				{

				}
				else*/
				{
					// specific cases handling ------------------------------------------------------------------------
					/*if (objectOnPrefab is BillboardAsset)
					{
						var billboardAsset = objectOnPrefab as BillboardAsset;
						var settings = new EntryAddSettings { suffix = "| BillboardAsset: Material" };
						TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, billboardAsset, billboardAsset.material, settings);
					}
					else if (objectOnPrefab is TreeData)
					{
						CachedObjectData objectInAssetCachedData = null;
						InspectComponent(assetConjunctions.conjunctions, objectOnPrefab, objectOnPrefab, -1, true, ref objectInAssetCachedData);
					}*/
				}
			}
		}

		private static void ProcessSceneForProjectLevelReferences(string path, List<TreeConjunction> conjunctions)
		{
			var openSceneResult = CSSceneTools.OpenScene(path);
			if (!openSceneResult.success)
			{
				Debug.LogWarning(Maintainer.ConstructLog("Can't open scene " + path));
				return;
			}

			SceneSettingsProcessor.Process(conjunctions);

			EntryFinder.currentLocation = Location.SceneGameObject;
			CSTraverseTools.TraverseSceneGameObjects(openSceneResult.scene, true, false, EntryFinder.OnGameObjectTraverse);

			CSSceneTools.CloseOpenedSceneIfNeeded(openSceneResult);
		}
		
		private static void ProcessScriptAsset(string path)
		{
			var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
			if (mainAsset == null) return;

			EntryFinder.currentLocation = Location.ScriptAsset;

			var addSettings = new EntryAddSettings();

			EntryFinder.TraverseObjectProperties(mainAsset, mainAsset, addSettings);
		}

		private static void ProcessUnityObjectAsset(string path)
		{
			var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
			if (mainAsset == null) return;

			EntryFinder.currentLocation = Location.UnityObjectAsset;

			var addSettings = new EntryAddSettings();

			EntryFinder.TraverseObjectProperties(mainAsset, mainAsset, addSettings);
		}
	}
}