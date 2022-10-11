using System;
using Cysharp.Threading.Tasks;
using RMC.Shared.Managers;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

#pragma warning disable 0472, CS4014, CS1998
namespace MoralisUnity.Samples.TheGame
{
	/// <summary>
	/// Core Scene Behavior - Using <see cref="Scene05_GameUI"/>
	/// </summary>
	public class Scene05_Game : MonoBehaviour
	{
		//  Events ----------------------------------------
		
		//  Properties ------------------------------------
		
		[Obsolete("TODO: Refactor away from using an 'Instance' here")]
		public static Scene05_Game Instance
		{
			get
			{
				if (_Instance == null)
				{
					Debug.Log("TheGame, instance not found.");
				}
				return _Instance;
			}
		}

		//  Fields ----------------------------------------
		[Header("References (Project)")]
		[SerializeField] 
		private DetailsView _detailsViewPrefab;
		
		[Header("References (Scene)")]
		[SerializeField]
		private Scene05_GameUI _ui;
		
		[SerializeField] 
		private NetworkManager _networkManager;

		[Header("Configuration (General)")]
		[Tooltip("Choose. Lan = local & fast. Full = online & robust")]
		[SerializeField] 
		private MultiplayerSetupServiceType _multiplayerSetupServiceType = MultiplayerSetupServiceType.Null;

		[Header("Configuration (Lan Only)")]
		[SerializeField] 
		private bool _isLanAutoStart = true;

		private static Scene05_Game _Instance;
		private bool _isInitialized = false;


		/// <summary>
		/// Can be used to simulate poor network conditions such as:
		/// - packet delay/latency
		/// - packet jitter (variances in latency, see: https://en.wikipedia.org/wiki/Jitter)
		/// - packet drop rate (packet loss)
		/// </summary>
		[SerializeField]
		[Tooltip("Optional. Can be used to simulate poor network conditions. Zeros = No lag.")]
		private UnityTransport.SimulatorParameters _lanSimulatorParameters = new UnityTransport.SimulatorParameters
		{
			PacketDelayMS = 0,
			PacketJitterMS = 0,
			PacketDropRate = 0
		};

		private IMultiplayerSetupService _multiplayerSetupService;
		private MultiplayerSetupServiceFactory _multiplayerSetupServiceFactory =  new MultiplayerSetupServiceFactory();
		
		//  Unity Methods ---------------------------------
		protected void Awake()
		{
			_Instance = this;
		}

		
		protected async void Start()
		{
			_ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
			SelectionManager.Instance.OnSelectionChanged.AddListener(SelectionManager_OnSelectionChanged);
			Initialize();
			
			RefreshUIAsync();
			bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
			if (isAuthenticated)
			{ 
				// Populate the top UI
				bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
				RefreshUIAsync();
			}

		}
		
		private void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}

			FindOrCreateUI();
			
			_multiplayerSetupService =
				_multiplayerSetupServiceFactory.CreateMultiplayerSetupService(
					_multiplayerSetupServiceType,
					(UnityTransport)_networkManager.NetworkConfig.NetworkTransport,
					_isLanAutoStart,
					_lanSimulatorParameters);
				
			_multiplayerSetupService.OnConnectionCompleted.AddListener(
				MultiplayerSetupService_OnConnectionCompleted);
			
			_multiplayerSetupService.OnStateNameChanged.AddListener(
				MultiplayerSetupService_OnStateNameChanged);

			_multiplayerSetupService.Connect();
			
			_isInitialized = _multiplayerSetupService != null;
			
			if (!_isInitialized)
			{
				Debug.LogWarning($"{GetType().Namespace}.Initialize() failed. ");
			}
		}

		protected void OnGUI()
		{
			if (!_isInitialized) return;
			_multiplayerSetupService.OnGUI();
		}
		
		
		protected async void OnDestroy()
		{
			if (!_isInitialized) return;
			
			Debug.Log($"{this.GetType().Name} OnDestroy() {_multiplayerSetupService.IsConnected}"); 
			if (_multiplayerSetupService.IsConnected)
			{
				await _multiplayerSetupService.DisconnectAsync();
			}
		}


		//  Methods ---------------------------------------

		private async UniTask RefreshUIAsync()
		{
			_ui.BackButton.IsInteractable = true;
		}

		
		/// <summary>
		/// For some reason this NetworkBehaviour UI could not be
		/// **IN** the scene hiearchy manually. So I move it here to be created
		/// once by each client/host.
		/// </summary>
		private void FindOrCreateUI()
		{
			if (DetailsView.Instance == null)
			{
				GameObject.Instantiate(_detailsViewPrefab);
			}
		}
		
		public void RegisterPlayerView(PlayerView playerView)
		{
			//Do not wait for Initialize here
			playerView.OnIsWalkingChanged.AddListener(PlayerView_OnIsWalkingChanged);
			playerView.OnPlayerAction.AddListener(PlayerView_OnPlayerAction);
		}

		public void UnregisterPlayerView(PlayerView playerView)
		{
			//Do not wait for Initialize here
			playerView.OnIsWalkingChanged.RemoveListener(PlayerView_OnIsWalkingChanged);
		}
		
		//  Event Handlers --------------------------------
		private void BackButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}
		
		private void MultiplayerSetupService_OnConnectionCompleted(string debugMessage)
		{
			if (!_isInitialized) return;
			
			Debug.Log($"OnConnectionCompleted() {debugMessage}");
		}
		
		private void MultiplayerSetupService_OnStateNameChanged(string stateName)
		{
			if (!_isInitialized) return;
			DetailsView.Instance.LocalStatus = stateName;
			
		}
		
		private void SelectionManager_OnSelectionChanged(ISelectionManagerSelectable selection)
		{
			if (!_isInitialized) return;
			
			Debug.Log($"OnSelectionChanged() selection = {selection}");
		}

		private void PlayerView_OnIsWalkingChanged(PlayerView playerView)
		{
			if (!_isInitialized) return;
			if (!playerView.IsLocalPlayer) return;
			DetailsView.Instance.LocalStatus = playerView.IsWalking.Value ? "Walking" : "Idle";
		}
		
		private void PlayerView_OnPlayerAction()
		{
			if (!_isInitialized) return;

			DetailsView.Instance.SharedStatusUpdateRequest();
		}
		
	}
}
