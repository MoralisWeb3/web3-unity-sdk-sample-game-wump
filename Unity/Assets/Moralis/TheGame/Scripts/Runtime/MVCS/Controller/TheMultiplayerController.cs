using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;
using Debug = UnityEngine.Debug;

#pragma warning disable CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Controller
{
	/// <summary>
	/// Stores data for the game
	///		* See <see cref="TheGameSingleton"/> - Handles the core functionality of the game
	/// </summary>
	public class TheMultiplayerController : IInitializable
	{
		// Events -----------------------------------------
		
		[HideInInspector]
		public readonly StringUnityEvent OnMultiplayerStateNameChanged = new StringUnityEvent();

			
		// Properties -------------------------------------
		public bool IsInitialized { get; private set; }
		public bool IsVisibleRuntimeNetStatsMonitor {
			get
			{
				return _networkManagerView.RuntimeNetStatsMonitor.Visible;
			}
			set
			{
				_networkManagerView.RuntimeNetStatsMonitor.Visible = value;
			}
		}

		// Fields -----------------------------------------
		private readonly TheGameController _theGameController = null;
		private readonly IMultiplayerSetupService _multiplayerSetupService = null;
		private NetworkManagerView _networkManagerView;
		
		// Wait, So click sound is audible before scene changes
		private const int DelayLoadSceneMilliseconds = 100;
		
		// Wait, So click sound is audible before scene changes
		private const int ExtraDelayAfterTransferMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheMultiplayerController(
			NetworkManagerView networkManagerView,
			TheGameController theGameController,
			IMultiplayerSetupService multiplayerSetupService)
		{
			_networkManagerView = networkManagerView;
			_theGameController = theGameController;
			_multiplayerSetupService = multiplayerSetupService;
			
		}

		public void Initialize()
		{
			if (IsInitialized) return;
			
			// MODEL
			
			// VIEW
			
			// SELECTION

			//
			IsInitialized = true;
		}

		
		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
		
		// Unity Methods --------------------------------
		public async void OnDestroy()
		{
			if (!IsInitialized) return;
			
			Debug.Log($"{this.GetType().Name} OnDestroy() {_multiplayerSetupService.IsConnected}"); 
			if (IsConnected())
			{
				await DisconnectAsync();
			}
		}

		// General Methods --------------------------------

		/// <summary>
		/// Prepare system and show OnGUI menu (Connect, ect...)
		/// </summary>
		public void MultiplayerSetupServiceInitialize()
		{
			_multiplayerSetupService.Initialize();
		}
		
		/// <summary>
		/// Connect to backend of Multiplayer system and show OnGUI menu (disconnect, ect...)
		/// </summary>
		public async UniTask ConnectAsync()
		{
			if (_multiplayerSetupService.IsInitialized && !_multiplayerSetupService.IsConnected)
			{
				//
				_multiplayerSetupService.OnConnectStarted.RemoveListener(MultiplayerSetupService_OnConnectStarted);
				_multiplayerSetupService.OnConnectCompleted.RemoveListener( MultiplayerSetupService_OnConnectCompleted);
				_multiplayerSetupService.OnDisconnectStarted.RemoveListener(MultiplayerSetupService_OnDisconnectStarted);
				_multiplayerSetupService.OnDisconnectCompleted.RemoveListener(MultiplayerSetupService_OnDisconnectCompleted);
				_multiplayerSetupService.OnStateNameForDebuggingChanged.RemoveListener( MultiplayerSetupService_OnStateNameChanged);
				//
				_multiplayerSetupService.OnConnectStarted.AddListener(MultiplayerSetupService_OnConnectStarted);
				_multiplayerSetupService.OnConnectCompleted.AddListener( MultiplayerSetupService_OnConnectCompleted);
				_multiplayerSetupService.OnDisconnectStarted.AddListener(MultiplayerSetupService_OnDisconnectStarted);
				_multiplayerSetupService.OnDisconnectCompleted.AddListener(MultiplayerSetupService_OnDisconnectCompleted);
				_multiplayerSetupService.OnStateNameForDebuggingChanged.AddListener( MultiplayerSetupService_OnStateNameChanged);
				await _multiplayerSetupService.Connect();
			}
		}

		
		public bool CanStartAsHost()
		{
			return _multiplayerSetupService.CanStartAsHost();
		}
		
		
		public bool CanJoinAsClient()
		{
			return _multiplayerSetupService.CanJoinAsClient();
		}
		
		
		public bool CanShutdown()
		{
			return _multiplayerSetupService.CanShutdown();
		}

		public bool CanToggleStatsButton()
		{
			return _multiplayerSetupService.CanToggleStatsButton();
		}
		
		public bool IsVisibleToggleStatsButton()
		{
			// As of October 23, 2022...
			// The https://docs-multiplayer.unity3d.com/tools/current/RNSM has BUILD errors
			// When included in a build. So its disabled and excluded per functionality
			// This IsVisibleToggleStatsButton bool does not change functionality, but changes visibility.
			return Application.isEditor;
		}
		
		public async UniTask StartAsHostAsync()
		{
			await _multiplayerSetupService.StartAsHost();
		}
		
		
		public async UniTask JoinAsClientAsync()
		{
			await _multiplayerSetupService.JoinAsClient();
		}
		
		
		public async UniTask LeaveAsync()
		{
			await _multiplayerSetupService.Shutdown();
		}

	
		public bool IsConnected()
		{
			return _multiplayerSetupService.IsConnected;
		}
		
		public bool IsHost()
		{
			return _multiplayerSetupService.IsHost;
		}
		
		public bool MultiplayerSetupServiceIsClient()
		{
			return _multiplayerSetupService.IsClient;
		}
		
		public async UniTask DisconnectAsync()
		{
			if (_multiplayerSetupService.IsConnected)
			{
				await _multiplayerSetupService.DisconnectAsync();
			}
		}
		
		private async void MultiplayerSetupService_OnConnectStarted()
		{
			Debug.Log($"OnConnectionStarted() ");
			await _theGameController.ShowMessageWithDelayAsync(TheGameConstants.MultiplayerConnecting, 2000);
		}
			
		
		private async void MultiplayerSetupService_OnConnectCompleted(string debugMessage)
		{
			await UniTask.Delay(1000);
			_theGameController.UpdateMessageDuringMethod(TheGameConstants.MultiplayerConnected);
			await UniTask.Delay(1000);
			_theGameController.HideMessageDuringMethod(true);
		}

		
		private async void MultiplayerSetupService_OnDisconnectStarted()
		{
			Debug.Log($"MultiplayerSetupService_OnDisconnectStarted() ");
			await _theGameController.ShowMessageWithDelayAsync(TheGameConstants.MultiplayerDisconnecting, 2000);
		}
		
		
		private async void MultiplayerSetupService_OnDisconnectCompleted()
		{
			Debug.Log($"MultiplayerSetupService_OnDisconnectionCompleted() ");
			_theGameController.UpdateMessageDuringMethod(TheGameConstants.MultiplayerDisconnected);
			await UniTask.Delay(1000);
			_theGameController.HideMessageDuringMethod(true);
		}
		
		
		private void MultiplayerSetupService_OnStateNameChanged(string stateName)
		{
			OnMultiplayerStateNameChanged.Invoke(stateName);
		}


	}
}
