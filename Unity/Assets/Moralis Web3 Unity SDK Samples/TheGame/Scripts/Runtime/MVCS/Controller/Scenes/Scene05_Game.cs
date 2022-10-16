using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
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
			TheGameSingleton.Instance.TheGameController.OnRPCTransferLogHistoryChanged.AddListener(OnRPCTransferLogHistoryChanged);
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
				
				if (TheGameConfiguration.Instance.MultiplayerIsAutoStart &&
				    !TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
				{
					TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
				}
			}
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
			_ui.StartAsHostButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanStartAsHost();
			_ui.JoinAsClientButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanJoinAsClient();
			_ui.ShutdownButton.IsInteractable = TheGameSingleton.Instance.TheGameController.MultiplayerCanShutdown();
			_ui.BackButton.IsInteractable = true;
		}
		
		
		
		//  Event Handlers --------------------------------
		
		private async void OnMultiplayerStateNameChanged(string debugStateName)
		{
			Debug.Log("05 State " + debugStateName);
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
		
		private async void OnRPCTransferLogHistoryChanged(PlayerView playerView)
		{
			//OPTIONAL: Optimize and only refresh if the log contains a change to the LOCAL player
			// if (transferLog.FromAddress == "my blah " ||
			//     transferLog.FromAddress == "my blah ")
			// {
			// }

			string message = "";
			try
			{
				//TODO
				//THE Unity client works
				//The Unity build has a null ref here... not sure why
				TransferLog transferLog = await TheGameSingleton.Instance.TheGameController.GetTransferLogHistoryAsync();
				message = TheGameHelper.GetTransferLogDisplayText(transferLog);
			}
			catch (Exception e)
			{
				message = e.Message;
			}
	
			_ui.TopUI.QueueSharedStatusText(message, 6000);
			
			//Update the gold/prize ui
			await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
		}
		
		private async void StartAsHostButton_OnClicked()
		{
			if (!TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerStartAsHostAsync();
		}
		
		
		private async void JoinAsClientButton_OnClicked()
		{
			if (!TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerJoinAsClientAsync();
		}
		
		private async void ShutdownButton_OnClicked()
		{
			if (TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				await TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceDisconnectAsync();
			}

			await TheGameSingleton.Instance.TheGameController.MultiplayerLeaveAsync();
		}
		
		
		private void BackButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}

	}
}
