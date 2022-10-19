#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Cleaner;
	using Core;
	using Issues.Detectors;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	public partial class ProjectSettings
	{
		private static void MigrateSettings(ProjectSettings settings)
		{
			if (string.IsNullOrEmpty(settings.version))
			{
				MigrateFilters(settings);
			}

			if (new Version(settings.version) < new Version("1.5.1"))
			{
				MigrateFromPre_1_5_1(settings);
			}
			
			if (new Version(settings.version) < new Version("1.7.4"))
			{
				MigrateFromPre_1_7_4(settings);
			}
			
			if (new Version(settings.version) < new Version("1.13.0"))
			{
#pragma warning disable 618
				MigrateFromPre_1_13_0(settings);
#pragma warning restore 618
			}
		}

		private static void MigrateFilters(ProjectSettings settings)
		{
			MigrateAllIgnores(settings.issuesFinderSettings.pathIgnores,
				ref settings.issuesFinderSettings.pathIgnoresFilters, FilterKind.Path);
			settings.issuesFinderSettings.pathIgnores = null;

			MigrateAllIgnores(settings.issuesFinderSettings.componentIgnores,
				ref settings.issuesFinderSettings.componentIgnoresFilters, FilterKind.Type);
			settings.issuesFinderSettings.componentIgnores = null;

			MigrateAllIgnores(settings.issuesFinderSettings.pathIncludes,
				ref settings.issuesFinderSettings.pathIncludesFilters, FilterKind.Path);
			settings.issuesFinderSettings.pathIncludes = null;

			MigrateAllIgnores(settings.issuesFinderSettings.sceneIncludes,
				ref settings.issuesFinderSettings.sceneIncludesFilters, FilterKind.Path);
			settings.issuesFinderSettings.sceneIncludes = null;

			MigrateAllIgnores(settings.projectCleanerSettings.pathIgnores,
				ref settings.projectCleanerSettings.pathIgnoresFilters, FilterKind.Path);
			settings.projectCleanerSettings.pathIgnores = null;

			MigrateAllIgnores(settings.projectCleanerSettings.sceneIgnores,
				ref settings.projectCleanerSettings.sceneIgnoresFilters, FilterKind.Path);
			settings.projectCleanerSettings.sceneIgnores = null;

			settings.projectCleanerSettings.AddDefaultFilters();
		}

		private static void MigrateFromPre_1_5_1(ProjectSettings settings)
		{
			if (settings.projectCleanerSettings.pathIgnoresFilters != null &&
			    settings.projectCleanerSettings.pathIgnoresFilters.Length > 0)
			{
				var defaultFilters = ProjectCleanerSettings.GetDefaultFilters();
				var mandatoryFilters = settings.projectCleanerSettings.MandatoryFilters;

				var modificationsLog = new StringBuilder();

				for (var i = settings.projectCleanerSettings.pathIgnoresFilters.Length - 1; i >= 0; i--)
				{
					var pathIgnoresFilter = settings.projectCleanerSettings.pathIgnoresFilters[i];
					if (pathIgnoresFilter.ignoreCase) continue;

					var isMandatory = false;

					if (CSFilterTools.IsValueMatchesAnyFilterOfKind(pathIgnoresFilter.value, mandatoryFilters,
						pathIgnoresFilter.kind))
					{
						isMandatory = true;
					}
					else
						switch (pathIgnoresFilter.kind)
						{
							case FilterKind.Extension:
								var extension = pathIgnoresFilter.value.ToLowerInvariant();
								if (extension == ".dll" ||
								    extension == ".asmdef" ||
								    extension == ".mdb" ||
								    extension == ".xml" ||
								    extension == ".rsp")
								{
									isMandatory = true;
								}

								break;

							case FilterKind.FileName:
								var value = pathIgnoresFilter.value.ToLowerInvariant();
								if (value == "readme" ||
								    value == "manual")
								{
									isMandatory = true;
								}

								break;
						}

					if (isMandatory)
					{
						modificationsLog.Append("Removing Project Cleaner filter '")
							.Append(pathIgnoresFilter.value)
							.AppendLine("': built-in mandatory filter covers it now.");
						ArrayUtility.RemoveAt(ref settings.projectCleanerSettings.pathIgnoresFilters, i);
						continue;
					}

					if (CSFilterTools.IsValueMatchesAnyFilterOfKind(pathIgnoresFilter.value, defaultFilters,
						pathIgnoresFilter.kind))
					{
						modificationsLog.Append("Changing default Project Cleaner filter '")
							.Append(pathIgnoresFilter.value)
							.AppendLine("': ignore case setting to 'true' for better accuracy.");
						pathIgnoresFilter.ignoreCase = true;
					}
				}

				if (modificationsLog.Length > 0)
				{
					modificationsLog.Insert(0, "Maintainer settings updated, read below for details\n");
					Debug.Log(Maintainer.ConstructLog(modificationsLog.ToString()));
				}
			}
		}

		private static void MigrateFromPre_1_7_4(ProjectSettings settings)
		{
			MigrateFiltersFromPre_1_7_4(settings.issuesFinderSettings.componentIgnoresFilters);
			MigrateFiltersFromPre_1_7_4(settings.issuesFinderSettings.pathIgnoresFilters);
			MigrateFiltersFromPre_1_7_4(settings.issuesFinderSettings.pathIncludesFilters);
			MigrateFiltersFromPre_1_7_4(settings.issuesFinderSettings.sceneIncludesFilters);
			MigrateFiltersFromPre_1_7_4(settings.projectCleanerSettings.pathIgnoresFilters);
			MigrateFiltersFromPre_1_7_4(settings.projectCleanerSettings.pathIncludesFilters);
			MigrateFiltersFromPre_1_7_4(settings.projectCleanerSettings.sceneIgnoresFilters);
			MigrateFiltersFromPre_1_7_4(settings.referencesFinderSettings.pathIgnoresFilters);
			MigrateFiltersFromPre_1_7_4(settings.referencesFinderSettings.pathIncludesFilters);
		}

		private static void MigrateFiltersFromPre_1_7_4(FilterItem[] filters)
		{
			if (filters == null)
				return;
			
			foreach (var filter in filters)
			{
				filter.enabled = true;
			}
		}

		[Obsolete("hacky way to ignore obsolete errors 👀", false)]
		private static void MigrateFromPre_1_13_0(ProjectSettings settings)
		{
			settings.issuesFinderSettings.SetDetectorEnabled<MissingReferenceDetector>(
					settings.issuesFinderSettings.missingReferences);
			
#if UNITY_2019_1_OR_NEWER
			settings.issuesFinderSettings.SetDetectorEnabled<ShaderErrorDetector>(
				settings.issuesFinderSettings.shadersWithErrors);
#endif
			
			settings.issuesFinderSettings.SetDetectorEnabled<MissingComponentDetector>(
				settings.issuesFinderSettings.missingComponents);
			
			settings.issuesFinderSettings.SetDetectorEnabled<MissingPrefabDetector>(
				settings.issuesFinderSettings.missingPrefabs);
			
			settings.issuesFinderSettings.SetDetectorEnabled<DuplicateComponentDetector>(
				settings.issuesFinderSettings.duplicateComponents);
			
			settings.issuesFinderSettings.SetDetectorEnabled<InconsistentTerrainDataDetector>(
				settings.issuesFinderSettings.inconsistentTerrainData);
			
			settings.issuesFinderSettings.SetDetectorEnabled<InvalidLayerDetector>(
				settings.issuesFinderSettings.unnamedLayers);
			
			settings.issuesFinderSettings.SetDetectorEnabled<HugePositionDetector>(
				settings.issuesFinderSettings.hugePositions);
			
			settings.issuesFinderSettings.SetDetectorEnabled<DuplicateLayersDetector>(
				settings.issuesFinderSettings.duplicateLayers);
		}

		private static void MigrateAllIgnores(ICollection<string> oldFilters, ref FilterItem[] newFilters, FilterKind filterKind)
		{
			if (oldFilters == null || oldFilters.Count == 0) return;

			var newFiltersList = new List<FilterItem>(oldFilters.Count);
			foreach (var oldFilter in oldFilters)
			{
				if (CSFilterTools.IsValueMatchesAnyFilter(oldFilter, newFilters)) continue;
				newFiltersList.Add(FilterItem.Create(oldFilter, filterKind));
			}

			ArrayUtility.AddRange(ref newFilters, newFiltersList.ToArray());
		}
	}
}