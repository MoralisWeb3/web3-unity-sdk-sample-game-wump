#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#pragma warning disable 0414

namespace CodeStage.Maintainer.Settings
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Core.Extension;
	using Issues;
	using Issues.Detectors;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Issues Finder module settings saved in ProjectSettings folder.
	/// </summary>
	/// Contains various settings for this module.
	[Serializable]
	public class IssuesFinderSettings
	{
		/// <summary>
		/// Defines which scenes will be included into the issues search.
		/// </summary>
		[Serializable]
		public enum ScenesSelection
		{
			/// <summary>
			/// All project scenes will be included.
			/// </summary>
			/// Filtering settings have higher priority so you still can filter out some of them.
			AllScenes,
			
			/// <summary>
			/// Only scenes added to the Scene Includes will be included.
			/// This includes the includeScenesInBuild and sceneIncludesFilters.
			/// </summary>
			/// Filtering settings have higher priority so you still can filter included scenes further.
			IncludedScenes,
			
			/// <summary>
			/// Includes only opened scenes into the issues search.
			/// </summary>
			OpenedScenesOnly
		}

		/// <summary>
		/// Represents single issue detector setting.
		/// </summary>
		[Serializable]
		public class DetectorSetting
		{
			public string id;
			public IssueGroup group;
			public bool enabled = true;
		}

		// -----------------------------------------------------------------------------
		// filtering
		// -----------------------------------------------------------------------------

		// TODO: refactor into the single enum
		
		/// <summary>
		/// Determines if scenes added to the Build Settings should be included into the issues search while using ScenesSelection.IncludedScenes.
		/// </summary>
		public bool includeScenesInBuild = true;
		
		/// <summary>
		/// Allows specifying if only enabled scenes from the Build Settings should be included into the issues search when using the includeScenesInBuild setting.
		/// </summary>
		public bool includeOnlyEnabledScenesInBuild = true;

		// obsolete settings, left for migration purposes only
		[SerializeField] internal string[] sceneIncludes = new string[0];
		[SerializeField] internal string[] pathIgnores = new string[0];
		[SerializeField] internal string[] pathIncludes = new string[0];
		[SerializeField] internal string[] componentIgnores = new string[0];

		public FilterItem[] sceneIncludesFilters = new FilterItem[0];
		public FilterItem[] pathIgnoresFilters = new FilterItem[0];
		public FilterItem[] pathIncludesFilters = new FilterItem[0];
		public FilterItem[] componentIgnoresFilters = new FilterItem[0];

		// -----------------------------------------------------------------------------
		// where to look
		// -----------------------------------------------------------------------------

		public bool lookInScenes;
		public bool lookInAssets;
		public bool lookInProjectSettings;

		public ScenesSelection scenesSelection;

		// TODO: refactor into the single enum
		public bool scanGameObjects;
		
		[SerializeField]
		public bool touchInactiveGameObjects;
		
		[SerializeField]
		public bool touchDisabledComponents;
		
		// -----------------------------------------------------------------------------
		// what to look for
		// -----------------------------------------------------------------------------

		public DetectorSetting[] detectors;
		
		/* project-wide  */

		[Obsolete("Please use MissingReferences property instead!", true)]
		public bool missingReferences;
		public bool MissingReferences
		{
			get { return GetDetectorEnabled<MissingReferenceDetector>(); }
			set { SetDetectorEnabled<MissingReferenceDetector>(value); }
		}

#if UNITY_2019_1_OR_NEWER
		[Obsolete("Please use ShadersWithErrors property instead!", true)]
		public bool shadersWithErrors;
		public bool ShadersWithErrors
		{
			get { return GetDetectorEnabled<ShaderErrorDetector>(); }
			set { SetDetectorEnabled<ShaderErrorDetector>(value); }
		}
#endif

		/* game objects common  */
		[Obsolete("Please use MissingComponents property instead!", true)]
		public bool missingComponents;
		public bool MissingComponents
		{
			get { return GetDetectorEnabled<MissingComponentDetector>(); }
			set { SetDetectorEnabled<MissingComponentDetector>(value); }
		}
		
		[Obsolete("Please use MissingPrefabs property instead!", true)]
		public bool missingPrefabs;
		public bool MissingPrefabs
		{
			get { return GetDetectorEnabled<MissingPrefabDetector>(); }
			set { SetDetectorEnabled<MissingPrefabDetector>(value); }
		}

		[Obsolete("Please use DuplicateComponents property instead!", true)]
		public bool duplicateComponents;
		public bool DuplicateComponents
		{
			get { return GetDetectorEnabled<DuplicateComponentDetector>(); }
			set { SetDetectorEnabled<DuplicateComponentDetector>(value); }
		}
		
		[Obsolete("Please use InconsistentTerrainData property instead!", true)]
		public bool inconsistentTerrainData;
		public bool InconsistentTerrainData
		{
			get { return GetDetectorEnabled<InconsistentTerrainDataDetector>(); }
			set { SetDetectorEnabled<InconsistentTerrainDataDetector>(value); }
		}

		/* game objects neatness */

		[Obsolete("Please use InvalidLayers property instead!", true)]
		public bool unnamedLayers;
		public bool InvalidLayers
		{
			get { return GetDetectorEnabled<InvalidLayerDetector>(); }
			set { SetDetectorEnabled<InvalidLayerDetector>(value); }
		}
		
		[Obsolete("Please use HugePositions property instead!", true)]
		public bool hugePositions;
		public bool HugePositions
		{
			get { return GetDetectorEnabled<HugePositionDetector>(); }
			set { SetDetectorEnabled<HugePositionDetector>(value); }
		}

		/* project settings */

		[Obsolete("Please use DuplicateLayers property instead!", true)]
		public bool duplicateLayers;
		public bool DuplicateLayers
		{
			get { return GetDetectorEnabled<DuplicateLayersDetector>(); }
			set { SetDetectorEnabled<DuplicateLayersDetector>(value); }
		}
		
		public IssuesFinderSettings()
		{
            Reset();
		}
		
		public int GetFiltersCount()
		{
			return sceneIncludesFilters.Length + pathIgnoresFilters.Length + pathIncludesFilters.Length +
			       componentIgnoresFilters.Length;
		}
		
		public void SyncDetectors()
		{
			var existingDetectors = IssuesFinderDetectors.detectors.extensions;

			// cleaning settings from non-existing detectors
			for (var i = detectors.Length - 1; i >= 0; i--)
			{
				var matchingDetectorFound = false;
				var detectorSetting = detectors[i];
				foreach (var existingDetector in existingDetectors)
				{
					if (existingDetector.Id == detectorSetting.id)
					{
						matchingDetectorFound = true;
						break;
					}
				}

				if (!matchingDetectorFound)
				{
					ArrayUtility.Remove(ref detectors, detectorSetting);
				}
			}
			
			// adding new detectors to the settings
			var newSettings = new List<DetectorSetting>();
			foreach (var existingDetector in existingDetectors)
			{
				var matchedSettingFound = false;
				for (var i = detectors.Length - 1; i >= 0; i--)
				{
					var detectorSetting = detectors[i];
					if (detectorSetting.id == existingDetector.Id)
					{
						matchedSettingFound = true;
						break;
					}
				}

				if (!matchedSettingFound)
				{
					var newSetting = new DetectorSetting
					{
						id = existingDetector.Id,
						enabled = true
					};
					newSettings.Add(newSetting);
				}
			}
			
			ArrayUtility.AddRange(ref detectors, newSettings.ToArray());
		}
		
		internal bool GetDetectorEnabled(IIssueDetector instance)
		{
			var id = instance.Id;
			return GetDetectorEnabled(id);
		}

		internal bool GetDetectorEnabled<T>() where T : IMaintainerExtension
		{
			var id = MaintainerExtension.GetId<T>();
			return GetDetectorEnabled(id);
		}
		
		internal void SetDetectorEnabled(IIssueDetector instance, bool enabled)
		{
			var id = instance.Id;
			SetDetectorEnabled(id, enabled);
		}
		
		internal void SetDetectorEnabled<T>(bool enabled) where T : IMaintainerExtension
		{
			var id = MaintainerExtension.GetId<T>();
			SetDetectorEnabled(id, enabled);
		}

		internal void SwitchDetectors(IList<IIssueDetector> detectorsInGroup, bool enabled)
		{
			foreach (var detector in detectors)
			{
				foreach (var issueDetector in detectorsInGroup)
				{
					if (detector.id == issueDetector.Id)
						detector.enabled = enabled;
				}
			}
		}

		internal void SwitchAllIssues(bool enable)
		{
			MissingReferences = enable;

#if UNITY_2019_1_OR_NEWER
			ShadersWithErrors = enable;
#endif
			SwitchCommon(enable);
			SwitchNeatness(enable);
			SwitchProjectSettings(enable);

			foreach (var detector in detectors)
			{
				detector.enabled = enable;
			}
		}

		internal void SwitchCommon(bool enable)
		{
			MissingComponents = enable;
			MissingPrefabs = enable;
			DuplicateComponents = enable;
			InconsistentTerrainData = enable;
		}

		internal void SwitchNeatness(bool enable)
		{
			InvalidLayers = enable;
			HugePositions = enable;
		}

		internal void SwitchProjectSettings(bool enable)
		{
			DuplicateLayers = enable;
		}

		internal void Reset()
		{
			scanGameObjects = true;
			lookInProjectSettings = true;
			lookInScenes = true;
			scenesSelection = ScenesSelection.AllScenes;
			lookInAssets = true;
			touchInactiveGameObjects = true;
			touchDisabledComponents = true;

			SwitchAllIssues(true);
		}
		
		private bool GetDetectorEnabled(string id)
		{
			if (detectors == null)
				detectors = new DetectorSetting[0];
			
			foreach (var detectorSetting in detectors)
			{
				if (detectorSetting.id == id)
					return detectorSetting.enabled;
			}

			var newSetting = new DetectorSetting { id = id, enabled = true };
			ArrayUtility.Add(ref detectors, newSetting);

			return false;
		}
		
		private void SetDetectorEnabled(string id, bool enabled)
		{
			if (detectors == null)
				detectors = new DetectorSetting[0];
			
			foreach (var detectorSetting in detectors)
			{
				if (detectorSetting.id == id)
				{
					detectorSetting.enabled = enabled;
					return;
				}
			}
			
			var newSetting = new DetectorSetting { id = id, enabled = enabled };
			ArrayUtility.Add(ref detectors, newSetting);
		}
	}
}