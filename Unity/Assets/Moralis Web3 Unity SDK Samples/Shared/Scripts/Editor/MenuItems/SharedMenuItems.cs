using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using UnityEditor;
using MoralisUnity.Sdk.Constants;
using MoralisUnity.Sdk.UI.ReadMe;
using UnityEngine;
using MoralisUnity.Samples.Shared.Utilities;

namespace MoralisUnity.Samples.Shared.MenuItems
{
	/// <summary>
	/// The MenuItem attribute allows you to add menu items to the main menu and inspector context menus.
	/// <see cref="https://docs.unity3d.com/ScriptReference/MenuItem.html"/>
	/// </summary>
	public static class SharedMenuItems
	{
		//  Properties ------------------------------------

        
		//  Fields ----------------------------------------
		// This class is in 'Shared' so can't directly use TheGameConstants.cs. That is ok.
		private const string ProjectTitle = "The Game"; 
		
		//  General Methods -------------------------------
		
		///////////////////////////////////////////
		// Tools Menu
		///////////////////////////////////////////
		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + SharedConstants.OpenReadMe, false,
			SharedConstants.PriorityMoralisTools_Examples)]
		public static void OpenReadMe()
		{
			ReadMeEditor.SelectReadmeGuid("3b4d333465945474ea57ff6e62ba4f37");
		}


		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + "Add Example Scenes To Build Settings", false,
			SharedConstants.PriorityMoralisTools_Examples)]
		public static void AddAllScenesToBuildSettings()
		{
			List<SceneData> sceneDatas = SceneDataStorage.Instance.SceneDatas;

			Debug.Log($"AddAllScenesToBuildSettings() sceneAssets.Count = {sceneDatas.Count}");
			EditorBuildSettingsUtility.AddScenesToBuildSettings(sceneDatas);
		}


		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + "Load Moralis Layout (10x16)", false,
			SharedConstants.PriorityMoralisTools_Examples)]
		public static void LoadExampleLayout_10x16()
		{
			string guid = "68e09fd97bc6f3f4f9154ccdf9ece35d";
			string path = AssetDatabase.GUIDToAssetPath(guid);
			UnityReflectionUtility.UnityEditor_WindowLayout_LoadWindowLayout(path);
		}
		
		
		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + "Load Moralis Layout (16x10)", false,
			SharedConstants.PriorityMoralisTools_Examples)]
		public static void LoadExampleLayout_16x10()
		{
			string guid = "bb0830cff9fd5fa4b9ac04292dc30acc";
			string path = AssetDatabase.GUIDToAssetPath(guid);
			UnityReflectionUtility.UnityEditor_WindowLayout_LoadWindowLayout(path);
		}
		
		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + "Storage/Open Persistent Data Path", false,
			SharedConstants.PriorityMoralisTools_Examples_Sub)]
		public static void OpenApplicationPersistentDataPath()
		{
			EditorUtility.RevealInFinder(Application.persistentDataPath);
		}
		
		[MenuItem(MoralisConstants.PathMoralisSamplesWindowMenu + "/" +
		          ProjectTitle + "/" + "Storage/Open Steaming Data Path", false,
			SharedConstants.PriorityMoralisTools_Examples_Sub)]
		public static void OpenApplicationStreamingDataPath()
		{
			EditorUtility.RevealInFinder(Application.streamingAssetsPath);
		}

		
		///////////////////////////////////////////
		// Assets Menu
		///////////////////////////////////////////

		[MenuItem( SharedConstants.PathMoralisSamplesAssetsMenu + "/" + "Copy Guid", false,
			SharedConstants.PriorityMoralisAssets_Examples)]
		public static void CopyGuidToClipboard()
		{
			// Support only if exactly 1 object is selected in project window
			var objs = Selection.objects;
			if (objs.Length != 1)
			{
				return;
			}

			var obj = objs[0];
			string path = AssetDatabase.GetAssetPath(obj);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			GUIUtility.systemCopyBuffer = guid.ToString();
			Debug.Log($"CopyGuidToClipboard() success! Value '{GUIUtility.systemCopyBuffer}' copied to clipboard.");
		}
		
		
		[MenuItem( SharedConstants.PathMoralisSamplesAssetsMenu + "/" + "Copy Guid", true,
			SharedConstants.PriorityMoralisAssets_Examples)]
		public static bool CopyGuidToClipboard_ValidationFunction()
		{
			// Support only if exactly 1 object is selected in project window
			var objs = Selection.objects;
			return objs.Length == 1;
		}
	}
}
