#if UNITY_2019_2_OR_NEWER

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
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class AssemblyDefinitionReferenceParser : DependenciesParser
	{
		public override Type Type
		{
			get
			{
				return CSReflectionTools.assemblyDefinitionReferenceAssetType;
			}
		}

#if !UNITY_2019_2_OR_NEWER
		static AssemblyDefinitionReferenceParser()
		{
			AssetDependenciesSearcher.AddInternalDependencyParser(new AssemblyDefinitionReferenceParser());
		}
#endif
		
		public override IList<string> GetDependenciesGUIDs(AssetInfo asset)
		{
			if (asset.Kind != AssetKind.Regular)
				return null;
			
			return GetAssetsReferencedFromAssemblyDefinitionReference(asset.Path);
		}
		
		private IList<string> GetAssetsReferencedFromAssemblyDefinitionReference(string assetPath)
		{
			var result = new List<string>();

			var asset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionReferenceAsset>(assetPath);
			var data = JsonUtility.FromJson<AssemblyDefinitionReferenceData>(asset.text);

			if (!string.IsNullOrEmpty(data.reference))
			{
				var assemblyDefinitionPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(data.reference);
				if (!string.IsNullOrEmpty(assemblyDefinitionPath))
				{
					assemblyDefinitionPath = CSPathTools.EnforceSlashes(assemblyDefinitionPath);
					var guid = AssetDatabase.AssetPathToGUID(assemblyDefinitionPath);
					if (!string.IsNullOrEmpty(guid))
					{
						result.Add(guid);
					}
				}
			}

			data.reference = null;

			return result;
		}

		private class AssemblyDefinitionReferenceData
		{
			public string reference;
		}
	}
}
#endif