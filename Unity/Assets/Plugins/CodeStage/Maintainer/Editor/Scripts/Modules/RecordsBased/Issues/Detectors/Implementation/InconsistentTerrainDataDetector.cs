#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;
	using UnityEngine;

#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class InconsistentTerrainDataDetector : IssueDetector, IComponentBeginIssueDetector, IGameObjectEndIssueDetector
	{
		public override DetectorInfo Info { get { return 
			DetectorInfo.From(
				IssueGroup.GameObject,
				DetectorKind.Defect,
				IssueSeverity.Warning,
				"Inconsistent Terrain Data", 
				"Search for Game Objects where Terrain and TerrainCollider have different Terrain Data.");
		}}

		public Type[] ComponentTypes
		{
			get { return new[] { 
				typeof(Terrain), 
				typeof(TerrainCollider) 
			}; }
		}

		private TerrainData terrainData;
		private TerrainData colliderTerrainData;
		private bool terrainChecked;
		private bool colliderChecked;

		private Type componentType;
		private string componentName;
		private int componentIndex;
		
#if !UNITY_2019_2_OR_NEWER
		static InconsistentTerrainDataDetector()
		{
			IssuesFinderDetectors.AddInternalDetector(new InconsistentTerrainDataDetector());
		}
#endif
		
		public void ComponentBegin(DetectorResults results, ComponentLocation location)
		{
			if (location.Component is Terrain)
				ProcessTerrainComponent(location.Component as Terrain, location.ComponentType, location.ComponentName, location.ComponentIndex);
			else if (location.Component is TerrainCollider)
				ProcessTerrainColliderComponent(location.Component as TerrainCollider);
			else
				Debug.LogError(Maintainer.ErrorForSupport("Unexpected component: " + location.Component + " (" + location.ComponentType + ")"));
		}

		public void GameObjectEnd(DetectorResults results, GameObjectLocation location)
		{
			if (terrainChecked && colliderChecked && colliderTerrainData != terrainData)
			{
				var narrow = location.Narrow();
				narrow.ComponentOverride(componentType, componentName, componentIndex);
				var issue = GameObjectIssueRecord.ForComponent(this, Issues.IssueKind.InconsistentTerrainData, narrow);
				issue.HeaderPostfix = "at Terrain and TerrainCollider";
				results.Add(issue);
			}
			
			terrainChecked = false;
			colliderChecked = false;

			terrainData = null;
			colliderTerrainData = null;
		}

		private void ProcessTerrainComponent(Terrain component, Type type, string name, int index)
		{
			componentType = type;
			componentName = name;
			componentIndex = index;

			terrainData = component.terrainData;
			terrainChecked = true;
		}

		private void ProcessTerrainColliderComponent(TerrainCollider component)
		{
			colliderTerrainData = component.terrainData;
			colliderChecked = true;
		}
	}
}