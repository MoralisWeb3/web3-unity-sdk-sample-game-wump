#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if UNITY_2019_1_OR_NEWER
namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	
#if !UNITY_2019_2_OR_NEWER
	[InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class ShaderErrorDetector : IssueDetector, IAssetBeginIssueDetector
	{

		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.Asset,
				DetectorKind.Defect,
				IssueSeverity.Error,
				"Shader with error(s)", 
				"Search for Shaders with compilation errors.");
		}}

		public Type[] AssetTypes { get { return new[] { CSReflectionTools.shaderType }; } }
		
#if !UNITY_2019_2_OR_NEWER
		static ShaderErrorDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new ShaderErrorDetector());
		}
#endif

		public void AssetBegin(DetectorResults results, AssetLocation location)
		{
			var loadedShader = AssetDatabase.LoadAssetAtPath<Shader>(location.Asset.Path);
			if (!ShaderUtil.ShaderHasError(loadedShader))
				return;
			
			var issue = ShaderIssueRecord.Create(this, location);
			results.Add(issue);
		}
	}
}
#endif