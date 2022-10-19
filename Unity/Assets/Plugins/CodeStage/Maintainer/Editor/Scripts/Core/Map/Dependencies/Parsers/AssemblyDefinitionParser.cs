#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Dependencies
{
	using System;
	using System.Collections.Generic;
	using Tools;
	using UnityEditor;
	using UnityEditor.Compilation;
	using UnityEngine;
	
#if !UNITY_2019_2_OR_NEWER
	[InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global
	// TODO: check if bug 1020737 is fixed and this can be removed
	internal class AssemblyDefinitionParser : DependenciesParser
	{
		private static string editorPlatformName;

		private static string EditorPlatformName
		{
			get
			{
				if (string.IsNullOrEmpty(editorPlatformName))
				{
					var platforms = CompilationPipeline.GetAssemblyDefinitionPlatforms();
					foreach (var platform in platforms)
					{
						if (platform.BuildTarget == BuildTarget.NoTarget)
						{
							editorPlatformName = platform.Name;
							break;
						}
					}
				}

				return editorPlatformName;
			}
		}
		
		public override Type Type
		{
			get
			{
				return CSReflectionTools.assemblyDefinitionAssetType;
			}
		}
		
#if !UNITY_2019_2_OR_NEWER
		static AssemblyDefinitionParser()
		{
			AssetDependenciesSearcher.AddInternalDependencyParser(new AssemblyDefinitionParser());
		}
#endif
		
		public override IList<string> GetDependenciesGUIDs(AssetInfo asset)
		{
			if (asset.Kind != AssetKind.Regular)
				return null;
			
			return GetAssetsReferencedFromAssemblyDefinition(asset.Path);
		}

		public static bool IsEditorOnlyAssembly(string asmdefPath)
		{
			var result = false;
			var data = ParseAssemblyDefinitionAsset(asmdefPath);
			if (data.includePlatforms == null || data.includePlatforms.Length == 0)
				return false;

			result = data.includePlatforms.Length == 1 && data.includePlatforms[0] == EditorPlatformName;

			data.includePlatforms = null; // to workaround compiler warning
			
			return result;
		}
		
		private static IList<string> GetAssetsReferencedFromAssemblyDefinition(string assetPath)
		{
			var result = new List<string>();
			var data = ParseAssemblyDefinitionAsset(assetPath);

			if (data.references != null && data.references.Length > 0)
			{
				foreach (var reference in data.references)
				{
#if !UNITY_2019_1_OR_NEWER
					var assemblyDefinitionFilePathFromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(reference);
#else
					var assemblyDefinitionFilePathFromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(reference);
#endif
					if (!string.IsNullOrEmpty(assemblyDefinitionFilePathFromAssemblyName))
					{
						assemblyDefinitionFilePathFromAssemblyName = CSPathTools.EnforceSlashes(assemblyDefinitionFilePathFromAssemblyName);
						var guid = AssetDatabase.AssetPathToGUID(assemblyDefinitionFilePathFromAssemblyName);
						if (!string.IsNullOrEmpty(guid))
						{
							result.Add(guid);
						}
					}
				}
			}

			data.references = null; // to workaround compiler warning

			return result;
		}

		private static AssemblyDefinitionData ParseAssemblyDefinitionAsset(string assetPath)
		{
			var asset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(assetPath);
			return JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);
		}

		[Serializable]
		private class AssemblyDefinitionData
		{
			public string[] references;
			public string[] includePlatforms;
		}
	}
}