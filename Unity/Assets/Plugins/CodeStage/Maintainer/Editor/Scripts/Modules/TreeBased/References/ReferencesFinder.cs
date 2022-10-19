#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using CodeStage.Maintainer.Tools;

namespace CodeStage.Maintainer.References
{
	using UnityEditor;
	using Core;
	using Settings;
	using Object = UnityEngine.Object;

	/// <summary>
	/// Allows to find references of specific objects in your project (where objects are referenced).
	/// </summary>
	public static class ReferencesFinder
	{
		internal const string ModuleName = "References Finder";

		internal const string ProgressText = "{0}: item {1} of {2}";

		internal const string ProgressCaption = ModuleName + ": phase {0} of {1}";
		
		internal const int PhasesCount = 2;

		public static bool debugMode;

		/// <summary>
		/// Finds references for current Project View selection.
		/// </summary>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ProjectReferenceItem for the TreeView buildup or manual parsing.</returns>
		public static ProjectReferenceItem[] FindSelectedAssetsReferences(bool showResults = true)
		{
			var selection = ProjectScopeReferencesFinder.GetSelectedAssets();
			return FindAssetsReferences(selection);
		}

		/// <summary>
		/// Finds references for specific asset.
		/// </summary>
		/// <param name="asset">Specific asset.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ProjectReferenceItem for the TreeView buildup or manual parsing.</returns>
		public static ProjectReferenceItem[] FindAssetReferences(string asset, bool showResults = true)
		{
			return FindAssetsReferences(new []{ asset }, showResults);
		}

		/// <summary>
		/// Finds references for specific assets.
		/// </summary>
		/// <param name="assets">Specific assets.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ProjectReferenceItem for the TreeView buildup or manual parsing.</returns>
		public static ProjectReferenceItem[] FindAssetsReferences(string[] assets, bool showResults = true)
		{
			var assetsFilters = new List<FilterItem>(assets.Length);
			
			foreach (var assetPath in assets)
			{
				FilterItem filter;
				if (AssetDatabase.IsValidFolder(assetPath))
				{
					var subfolders = CSPathTools.GetAllDirectoriesRecursive(assetPath);
					foreach (var subfolder in subfolders)
					{
						if (Array.IndexOf(assets, subfolder) == -1)
						{
							assetsFilters.Add(FilterItem.Create(subfolder, FilterKind.Directory, false, true));
						}
					}
					
					filter = FilterItem.Create(assetPath, FilterKind.Directory, false, true);
				}
				else
				{
					filter = FilterItem.Create(assetPath, FilterKind.Path);
				}

				assetsFilters.Add(filter);
			}

			return ProjectScopeReferencesFinder.FindAssetsReferences(assetsFilters.ToArray(), false, showResults);
		}

		/// <summary>
		/// Returns references of all assets at the project, e.g. where each asset is referenced.
		/// </summary>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ProjectReferenceItem for the TreeView buildup or manual parsing.</returns>
		public static ProjectReferenceItem[] FindAllAssetsReferences(bool showResults = true)
		{
			if (!UserSettings.References.fullProjectScanWarningShown)
			{
				if (!EditorUtility.DisplayDialog(ModuleName,
					"Full project scan may take significant amount of time if your project is very big.\nAre you sure you wish to make a full project scan?\nThis message shows only before first full scan.",
					"Yes", "Nope"))
				{
					return null;
				}

				UserSettings.References.fullProjectScanWarningShown = true;
				ProjectSettings.Save();
			}

			return ProjectScopeReferencesFinder.FindAssetsReferences(null, null, showResults);
		}

		/// <summary>
		/// Finds object(s) references in current Hierarchy (scenes and prefabs are supported).
		/// </summary>
		/// <param name="objects">Target objects you wish to look references for (Game Objects, Components)</param>
		/// <param name="checkAllComponentsOfGameObjects">Includes all Components of passed GameObjects to the references search if True is passed.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of HierarchyReferenceItem for the TreeView buildup or manual parsing.</returns>
		public static HierarchyReferenceItem[] FindObjectsReferencesInHierarchy(Object[] objects, bool checkAllComponentsOfGameObjects = false, bool showResults = true)
		{
			return HierarchyScopeReferencesFinder.FindObjectsReferencesInHierarchy(objects, checkAllComponentsOfGameObjects, showResults);
		}
	}
}