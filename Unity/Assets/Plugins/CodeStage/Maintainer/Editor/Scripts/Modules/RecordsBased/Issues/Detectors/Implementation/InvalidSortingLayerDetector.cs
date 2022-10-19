#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if false // for future use

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;
	using Tools;
	using UnityEditor;
	using UnityEngine;

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class InvalidSortingLayerDetector : IssueDetector, IComponentBeginIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.Component,
				DetectorKind.Defect,
				IssueSeverity.Info,
				"Invalid Sorting Layer", 
				"Search for Renderer-derived components with invalid Sorting Layer.");
		}}
		
		public Type[] ComponentTypes { get { return new[] { CSReflectionTools.rendererType }; } }
		
#if !UNITY_2019_2_OR_NEWER
		static InvalidSortingLayerDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new InvalidSortingLayerDetector());
		}
#endif
		
		public void ComponentBegin(DetectorResults results, ComponentLocation location)
		{
			var so = new SerializedObject(location.Component);
			var sortingLayerIdProperty = so.FindProperty("m_SortingLayerID");
			if (sortingLayerIdProperty == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_SortingLayerID property at the component " + location.Component,
					IssuesFinder.ModuleName), location.Component);
				return;
			}

			var id = sortingLayerIdProperty.intValue;
			if (!SortingLayer.IsValid(id))
			{
				var narrow = location.Narrow();
				narrow.PropertyOverride("Sorting Layer");
				results.Add(GameObjectIssueRecord.ForProperty(this, Issues.IssueKind.InvalidSortingLayer, narrow));
			}
		}
	}
}

#endif