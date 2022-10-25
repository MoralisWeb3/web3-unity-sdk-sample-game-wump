using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Architectures.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.Controller;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Netcode;
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
		public TheWeb3Controller TheWeb3Controller  { get { return _theWeb3Controller; }}
		public TheMultiplayerController TheMultiplayerController  { get { return _theMultiplayerController; }}
		
		
		// Fields -----------------------------------------
		private TheGameModel _theGameModel;
		private TheGameView _theGameView;
		private NetworkManagerView _networkManagerView;
		private TheGameController _theGameController;
		private TheWeb3Controller _theWeb3Controller;
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
			// The Game Models
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
			// The Services
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
			// Controllers
			
			// TheGameController
			_theGameController = new TheGameController(
				_theGameModel, 
				_theGameView);
			
			// TheWeb3Controller
			_theWeb3Controller = new TheWeb3Controller(
				_theGameModel, 
				_theGameController,
				_theGameService);
			_theWeb3Controller.Initialize();
			
			//TheGameController Init - Set later since its created later
			_theGameController.TheWeb3Controller = _theWeb3Controller; 
			_theGameController.Initialize();
			
			//  TheMultiplayerController
			_theMultiplayerController = new TheMultiplayerController(
				_networkManagerView,
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

		// Debug high level changes to networking state.
		// private string lastTempString = "";
		//
		// // Unity Methods --------------------------------
		// protected void Update()
		// {
		// 	string tempString = ("Network: c " + NetworkManager.Singleton.IsClient + " &s " + NetworkManager.Singleton.IsServer + " &g " + NetworkManager.Singleton.IsHost);
		//
		// 	if (tempString != lastTempString)
		// 	{
		// 		lastTempString = tempString;
		// 		Debug.Log(lastTempString);
		// 	}
		// }
		
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
