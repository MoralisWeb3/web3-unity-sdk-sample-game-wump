using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using Unity.Netcode;
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


		//  Fields ----------------------------------------
		[Header("References (Scene)")]
		[SerializeField]
		private Scene05_GameUI _ui;
	
		private static Scene05_Game _Instance;
		private bool _isInitialized = false;


		//  Unity Methods ---------------------------------
		protected void Awake()
		{
			_Instance = this;
		}

		
		protected async void Start()
		{
			_ui.StartAsHostButton.Button.onClick.AddListener(StartAsHostButton_OnClicked);
			_ui.JoinAsClientButton.Button.onClick.AddListener(JoinAsClientButton_OnClicked);
			_ui.ShutdownButton.Button.onClick.AddListener(ShutdownButton_OnClicked);
			_ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
			//
			TheGameSingleton.Instance.TheGameController.OnPlayerAction.AddListener(OnPlayerAction);
			TheGameSingleton.Instance.TheGameController.OnRPCSharedStatusChanged.AddListener(OnRPCSharedStatusChanged);
			TheGameSingleton.Instance.TheGameController.OnRPCTransferLogChanged.AddListener(OnRPCTransferLogHistoryChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
			TheGameSingleton.Instance.TheGameController.OnMultiplayerStateNameChanged.AddListener(OnMultiplayerStateNameChanged);
			
			Initialize();
			
			RefreshUIAsync();
			bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
			RefreshUIAsync();
			
			if (!isAuthenticated)
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync( TheGameConstants.MustBeAuthenticated, 5000);
				BackButton_OnClicked();
			}
			else
			{
				bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
				RefreshUIAsync();
				
				if (!isRegistered)
				{
					await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync( TheGameConstants.MustBeRegistered, 5000);
					BackButton_OnClicked();
				}

				TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceInitialize();
				await AutoConnect();
			}
		}

		
		/// <summary>
		/// WIP
		///
		/// Do I want to aut connect without using clicking 'join'? Maybe....
		/// </summary>
		private async UniTask AutoConnect()
		{

			if (TheGameConfiguration.Instance.MultiplayerSetupServiceType == MultiplayerSetupServiceType.Full)
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}
			/*
			
				//TODO: REmove this
			if (TheGameConfiguration.Instance.MultiplayerIsAutoStart &&
			    !TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}
				
				
			// The instance playing in the Primary UNITY EDITOR will host
			// All others will NOT host
			if (ClonesManagerWrapper.HasClonesManager)
			{
				if (ClonesManagerWrapper.IsClone)
				{
					_onConnectStarted.Invoke();
					await JoinAsClient();
				}
				else
				{
					// Primary UNITY EDITOR
					_onConnectStarted.Invoke();
					await StartAsHost();
				}
			}
			else
			{
				_onConnectStarted.Invoke();
				await JoinAsClient();
			}
			*/
		}


		protected async void OnDestroy()
		{
			if (!TheGameSingleton.IsShuttingDown)
			{
				if (TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
				{
					TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceDisconnectAsync();
				}
			}
		}


		private void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}
			_isInitialized = true;
			
			if (!_isInitialized)
			{
				Debug.LogWarning($"{GetType().Namespace}.Initialize() failed. ");
			}
		}


		//  Methods ---------------------------------------
		private async UniTask RefreshUIAsync()
		{
			// Change label
			TheGameHelper.SetShutdownButtonText(_ui.ShutdownButton,
				TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected(),
				TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsHost());
			
			//
			_ui.StartAsHostButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanStartAsHost();
			_ui.JoinAsClientButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanJoinAsClient();
			_ui.ShutdownButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanShutdown();
			Debug.Log("NetworkManager.Singleton.IsHost: " + NetworkManager.Singleton.IsHost);
			Debug.Log("NetworkManager.Singleton.IsServer: " + NetworkManager.Singleton.IsServer);
			Debug.Log("NetworkManager.Singleton.IsClient: " + NetworkManager.Singleton.IsClient);
			
			Debug.Log(" MultiplayerSetupServiceIsHost:  " + 
			          TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsHost());
			Debug.Log(" MultiplayerSetupServiceIsClient:  " + 
			          TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsClient());
			_ui.BackButton.IsInteractable = true;
		}
		
		
		
		//  Event Handlers --------------------------------
		private async void StartAsHostButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

			
			if (!TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerStartAsHostAsync();
		}
		
		
		private async void JoinAsClientButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
			
			if (!TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerJoinAsClientAsync();
		}
		
		
		private async void ShutdownButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

			if (TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceDisconnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerLeaveAsync();
		}
		
		
		private void BackButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
			
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}
		
		
		private async void OnMultiplayerStateNameChanged(string debugStateName)
		{
			Debug.Log("05 State " + debugStateName);
			// Refresh for UI buttons
			await RefreshUIAsync();

			//TODO: Remove
			UniTask.Delay(1000);
			// Refresh for UI buttons
			await RefreshUIAsync();

			
		}
		
		private void OnTheGameModelChanged(TheGameModel theGameModel)
		{
		}
		
		private void OnPlayerAction(PlayerView playerView)
		{
		}
		
		private async void OnRPCSharedStatusChanged(PlayerView playerView)
		{
			if (!string.IsNullOrEmpty(playerView.SharedStatus))
			{
				_ui.TopUI.QueueSharedStatusText(playerView.SharedStatus, 6000);
			}
		}
		
		private async void OnRPCTransferLogHistoryChanged(TransferLog transferLog)
		{
			string message = TheGameHelper.GetTransferLogDisplayText(transferLog);
			_ui.TopUI.QueueSharedStatusText(message, 6000);
			
			//Update the gold/prize ui
			await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
		}
		

	}
}
