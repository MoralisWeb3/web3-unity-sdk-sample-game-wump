#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer
{
	using System;
	using EditorCommon.Tools;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Entry point class with few common APIs.
	/// </summary>
	public static class Maintainer
	{
		/// <summary>
		/// Current version in X.Y.Z format (using Semantic Versioning 2.0 scheme).
		/// </summary>
		public const string Version = "1.16.0";

		internal const string LogPrefix = "<b>[Maintainer]</b> ";
		internal const string HomePage = "https://codestage.net/uas/maintainer";
		internal const string SupportContact = "https://codestage.net/contacts";
		internal const string DataLossWarning = "Make sure you've made a backup of your project before proceeding.\nAuthor is not responsible for any data loss due to use of the Maintainer!";

		private static string directory;

		/// <summary>
		/// Path to the Maintainer Directory in your project or packages.
		/// </summary>
		public static string Directory
		{
			get
			{
				if (!string.IsNullOrEmpty(directory)) return directory;

				directory = MaintainerMarker.GetAssetPath();

				if (!string.IsNullOrEmpty(directory))
				{
					if (directory.IndexOf("Scripts/MaintainerMarker.cs", StringComparison.Ordinal) >= 0)
					{
						directory = directory.Replace("Scripts/MaintainerMarker.cs", "");
					}
					else
					{
						directory = null;
						Debug.LogError(ErrorForSupport("Looks like Maintainer is placed in project incorrectly!"));
					}
				}
				else
				{
					directory = null;
					Debug.LogError(ErrorForSupport("Can't locate the Maintainer directory!"));
				}
				return directory;
			}
		}

		[InitializeOnLoadMethod]
		internal static void Init()
		{
			CSTextureLoader.LogPrefix = LogPrefix;
			CSTextureLoader.ExternalTexturesFolder = Directory;
		}
		
		internal static void PrintExceptionForSupport(Exception exception)
		{
			PrintExceptionForSupport(exception.Message, null, exception);
		}
		
		internal static void PrintExceptionForSupport(string errorText, Exception exception = null)
		{
			PrintExceptionForSupport(errorText, null, exception);
		}
		
		internal static void PrintExceptionForSupport(string errorText, string prefix = null, Exception exception = null)
		{
			Debug.LogError(ErrorForSupport(errorText, prefix, exception));
			Debug.LogException(exception);
		}
			
		internal static string ErrorForSupport(string errorText, string moduleName = null, Exception exception = null)
		{
			var prefixLog = LogPrefix + (string.IsNullOrEmpty(moduleName) ? "" : moduleName + ": ");
			var logText = string.Format("{0}{1}\n", prefixLog, errorText) +
						  string.Format("Please report at: {0}\n", SupportContact) +
						  string.Format("Also please include this information:\n{0}", GenerateBugReport(exception));

			return logText;
		}

		internal static string ConstructLog(string log, string moduleName = null)
		{
			return LogPrefix + (string.IsNullOrEmpty(moduleName) ? "" : moduleName + ": ") + log;
		}
		
		private static string GenerateBugReport(Exception exception)
		{
			var result = string.Format("Unity version: {0}\n", Application.unityVersion) +
						 string.Format("Asset version: {0}\n", Version) +
						 string.Format("Current platform: {0}\n", Application.platform.ToString()) +
						 string.Format("Target platform: {0}",
							 UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
			
			if (exception != null)
			{
				result += string.Format("\n{0}", exception);
			}
			
			return result;
		}
	}
}