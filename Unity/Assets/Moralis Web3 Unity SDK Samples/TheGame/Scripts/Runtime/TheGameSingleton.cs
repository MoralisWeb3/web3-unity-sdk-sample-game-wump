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

		// Initialization Methods -------------------------
		public override void InstantiateCompleted()
		{
			//Set screen size for Standalone
#if UNITY_STANDALONE
			Screen.SetResolution(1600, 800, false);
			Screen.fullScreen = false;
#endif

			// Name it
			gameObject.name = GetType().Name;
			
			///////////////////////////////////
			// The Game Model
			_theGameModel = new TheGameModel();
			
			///////////////////////////////////
			// The Game View
			_theGameView = TheGameHelper.InstantiatePrefab<TheGameView>(TheGameConfiguration.Instance.TheGameViewPrefab,
				null, new Vector3(0, 0, 0));
			
			// The Details View
			DetailsView detailsView = TheGameHelper.InstantiatePrefab<DetailsView>(TheGameConfiguration.Instance.DetailsViewPrefab,
				null, new Vector3(0, 0, 0));
			
			
			// The Details View
			NetworkManagerView networkManagerView = TheGameHelper.InstantiatePrefab<NetworkManagerView>(TheGameConfiguration.Instance.NetworkManagerViewPrefab,
				null, new Vector3(0, 0, 0));	
			UnityTransport unityTransport = (UnityTransport)networkManagerView.NetworkManager.NetworkConfig.NetworkTransport;

			
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
				detailsView,
				networkManagerView,
				_theGameService,
				_multiplayerSetupService);
			
			_theGameController.Initialize();
			
		}

		// Unity Methods --------------------------------
		
		public void OnGUI()
		{
			_theGameController.OnGUI();
		}
		
		
		public void OnDestroy()
		{
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
