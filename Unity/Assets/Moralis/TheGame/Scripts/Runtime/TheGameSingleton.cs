using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Architectures.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.Controller;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame
{

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.Build;
	class TheGameSingletonBuildProcessor : IProcessSceneWithReport 
	{
		public int callbackOrder { get { return 1;} }

		public bool DidProcessAnyScene = false;
		public void OnProcessScene(Scene scene, BuildReport report)
		{
			if (!DidProcessAnyScene)
			{
				DidProcessAnyScene = true;
				TheGameSingleton.SetUniquePlayerPrefsKey();
			}
		}
	}
#endif
	
	/// <summary>
	/// The main entry point for the whole game
	/// <para />
	/// This is a 'light' implementation of MVCS.
	/// <list type="bullet">
	///		<item>M - <see cref="TheGameModel"/> - Stores data for the game</item>
	///		<item>V - <see cref="TheGameView"/>  - Handles the UI for the game</item>
	///		<item>C - <see cref="TheGameController"/> - Handles the core functionality of the game</item>
	///		<item>S - <see cref="TheGameService"/> - Handles communication with external sources (e.g. database/servers/contracts)</item>
	/// </list>
	/// </summary>
	public class TheGameSingleton : BaseMVCS<TheGameSingleton>
	{

		// Properties -------------------------------------
		public TheGameController TheGameController  { get { return _theGameController; }}
		public TheMultiplayerController TheMultiplayerController  { get { return _theMultiplayerController; }}
		
		
		// Fields -----------------------------------------
		private TheGameModel _theGameModel;
		private TheGameView _theGameView;
		private NetworkManagerView _networkManagerView;
		private TheGameController _theGameController;
		private TheMultiplayerController _theMultiplayerController;
		private ITheGameService _theGameService;
		private IMultiplayerSetupService _multiplayerSetupService = null;

		// Initialization Methods -------------------------
		public override void InstantiateCompleted()
		{
			
#if UNITY_STANDALONE
			//Set screen size for Standalone
			Screen.SetResolution((int)TheGameConstants.ScreenResolution.x, (int)TheGameConstants.ScreenResolution.y, false);
			Screen.fullScreen = false;
			
#endif

			// Name the runtime game object
			gameObject.name = GetType().Name;
			
			///////////////////////////////////
			// The Game Model
			_theGameModel = new TheGameModel();
			
			////////////// VIEW - (PARENT UNDER SINGLETON) /////////////////////
			// The Game View 
			_theGameView = TheGameHelper.InstantiatePrefab<TheGameView>(TheGameConfiguration.Instance.TheGameViewPrefab,
				transform, new Vector3(0, 0, 0));
			SharedHelper.SafeDontDestroyOnLoad(_theGameView.gameObject);
			
			////////////// VIEW - (PARENT UNDER NUL, REQUIRED OF NETWORKOBJECTS) /////////////////////
			
			// The Network Manager View
			_networkManagerView = TheGameHelper.InstantiatePrefab<NetworkManagerView>(TheGameConfiguration.Instance.NetworkManagerViewPrefab,
				null, new Vector3(0, 0, 0));	
			SharedHelper.SafeDontDestroyOnLoad(_networkManagerView.gameObject);

			///////////////////////////////////
			// The Game Service
			TheGameServiceType theGameServiceType = 
				TheGameConfiguration.Instance.TheGameServiceType;
			
			_theGameService = new TheGameServiceFactory().
				Create(theGameServiceType);
	
			// The Multiplayer Setup Service 
			MultiplayerSetupServiceType multiplayerSetupServiceType = 
				TheGameConfiguration.Instance.MultiplayerSetupServiceType;
			_multiplayerSetupService = new MultiplayerSetupServiceFactory().CreateMultiplayerSetupService(
				multiplayerSetupServiceType,
				(UnityTransport)_networkManagerView.NetworkManager.NetworkConfig.NetworkTransport,
				TheGameConfiguration.Instance.LanSimulatorParameters);
			
			///////////////////////////////////
			// Controller
			_theGameController = new TheGameController(
				_theGameModel, 
				_theGameView,
				_theGameService);
			
			_theGameController.Initialize();
			
			_theMultiplayerController = new TheMultiplayerController(
				_theGameController,
				_multiplayerSetupService);
			
			_theMultiplayerController.Initialize();
			
		}

		/// <summary>
		///  Makes a Unique Key: UNITY EDITOR vs UNITY EDITOR CLONE vs UNITY BUILD
		///
		///   *   This **maybe** affects the uniqueness of Unity Multiplayer.
		///				-- Unique value means that instance of unity running is a 'different player in the multiplayer room'
		///   *   This **certainly** affects the uniqueness of WalletConnect
		///				-- Unique value means that instance of unity has a different Web3 Login State and Web3 Wallet Address
		///
		///  NOT REQUIRED, BUT IF YOU WANT TO BE MOST VIGILANT FOR UNIQUENESS...
		///   * Set a unique value for TheGameConfiguration.Instance.UniquePlayerPrefsSuffix for EVERY unity instance and before EACH unity build
		///   * Play with each player on a UNIQUE build.
		/// 
		/// </summary>
		public static void SetUniquePlayerPrefsKey()
		{
#if UNITY_EDITOR
			string uniquePlayerPrefsSuffix = TheGameConfiguration.Instance.UniquePlayerPrefsSuffix;
			string newProductname = TheGameConstants.GetNewProductName(uniquePlayerPrefsSuffix);
			PlayerSettings.productName = newProductname;
#endif

			//KEEP LOG FOREVER
			TheGameSingleton.Debug.LogBlueMessage($"SetUniquePlayerPrefsKey() key = {TheGameConstants.GetPlayerPrefsKeyForWeb3AndMultiplayer()}");
		}
		
		// Unity Methods --------------------------------
		
		protected override void OnDestroy()
		{
			//Debug.LogWarning("It is expected to never reach this, unless the Unity Scene stops.");
			base.OnDestroy();
			if (_theGameController == null) return;
			_theGameController.OnDestroy();
		}
		
		
		// General Methods --------------------------------

		
		// Event Handlers ---------------------------------



	}
}
