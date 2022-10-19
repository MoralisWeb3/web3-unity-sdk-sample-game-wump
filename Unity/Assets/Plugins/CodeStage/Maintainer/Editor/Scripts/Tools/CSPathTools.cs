#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using System.IO;
	using System.Collections.Generic;

	using UnityEngine;

	using Core;
	using UnityEditor;

	internal static class CSPathTools
	{
		public const string UntitledScenePath = "Untitled.scene";
		private const string AssetsFolderName = "/Assets";
		private static readonly string AssetsFolderPath = Application.dataPath;
		private static readonly string FullProjectPath = Path.GetFullPath(Path.Combine(AssetsFolderPath, "../"));
		private static readonly int AssetsFolderIndex = FullProjectPath.Length - 1;

		// not using FileUtil.GetProjectRelativePath here since it does not work properly
		public static string GetProjectRelativePath(string path)
		{
			if (!Path.IsPathRooted(path))
				return path;
			
			if (Path.GetFullPath(path).StartsWith(FullProjectPath))
			{
				return path.Substring(AssetsFolderIndex + 1);
			}
			
			return path;
		}
		
		public static string GetPathForMenuItem(string path)
		{
			return NicifyAssetPath(EnforceBackslashes(GetProjectRelativePath(path)));
		}

		public static string NicifyAssetPath(string path, bool trimExtension = false)
		{
			return NicifyAssetPath(path, AssetKind.Regular, trimExtension);
		}

		public static string NicifyAssetPath(string path, AssetKind kind, bool trimExtension = false)
		{
			var nicePath = path;

			if (nicePath == UntitledScenePath)
				return Path.GetFileNameWithoutExtension(nicePath);

			switch (kind)
			{
				case AssetKind.Regular:
					if (path.Length <= 7) return path;
					nicePath = nicePath.Remove(0, 7);
					break;
				case AssetKind.Settings:
				case AssetKind.FromPackage:
				case AssetKind.FromEmbeddedPackage:
				case AssetKind.Unsupported:
					break;
				default:
					throw new ArgumentOutOfRangeException("kind", kind, null);
			}

			if (trimExtension)
			{
				var lastSlash = nicePath.LastIndexOf('/');
				var lastDot = nicePath.LastIndexOf('.');

				// making sure we'll not trim path like Test/My.Test/linux_file
				if (lastDot > lastSlash)
				{
					nicePath = nicePath.Remove(lastDot, nicePath.Length - lastDot);
				}
			}

			return nicePath;
		}
		
		public static string GetParentDirectory(string path)
		{
			var dir = Directory.GetParent(path);
			if (dir == null)
				return null;

			var systemPath = dir.FullName;
			var assetDatabasePath = EnforceSlashes(GetProjectRelativePath(systemPath));
			
			return assetDatabasePath;
		}
		
		// i.e. Assets/SomeDir
		public static string GetTopAssetsDirectory(string path)
		{
			if (IsAssetsRootPath(path))
				return null;
			
			var systemPath = EnforceSlashes(Path.GetFullPath(path));
			if (!systemPath.Contains(EnforceSlashes(AssetsFolderPath))) // if path comes not from Assets
				return null;

			var nextFolderSeparator = systemPath.IndexOf('/', AssetsFolderPath.Length + 1);
			if (nextFolderSeparator == -1)
				return null;

			var result = systemPath.Substring(0, nextFolderSeparator);

			return GetProjectRelativePath(result);
		}

		public static string EnforceSlashes(string path)
		{
			return string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');
		}
		
		public static string EnforceBackslashes(string path)
		{
			return string.IsNullOrEmpty(path) ? path : path.Replace('/', '\\');
		}

		// source: UnityEditor.U2D.SpriteAtlasInspector.IsPackable(Object) : bool
		public static string[] GetAllPackableAssetsGUIDsRecursive(string folder)
		{
			return AssetDatabase.FindAssets("t:Sprite t:Texture2D", new[] { folder });
		}
		
		public static string[] GetAllDirectoriesRecursive(string folder)
		{
			var folders = new List<string>();
			GetAllDirectoriesRecursivePrivate(folder, folders);
			return folders.ToArray();
		}

		private static void GetAllDirectoriesRecursivePrivate(string parent, List<string> result)
		{
			var folders = AssetDatabase.GetSubFolders(parent);
			foreach (var folder in folders)
			{
				result.Add(folder);
				GetAllDirectoriesRecursivePrivate(folder, result);
			}
		}

		public static bool IsAssetsRootPath(string path)
		{
			return EnforceSlashes(Path.GetFullPath(path)) == EnforceSlashes(AssetsFolderPath);
		}
	}
}