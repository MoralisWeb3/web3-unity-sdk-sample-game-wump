#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#pragma warning disable 0414

namespace CodeStage.Maintainer.Settings
{
	using System;
	using System.IO;
	using UnityEngine;

	using EditorCommon.Tools;
	using UnityEditor;
	using Object = UnityEngine.Object;

	/// <summary>
	/// Main settings scriptable object.
	/// Make sure to call Save() after changing any settings to make sure changes will persist.
	/// </summary>
	/// All settings in the scriptable object are saved in ProjectSettings folder.
	[Serializable]
	public partial class ProjectSettings : ScriptableObject
	{
#if !UNITY_2020_1_OR_NEWER
		internal const int UpdateProgressStep = 10;
#endif

		private const string Directory = "ProjectSettings";
		private const string Path = Directory + "/MaintainerSettings.asset";
		private static ProjectSettings instance;

		[SerializeField]
		private IssuesFinderSettings issuesFinderSettings;

		[SerializeField]
		private ProjectCleanerSettings projectCleanerSettings;

		[SerializeField]
		private ReferencesFinderSettings referencesFinderSettings;

		[SerializeField]
		private string version = Maintainer.Version;

		private static ProjectSettings Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = LoadOrCreate();
				return instance;
			}
		}

		/// <summary>
		/// Issues Finder module settings.
		/// </summary>
		public static IssuesFinderSettings Issues
		{
			get
			{
				if (Instance.issuesFinderSettings == null)
				{
					Instance.issuesFinderSettings = new IssuesFinderSettings();
				}
				return Instance.issuesFinderSettings;
			}
		}

		/// <summary>
		/// Project Cleaner module settings.
		/// </summary>
		public static ProjectCleanerSettings Cleaner
		{
			get
			{
				if (Instance.projectCleanerSettings == null)
				{
					Instance.projectCleanerSettings = new ProjectCleanerSettings();
				}

				return Instance.projectCleanerSettings;
			}
		}

		/// <summary>
		/// References Finder module settings.
		/// </summary>
		public static ReferencesFinderSettings References
		{
			get
			{
				if (Instance.referencesFinderSettings == null)
				{
					Instance.referencesFinderSettings = new ReferencesFinderSettings();
				}
				return Instance.referencesFinderSettings;
			}
		}

		/// <summary>
		/// Call to remove all Maintainer Settings (including personal settings).
		/// </summary>
		public static void Delete()
		{
			instance = null;
			CSFileTools.DeleteFile(Path);
			UserSettings.Delete();
		}

		/// <summary>
		/// Call to save any changes in any settings.
		/// </summary>
		public static void Save()
		{
			SaveInstance(Instance);
			UserSettings.Save();
		}
		
		// for debugging purposes
		internal static void Reload()
		{
			Save();
			instance = null;
			var dummy = Instance;
		}

		private static ProjectSettings LoadOrCreate()
		{
			ProjectSettings settings;

			if (!File.Exists(Path))
			{
				settings = CreateNewSettingsFile();
			}
			else
			{
				settings = LoadInstance();

				if (settings == null)
				{
					CSFileTools.DeleteFile(Path);
					settings = CreateNewSettingsFile();
				}

				if (settings.version != Maintainer.Version)
				{
					MigrateSettings(settings);
					SearchResultsStorage.Clear();
				}
			}

			settings.hideFlags = HideFlags.HideAndDontSave;
			settings.version = Maintainer.Version;

			settings.InitAfterLoad();
			
			return settings;
		}

		private static ProjectSettings CreateNewSettingsFile()
		{
			var settingsInstance = CreateInstance();

			settingsInstance.projectCleanerSettings.SetDefaultFilters();
			SaveInstance(settingsInstance);

			return settingsInstance;
		}

		private static void SaveInstance(ProjectSettings settingsInstance)
		{
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]{settingsInstance}, Path, true);
			}
			catch (Exception e)
			{
				Maintainer.PrintExceptionForSupport("Can't save settings!", e);
			}
		}

		private static ProjectSettings LoadInstance()
		{
			ProjectSettings settingsInstance;

			try
			{
				settingsInstance = (ProjectSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.Log(Maintainer.ConstructLog("Couldn't read settings, resetting them to defaults.\n" +
												  "This is a harmless message in most cases and can be ignored.\n" + ex));
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static ProjectSettings CreateInstance()
		{
			var newInstance = CreateInstance<ProjectSettings>();
			//var newInstance = new MaintainerSettings();
			newInstance.issuesFinderSettings = new IssuesFinderSettings();
			newInstance.projectCleanerSettings = new ProjectCleanerSettings();
			newInstance.referencesFinderSettings = new ReferencesFinderSettings();
			return newInstance;
		}
		
		private void InitAfterLoad()
		{
			EditorApplication.delayCall += () => Issues.SyncDetectors();
		}
	}
}