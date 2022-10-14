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

#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame
{

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
		private TheGameModel TheGameModel  { get { return _theGameModel; }}
		private TheGameView TheGameView  { get { return _theGameView; }}
		public TheGameController TheGameController  { get { return _theGameController; }}
		private ITheGameService TheGameService  { get { return _theGameService; }}
		
		
		// Fields -----------------------------------------
		private TheGameModel _theGameModel;
		private TheGameView _theGameView;
		private TheGameController _theGameController;
		private ITheGameService _theGameService;
		private IMultiplayerSetupService _multiplayerSetupService = null;
		private NetworkManagerView _networkManagerView;
		private bool _hadController;

		// Initialization Methods -------------------------
		public override void InstantiateCompleted()
		{
			
#if UNITY_STANDALONE
			//Set screen size for Standalone
			Screen.SetResolution((int)TheGameConstants.ScreenResolution.x, (int)TheGameConstants.ScreenResolution.y, false);
			Screen.fullScreen = false;
			
#endif
			
#if UNITY_EDITOR
			//Set the product name inside the editor. 
			//This value is use by both moralis and the unity FULL Multiplayer for 'am I unique'
			
			//Do this "!=" check to prevent issues
			if (PlayerSettings.productName != TheGameConstants.GetNewProductName())
			{
				PlayerSettings.productName = TheGameConstants.GetNewProductName();
			}
			UnityEngine.Debug.Log($"PlayerSettings.productName={PlayerSettings.productName}");
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
			
			UnityTransport unityTransport = (UnityTransport)_networkManagerView.NetworkManager.NetworkConfig.NetworkTransport;

			
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
				unityTransport,
				TheGameConfiguration.Instance.LanSimulatorParameters);
			
			///////////////////////////////////
			// Controller
			_theGameController = new TheGameController(
				_theGameModel, 
				_theGameView,
				_networkManagerView,
				_theGameService,
				_multiplayerSetupService);
			
			_theGameController.Initialize();
			
		}

		// Unity Methods --------------------------------
		
		public void Update()
		{
			bool hasController = _theGameController != null;
			if (hasController != _hadController)
			{
				UnityEngine.Debug.Log($"1 [[[[[[[[[HadCCONT goes from {_hadController} to {hasController}");
			}
			
			_hadController = _theGameController != null;
		}
		
		public void OnGUI()
		{
			bool hasController = _theGameController != null;
			if (hasController != _hadController)
			{
				UnityEngine.Debug.Log($"2 [[[[[[[[[ HadCCONT goes from {_hadController} to {hasController}");
			}
			
			_hadController = _theGameController != null;

			if (hasController)
			{
				_theGameController.OnGUI();
			}
			
		}
		
		protected override void OnDestroy()
		{
			Debug.Log("NEVER BE IN HEre - unless unity is stopping");
			base.OnDestroy();
			if (_theGameController == null) return;
			_theGameController.OnDestroy();
		}
		
		
		// General Methods --------------------------------
		public bool WasActiveSceneLoadedDirectly()
		{
			return _theGameView.SceneManagerComponent.WasActiveSceneLoadedDirectly();
		}
		
		// Event Handlers ---------------------------------



	}
}
