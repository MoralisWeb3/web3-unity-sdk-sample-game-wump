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

#if !UNITY_2019_2_OR_NEWER
	[InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class SettingsParser : DependenciesParser
	{
		public override Type Type
		{
			get
			{
				return null;
			}
		}
		
#if !UNITY_2019_2_OR_NEWER
		static SettingsParser()
		{
			AssetDependenciesSearcher.AddInternalDependencyParser(new SettingsParser());
		}
#endif

		public override IList<string> GetDependenciesGUIDs(AssetInfo asset)
		{
			if (asset.Kind != AssetKind.Settings)
				return null;
			
			var result = new List<string>();
			
			if (asset.SettingsKind == AssetSettingsKind.EditorBuildSettings)
			{
				var scenesInBuildGUIDs = CSSceneTools.GetScenesInBuildGUIDs(true);
				if (scenesInBuildGUIDs != null)
				{
					result.AddRange(scenesInBuildGUIDs);
				}
			}
			else
			{
				var settingsAsset = AssetDatabase.LoadAllAssetsAtPath(asset.Path);
				if (settingsAsset != null && settingsAsset.Length > 0)
				{
					var settingsAssetSerialized = new SerializedObject(settingsAsset[0]);

					var sp = settingsAssetSerialized.GetIterator();
					while (sp.Next(true))
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							var instanceId = sp.objectReferenceInstanceIDValue;
							if (instanceId != 0)
							{
								var referencePath = CSPathTools.EnforceSlashes(AssetDatabase.GetAssetPath(instanceId));
								if (!string.IsNullOrEmpty(referencePath) && referencePath.StartsWith("Assets"))
								{
									var guid = AssetDatabase.AssetPathToGUID(referencePath);
									if (!string.IsNullOrEmpty(guid))
									{
										result.Add(guid);
									}
								}
							}
						}
					}
				}
			}
			
			return result;
		}
	}
}