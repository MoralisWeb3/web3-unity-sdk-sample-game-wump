#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Routines
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	internal static class TargetCollector
	{
		internal static AssetInfo[] CollectTargetAssets(out bool canceled)
		{
			var map = AssetsMap.GetUpdated(out canceled);
			if (map == null)
			{
				if (!canceled)
					Debug.LogError(Maintainer.ConstructLog("Couldn't get updated assets map!"));
				return null;
			}

			EditorUtility.DisplayProgressBar(IssuesFinder.ModuleName, "Collecting input data...", 0);

			var supportedKinds = new List<AssetKind> {AssetKind.Regular, AssetKind.Settings};
			var assets = CSFilterTools.GetAssetInfosWithKinds(map.assets, supportedKinds);
			
			var result = new HashSet<AssetInfo>();

			try
			{
				var targetAssetTypes = new List<TypeFilter>();

				if (ProjectSettings.Issues.lookInScenes)
				{
					switch (ProjectSettings.Issues.scenesSelection)
					{
						case IssuesFinderSettings.ScenesSelection.AllScenes:
						{
							targetAssetTypes.Add(new TypeFilter(CSReflectionTools.sceneAssetType, false));
							TryIncludeUntitledScene(result);
							
							break;
						}
						case IssuesFinderSettings.ScenesSelection.IncludedScenes:
						{
							if (ProjectSettings.Issues.includeScenesInBuild)
							{
								var paths = CSSceneTools.GetScenesInBuild(!ProjectSettings.Issues.includeOnlyEnabledScenesInBuild);
								result.UnionWith(CSFilterTools.GetAssetInfosWithPaths(assets, paths));
							}

							var assetInfos = CSFilterTools.FilterAssetInfos(assets, ProjectSettings.Issues.sceneIncludesFilters);
							result.UnionWith(assetInfos);

							break;
						}
						case IssuesFinderSettings.ScenesSelection.OpenedScenesOnly:
						{
							var paths = CSSceneTools.GetScenesSetup().Select(scene => scene.path).ToArray();
							result.UnionWith(CSFilterTools.GetAssetInfosWithPaths(assets, paths));
							TryIncludeUntitledScene(result);

							break;
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				if (ProjectSettings.Issues.lookInAssets)
				{
					targetAssetTypes.Add(new TypeFilter(CSReflectionTools.scriptableObjectType));
					targetAssetTypes.Add(new TypeFilter(CSReflectionTools.textAssetType));
					targetAssetTypes.Add(new TypeFilter(CSReflectionTools.objectType, new []{CSReflectionTools.gameObjectType, CSReflectionTools.sceneAssetType}));
					targetAssetTypes.Add(new TypeFilter(null, false));
					
					if (ProjectSettings.Issues.scanGameObjects)
					{
						targetAssetTypes.Add(new TypeFilter(CSReflectionTools.gameObjectType, false));
					}
				}

				var filtered = CSFilterTools.FilterAssetInfos(
					assets,
					targetAssetTypes,
					ProjectSettings.Issues.pathIncludesFilters,
					ProjectSettings.Issues.pathIgnoresFilters
				);

				result.UnionWith(filtered);

				if (ProjectSettings.Issues.lookInProjectSettings)
				{
					result.UnionWith(CSFilterTools.GetAssetInfosWithKind(assets, AssetKind.Settings));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			var resultArray = new AssetInfo[result.Count];
			result.CopyTo(resultArray);
			return resultArray;
		}

		private static void TryIncludeUntitledScene(HashSet<AssetInfo> result)
		{
			var untitledScene = CSSceneTools.GetUntitledScene();
			if (!untitledScene.IsValid())
				return;

			var asset = AssetInfo.CreateUntitledScene();
			result.Add(asset);
		}
	}
}