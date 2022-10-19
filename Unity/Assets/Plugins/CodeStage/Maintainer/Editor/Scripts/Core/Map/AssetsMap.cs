#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;

	using UnityEditor;
	using UnityEngine;

	using Settings;
	using Tools;
	using EditorCommon.Tools;

	[Serializable]
	public class AssetsMap
	{
		private const string MapPath = "Library/MaintainerMap.dat";

		private static AssetsMap cachedMap;

		internal readonly List<AssetInfo> assets = new List<AssetInfo>();
		public string version;

		public static AssetsMap CreateNew(out bool canceled)
		{
			Delete();
			return GetUpdated(out canceled);
		}

		public static void Delete()
		{
			cachedMap = null;
			CSFileTools.DeleteFile(MapPath);
		}

		public static AssetsMap GetUpdated(out bool canceled)
		{
			canceled = false;
			
			if (cachedMap == null)
				cachedMap = LoadMap(MapPath);

			if (cachedMap == null)
				cachedMap = new AssetsMap {version = Maintainer.Version};

			try
			{
				if (UpdateMap(cachedMap))
				{
					SaveMap(MapPath, cachedMap);
				}
				else
				{
					canceled = true;
					cachedMap.assets.Clear();
					cachedMap = null;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			EditorUtility.ClearProgressBar();

			return cachedMap;
		}

		public static void Save()
		{
			if (cachedMap != null)
			{
				SaveMap(MapPath, cachedMap);
			}
		}
		
		internal static void ResetReferenceEntries()
		{
			if (cachedMap == null)
				cachedMap = LoadMap(MapPath);

			if (cachedMap == null || cachedMap.assets == null || cachedMap.assets.Count == 0)
				return;

			var dirty = false;
			
			foreach (var assetInfo in cachedMap.assets)
			{
				foreach (var info in assetInfo.referencedAtInfoList)
				{
					if (info.entries != null && info.entries.Length > 0)
					{
						dirty = true;
						info.entries = null;
					}
				}
			}
			
			if (dirty)
				SaveMap(MapPath, cachedMap);
		}

		internal static AssetInfo GetAssetInfoWithGUID(string guid, out bool canceled)
		{
			var map = cachedMap;
			canceled = false;

			if (map == null)
				map = GetUpdated(out canceled);

			return map.assets.FirstOrDefault(item => item.GUID == guid);
		}

		private static bool UpdateMap(AssetsMap map)
		{
			// ----------------------------------------
			// getting all valid assets within project
			// ----------------------------------------
			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 1 of 4", "Getting all valid assets...", 0))
			{
				Debug.LogError(Maintainer.ConstructLog("Assets Map update was canceled by user."));
				return false;
			}

			var allAssetPaths = AssetDatabase.GetAllAssetPaths();
			var validNewAssets = new List<RawAssetInfo>(allAssetPaths.Length);

			foreach (var assetPath in allAssetPaths)
			{
				/*if (assetPath.Contains(@"Default Local Group"))
				{
					Debug.Log(assetPath);
				}*/

				var kind = CSAssetTools.GetAssetKind(assetPath);
				if (kind == AssetKind.Unsupported) continue;

				if (!File.Exists(assetPath)) continue;
				if (AssetDatabase.IsValidFolder(assetPath)) continue;

				var guid = AssetDatabase.AssetPathToGUID(assetPath);
				var rawInfo = new RawAssetInfo
				{
					path = CSPathTools.EnforceSlashes(assetPath),
					guid = guid,
					kind = kind,
				};

				validNewAssets.Add(rawInfo);
			}

			// -----------------------------
			// checking existing map assets
			// -----------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map...", 0))
			{
				Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
				return false;
			}

			var count = map.assets.Count;
#if !UNITY_2020_1_OR_NEWER
			var updateStep = Math.Max(count / ProjectSettings.UpdateProgressStep, 1);
#endif
			for (var i = count - 1; i > -1; i--)
			{
#if !UNITY_2020_1_OR_NEWER
				if (i % updateStep == 0 && i != 0)
#endif
				{
					var index = count - i;
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map..." + index + "/" + count, (float) index / count))
					{
						EditorUtility.ClearProgressBar();
						Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
						return false;
					}
				}

				var assetInMap = map.assets[i];
				if (assetInMap.Exists())
				{
					validNewAssets.RemoveAll(a => a.guid == assetInMap.GUID);
					assetInMap.UpdateIfNeeded();
				}
				else
				{
					assetInMap.Clean();
					map.assets.RemoveAt(i);
				}
			}

			// ------------------------
			// dealing with new assets
			// ------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4", "Looking for new assets...", 0))
			{
				Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
				return false;
			}

			count = validNewAssets.Count;
#if !UNITY_2020_1_OR_NEWER
			updateStep = Math.Max(count / ProjectSettings.UpdateProgressStep, 1);
#endif
			for (var i = 0; i < count; i++)
			{
#if !UNITY_2020_1_OR_NEWER
				if (i % updateStep == 0 && i != 0)
#endif
				{

					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4",
						"Looking for new assets..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
						return false;
					}
				}

				var rawAssetInfo = validNewAssets[i];
				var rawAssetInfoPath = rawAssetInfo.path;

				var type = AssetDatabase.GetMainAssetTypeAtPath(rawAssetInfoPath);
				if (type == null)
				{
					var loadedAsset = AssetDatabase.LoadMainAssetAtPath(rawAssetInfoPath);
					if (loadedAsset == null)
					{
						if (rawAssetInfo.kind != AssetKind.FromPackage)
						{
							if (!CSAssetTools.IsAssetScriptableObjectWithMissingScript(rawAssetInfoPath))
							{
								Debug.LogWarning(Maintainer.ConstructLog("Can't retrieve type of the asset:\n" +
																		rawAssetInfoPath));
								continue;
							}
						}
						else
						{
							continue;
						}
					}
					else
					{
						type = loadedAsset.GetType();
					}
				}

				var settingsKind = rawAssetInfo.kind == AssetKind.Settings ? GetSettingsKind(rawAssetInfoPath) : AssetSettingsKind.Undefined;

				var asset = AssetInfo.Create(rawAssetInfo, type, settingsKind);
				map.assets.Add(asset);
			}

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links...", 0))
			{
				Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
				return false;
			}

			count = map.assets.Count;
			
#if !UNITY_2020_1_OR_NEWER
			updateStep = Math.Max(count / ProjectSettings.UpdateProgressStep, 1);
#endif
			for (var i = 0; i < count; i++)
			{
#if !UNITY_2020_1_OR_NEWER
				if (i % updateStep == 0 && i != 0)
#endif
				{
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.Log(Maintainer.ConstructLog("Assets Map update was canceled by user."));
						return false;
					}
				}

				var asset = map.assets[i];
				
				if (!asset.needToRebuildReferences) continue;

				var dependencies = asset.dependenciesGUIDs;
				if (dependencies != null && dependencies.Length > 0)
				{
					var referenceInfos = new List<AssetReferenceInfo>(asset.assetReferencesInfo);
					foreach (var mapAsset in map.assets)
					{
						var referencedAtInfos = new List<ReferencedAtAssetInfo>(mapAsset.referencedAtInfoList);
						foreach (var dependency in dependencies)
						{
							if (mapAsset.GUID != dependency) continue;
						
							// fix for recursive font dependencies, was needed for recursive references tree
							// commented out for now to fix fallback fonts treated as garbage
							// if (mapAsset.Type == asset.Type && asset.Type == CSReflectionTools.fontType) continue;

							var referencedAtInfo = new ReferencedAtAssetInfo
							{
								assetInfo = asset
							};
							referencedAtInfos.Add(referencedAtInfo);

							var referenceInfo = new AssetReferenceInfo
							{
								assetInfo = mapAsset
							};
							referenceInfos.Add(referenceInfo);
						}
						mapAsset.referencedAtInfoList = referencedAtInfos.ToArray();
					}
					asset.assetReferencesInfo = referenceInfos.ToArray();
				}
				asset.needToRebuildReferences = false;
			}

			/*Debug.Log("Total assets in map: " + map.assets.Count);
			foreach (var mapAsset in map.assets)
			{
				//if (!(mapAsset.path.Contains("frag_ab") || mapAsset.path.Contains("frag_ac"))) continue;
				if (!mapAsset.Path.Contains("NewAssembly")) continue;

				Debug.Log("==================================================\n" + mapAsset.Path + "\n" + mapAsset.Path);
				Debug.Log("[REFERENCED BY]");
				foreach (var reference in mapAsset.referencedAtInfoList)
				{
					Debug.Log(reference.assetInfo.Path);
				}

				Debug.Log("[REFERENCES]");
				foreach (var reference in mapAsset.assetReferencesInfo)
				{
					Debug.Log(reference.assetInfo.Path);
				}
			}*/

			return true;
		}

		private static AssetsMap LoadMap(string path)
		{
			if (!File.Exists(path)) return null;

			var fileSize = new FileInfo(path).Length;

			if (fileSize > 500000)
				EditorUtility.DisplayProgressBar("Loading Assets Map", "Please wait...", 0);

			AssetsMap result = null;
			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			try
			{
				result = bf.Deserialize(stream) as AssetsMap;

				if (result != null && result.version != Maintainer.Version)
				{
					result = null;
				}
			}
			catch (Exception)
			{
				Debug.Log(Maintainer.ConstructLog("Couldn't read assets map (more likely you've updated Maintainer recently).\nThis message is harmless unless repeating on every Maintainer run.")); //"\n" + ex);
			}
			finally
			{
				stream.Close();
			}

			EditorUtility.ClearProgressBar();

			return result;
		}

		private static void SaveMap(string path, AssetsMap map)
		{
			if (map.assets.Count > 10000)
			{
				EditorUtility.DisplayProgressBar("Saving Assets Map", "Please wait...", 0);
			}

			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			bf.Serialize(stream, map);
			stream.Close();

			EditorUtility.ClearProgressBar();
		}

		private static AssetSettingsKind GetSettingsKind(string assetPath)
		{
			var result = AssetSettingsKind.UnknownSettingAsset;

			var fileName = Path.GetFileNameWithoutExtension(assetPath);
			if (!string.IsNullOrEmpty(fileName))
			{
				try
				{
					result = (AssetSettingsKind)Enum.Parse(CSReflectionTools.assetSettingsKindType, fileName);
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return result;
		}
	}
}