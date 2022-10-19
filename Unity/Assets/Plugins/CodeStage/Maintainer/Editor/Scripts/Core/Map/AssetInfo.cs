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
	using Dependencies;
	using UnityEditor;
	using UnityEngine;

	using Tools;

	[Serializable]
	public enum AssetKind : byte
	{
		Regular = 0,
		Settings = 10,
		FromPackage = 20,
		FromEmbeddedPackage = 30,
		Unsupported = 100
	}

	[Serializable]
	public enum AssetSettingsKind : byte
	{
		Undefined = 0,
		AudioManager = 10,
		ClusterInputManager = 20,
		DynamicsManager = 30,
		EditorBuildSettings = 40,
		EditorSettings = 50,
		GraphicsSettings = 60,
		InputManager = 70,
		NavMeshAreas = 80,
		NavMeshLayers = 90,
		NavMeshProjectSettings = 100,
		NetworkManager = 110,
		Physics2DSettings = 120,
		ProjectSettings = 130,
		PresetManager = 140,
		QualitySettings = 150,
		TagManager = 160,
		TimeManager = 170,
		UnityAdsSettings = 180,
		UnityConnectSettings = 190,
		VFXManager = 200,
		UnknownSettingAsset = 250
	}

	internal class RawAssetInfo
	{
		public string path;
		public string guid;
		public AssetKind kind;
	}

	[Serializable]
	public class AssetInfo : IEquatable<AssetInfo>
	{
		public string GUID { get; private set; }
		public string Path { get; private set; }
		public AssetKind Kind { get; private set; }
		public AssetSettingsKind SettingsKind { get; private set; }
		public Type Type { get; private set; }
		
		[field:NonSerialized]
		public long Size { get; private set; }
		
		[field:NonSerialized]
		public bool IsUntitledScene { get; private set; }

		internal string[] dependenciesGUIDs = new string[0];
		internal AssetReferenceInfo[] assetReferencesInfo = new AssetReferenceInfo[0];
		internal ReferencedAtAssetInfo[] referencedAtInfoList = new ReferencedAtAssetInfo[0];

		private ulong lastHash;

		[NonSerialized] internal bool needToRebuildReferences = true;
		[NonSerialized] private FileInfo fileInfo;
		[NonSerialized] private FileInfo metaFileInfo;
		[NonSerialized] private int[] allAssetObjects;

		internal static AssetInfo Create(RawAssetInfo rawAssetInfo, Type type, AssetSettingsKind settingsKind)
		{
			if (string.IsNullOrEmpty(rawAssetInfo.guid))
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't create AssetInfo since guid for file " + rawAssetInfo.path + " is invalid!"));
				return null;
			}

			var newAsset = new AssetInfo
			{
				GUID = rawAssetInfo.guid,
				Path = rawAssetInfo.path,
				Kind = rawAssetInfo.kind,
				Type = type,
				SettingsKind = settingsKind,
			};

			newAsset.UpdateIfNeeded();

			return newAsset;
		}

		internal static AssetInfo CreateUntitledScene()
		{
			return new AssetInfo
			{
				GUID = CSPathTools.UntitledScenePath,
				Path = CSPathTools.UntitledScenePath,
				Kind = AssetKind.Regular,
				Type = CSReflectionTools.sceneAssetType,
				IsUntitledScene = true
			};
		}

		private AssetInfo() { }

		internal bool Exists()
		{
			ActualizePath();
			
			if (fileInfo == null)
				fileInfo = new FileInfo(Path);
			else
				fileInfo.Refresh();
			
			return fileInfo.Exists;
		}

		internal void UpdateIfNeeded()
		{
			if (string.IsNullOrEmpty(Path))
			{
				Debug.LogWarning(Maintainer.ConstructLog("Can't update Asset since path is not set!"));
				return;
			}

			/*if (Path.Contains("qwerty.unity"))
			{
				Debug.Log(Path);
			}*/

			if (fileInfo == null)
				fileInfo = new FileInfo(Path);
			else
				fileInfo.Refresh();

			if (!fileInfo.Exists)
			{
				Debug.LogWarning(Maintainer.ConstructLog("Can't update asset since file at path is not found:\n" + fileInfo.FullName + "\nAsset Path: " + Path));
				return;
			}

			ulong currentHash = 0;

			if (metaFileInfo == null)
				metaFileInfo = new FileInfo(Path + ".meta");
			else
				metaFileInfo.Refresh();
			
			if (metaFileInfo.Exists)
			{
				currentHash += (ulong)metaFileInfo.LastWriteTimeUtc.Ticks;
				currentHash += (ulong)metaFileInfo.Length;
			}

			currentHash += (ulong)fileInfo.LastWriteTimeUtc.Ticks;
			currentHash += (ulong)fileInfo.Length;
			
			Size = fileInfo.Length;

			if (lastHash == currentHash)
			{
				for (var i = dependenciesGUIDs.Length - 1; i > -1; i--)
				{
					var guid = dependenciesGUIDs[i];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					path = CSPathTools.EnforceSlashes(path);
					if (!string.IsNullOrEmpty(path) && File.Exists(path)) continue;

					ArrayUtility.RemoveAt(ref dependenciesGUIDs, i);
					foreach (var referenceInfo in assetReferencesInfo)
					{
						if (referenceInfo.assetInfo.GUID != guid) continue;

						ArrayUtility.Remove(ref assetReferencesInfo, referenceInfo);
						break;
					}
				}

				if (!needToRebuildReferences) return;
			}

			foreach (var referenceInfo in assetReferencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (!info.assetInfo.Equals(this)) continue;

					ArrayUtility.Remove(ref referenceInfo.assetInfo.referencedAtInfoList, info);
					break;
				}
			}

			lastHash = currentHash;
			needToRebuildReferences = true;

			assetReferencesInfo = new AssetReferenceInfo[0];
			dependenciesGUIDs = AssetDependenciesSearcher.FindDependencies(this);
		}

		internal List<AssetInfo> GetReferencesRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencesRecursive(result, assetReferencesInfo);

			return result;
		}

		internal List<AssetInfo> GetReferencedAtRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencedAtRecursive(result, referencedAtInfoList);

			return result;
		}

		internal void Clean()
		{
			foreach (var referenceInfo in assetReferencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (!info.assetInfo.Equals(this)) 
						continue;
					ArrayUtility.Remove(ref referenceInfo.assetInfo.referencedAtInfoList, info);
					break;
				}
			}

			foreach (var referencedAtInfo in referencedAtInfoList)
			{
				foreach (var info in referencedAtInfo.assetInfo.assetReferencesInfo)
				{
					if (!info.assetInfo.Equals(this)) 
						continue;
					ArrayUtility.Remove(ref referencedAtInfo.assetInfo.assetReferencesInfo, info);
					referencedAtInfo.assetInfo.needToRebuildReferences = true;
					break;
				}
			}
		}

		internal int[] GetAllAssetObjects()
		{
			if (allAssetObjects != null) return allAssetObjects;

			var assetType = Type;
			var assetTypeName = assetType != null ? assetType.Name : null;

			if ((assetType == CSReflectionTools.fontType ||
				assetType == CSReflectionTools.texture2DType ||
				assetType == CSReflectionTools.gameObjectType ||
				assetType == CSReflectionTools.defaultAssetType && Path.EndsWith(".dll") ||
				assetTypeName == "AudioMixerController" ||
				Path.EndsWith("LightingData.asset")) &&
				assetType != CSReflectionTools.lightingDataAsset
#if UNITY_2020_1_OR_NEWER
			    && assetType != CSReflectionTools.lightingSettings
#endif
				)
			{
				var loadedObjects = AssetDatabase.LoadAllAssetsAtPath(Path);
				var referencedObjectsCandidatesList = new List<int>(loadedObjects.Length);
				foreach (var loadedObject in loadedObjects)
				{
					if (loadedObject == null) 
						continue;
					
					var instance = loadedObject.GetInstanceID();
					if (assetType == CSReflectionTools.gameObjectType)
					{
						var isComponent = loadedObject is Component;
						if (!isComponent && 
							!AssetDatabase.IsSubAsset(instance) && 
							!AssetDatabase.IsMainAsset(instance)) continue;
					}

					referencedObjectsCandidatesList.Add(instance);
				}

				allAssetObjects = referencedObjectsCandidatesList.ToArray();
			}
			else
			{
				var mainAsset = AssetDatabase.LoadMainAssetAtPath(Path);
				allAssetObjects = mainAsset != null ? 
					new[] { AssetDatabase.LoadMainAssetAtPath(Path).GetInstanceID() } : 
					new int[0];
			}

			return allAssetObjects;
		}

		private void WalkReferencesRecursive(List<AssetInfo> result, AssetReferenceInfo[] assetReferenceInfos)
		{
			foreach (var referenceInfo in assetReferenceInfos)
			{
				if (result.IndexOf(referenceInfo.assetInfo) == -1)
				{
					result.Add(referenceInfo.assetInfo);
					WalkReferencesRecursive(result, referenceInfo.assetInfo.assetReferencesInfo);
				}
			}
		}

		private void WalkReferencedAtRecursive(List<AssetInfo> result, ReferencedAtAssetInfo[] referencedAtInfos)
		{
			foreach (var referencedAtInfo in referencedAtInfos)
			{
				if (result.IndexOf(referencedAtInfo.assetInfo) == -1)
				{
					result.Add(referencedAtInfo.assetInfo);
					WalkReferencedAtRecursive(result, referencedAtInfo.assetInfo.referencedAtInfoList);
				}
			}
		}

		private void ActualizePath()
		{
			if (Kind == AssetKind.FromPackage) return;

			var actualPath = CSPathTools.EnforceSlashes(AssetDatabase.GUIDToAssetPath(GUID));
			if (!string.IsNullOrEmpty(actualPath) && actualPath != Path)
			{
				fileInfo = new FileInfo(actualPath);
				metaFileInfo = new FileInfo(actualPath + ".meta");
				Path = actualPath;
			}
		}

		public override string ToString()
		{
			var baseType = "N/A";
			if (Type != null && Type.BaseType != null)
				baseType = Type.BaseType.ToString();
			
			return "Asset Info\n" +
				   "Path: " + Path + "\n" +
				   "GUID: " + GUID + "\n" +
				   "Kind: " + Kind + "\n" +
				   "SettingsKind: " + SettingsKind + "\n" +
				   "Size: " + Size + "\n" +
				   "Type: " + Type + "\n" +
				   "Type.BaseType: " + baseType;
		}
		
		public bool Equals(AssetInfo other)
		{
			if (ReferenceEquals(null, other))
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return GUID == other.GUID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != GetType())
				return false;

			return Equals((AssetInfo)obj);
		}

		public override int GetHashCode()
		{
			return GUID != null ? GUID.GetHashCode() : 0;
		}
	}
}