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

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class TextAssetParser : DependenciesParser
	{
		public override Type Type
		{
			get
			{
				return CSReflectionTools.textAssetType;
			}
		}
		
#if !UNITY_2019_2_OR_NEWER
		static TextAssetParser()
		{
			AssetDependenciesSearcher.AddInternalDependencyParser(new TextAssetParser());
		}
#endif

		public override IList<string> GetDependenciesGUIDs(AssetInfo asset)
		{
			if (asset.Path.EndsWith(".cginc"))
			{
				// below is an another workaround for dependenciesGUIDs not include #include-ed files, like *.cginc
				return ShaderParser.ScanFileForIncludes(asset.Path);
			}

			return null;
		}
	}
}