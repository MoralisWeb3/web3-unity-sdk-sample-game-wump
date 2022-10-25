using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
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
	
		private bool _isInitialized = false;


		//  Unity Methods ---------------------------------
		
		protected async void Start()
		{
			_ui.StartAsHostButton.Button.onClick.AddListener(StartAsHostButton_OnClicked);
			_ui.JoinAsClientButton.Button.onClick.AddListener(JoinAsClientButton_OnClicked);
			_ui.ShutdownButton.Button.onClick.AddListener( () => ShutdownButton_OnClicked());
			_ui.ToggleStatsButton.Button.onClick.AddListener(ToggleStatsButton_OnClicked);
			_ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
			//
			TheGameSingleton.Instance.TheGameController.OnPlayerAction.AddListener(OnPlayerAction);
			TheGameSingleton.Instance.TheGameController.OnRPCSharedStatusChanged.AddListener(OnRPCSharedStatusChanged);
			TheGameSingleton.Instance.TheGameController.OnRPCTransferLogChanged.AddListener(OnRPCTransferLogHistoryChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
			TheGameSingleton.Instance.TheMultiplayerController.OnMultiplayerStateNameChanged.AddListener(OnMultiplayerStateNameChanged);
			
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
				bool isRegistered = await TheGameSingleton.Instance.TheWeb3Controller.GetIsRegisteredAndUpdateModelAsync();
				RefreshUIAsync();
				
				if (!isRegistered)
				{
					await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync( TheGameConstants.MustBeRegistered, 5000);
					BackButton_OnClicked();
				}

				TheGameSingleton.Instance.TheMultiplayerController.MultiplayerSetupServiceInitialize();
				await AutoConnect();
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

		
		protected async void OnDestroy()
		{
			TheGameSingleton.Instance.TheGameController.OnPlayerAction.RemoveListener(OnPlayerAction);
			TheGameSingleton.Instance.TheGameController.OnRPCSharedStatusChanged.RemoveListener(OnRPCSharedStatusChanged);
			TheGameSingleton.Instance.TheGameController.OnRPCTransferLogChanged.RemoveListener( OnRPCTransferLogHistoryChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.RemoveListener(OnTheGameModelChanged);
		
			TheGameSingleton.Instance.TheMultiplayerController.OnMultiplayerStateNameChanged.RemoveListener( OnMultiplayerStateNameChanged);

			if (!TheGameSingleton.IsShuttingDown)
			{
				if (TheGameSingleton.Instance.TheMultiplayerController.IsConnected())
				{
					TheGameSingleton.Instance.TheMultiplayerController.DisconnectAsync();
				}
			}
		}

		
		//  Methods ---------------------------------------
		
		/// <summary>
		/// WIP
		///
		/// Do I want to aut connect without using clicking 'join'? Maybe....
		/// </summary>
		private async UniTask AutoConnect()
		{
			if (!TheGameSingleton.Instance.TheMultiplayerController.IsConnected())
			{
				await TheGameSingleton.Instance.TheMultiplayerController.ConnectAsync();
			}
		}

		private async UniTask RefreshUIAsync()
		{
			// Change label
			TheGameHelper.SetShutdownButtonText(_ui.ShutdownButton,
				TheGameSingleton.Instance.TheMultiplayerController.IsConnected(),
				TheGameSingleton.Instance.TheMultiplayerController.IsHost());
			
			//Special situation: See comment in IsVisibleToggleStatsButton
			_ui.ToggleStatsButton.IsVisible = TheGameSingleton.Instance.TheMultiplayerController.IsVisibleToggleStatsButton();
			
			//
			_ui.StartAsHostButton.IsInteractable = TheGameSingleton.Instance.TheMultiplayerController.CanStartAsHost();
			_ui.JoinAsClientButton.IsInteractable = TheGameSingleton.Instance.TheMultiplayerController.CanJoinAsClient();
			_ui.ShutdownButton.IsInteractable = TheGameSingleton.Instance.TheMultiplayerController.CanShutdown();
			_ui.ToggleStatsButton.IsInteractable = TheGameSingleton.Instance.TheMultiplayerController.CanToggleStatsButton();
			_ui.BackButton.IsInteractable = true;
		}
		
		
		
		//  Event Handlers --------------------------------
		private async void StartAsHostButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

			if (!TheGameSingleton.Instance.TheMultiplayerController.IsConnected())
			{
				await TheGameSingleton.Instance.TheMultiplayerController.ConnectAsync();
			}

			await TheGameSingleton.Instance.TheMultiplayerController.StartAsHostAsync();
		}
		
		
		private async void JoinAsClientButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
			
			if (!TheGameSingleton.Instance.TheMultiplayerController.IsConnected())
			{
				await TheGameSingleton.Instance.TheMultiplayerController.ConnectAsync();
			}

			await TheGameSingleton.Instance.TheMultiplayerController.JoinAsClientAsync();
		}
		
		
		private async void ToggleStatsButton_OnClicked()
		{
			if (TheGameSingleton.Instance.TheMultiplayerController.CanToggleStatsButton())
			{
				TheGameSingleton.Instance.TheMultiplayerController.IsVisibleRuntimeNetStatsMonitor =
					!TheGameSingleton.Instance.TheMultiplayerController.IsVisibleRuntimeNetStatsMonitor;
			}
		}	
		
		private async UniTask ShutdownButton_OnClicked()
		{
			if (TheGameSingleton.Instance.TheMultiplayerController.IsVisibleRuntimeNetStatsMonitor)
			{
				TheGameSingleton.Instance.TheMultiplayerController.IsVisibleRuntimeNetStatsMonitor = false;
			}
			
			if (TheGameSingleton.Instance.TheMultiplayerController.CanShutdown())
			{
				TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

				if (TheGameSingleton.Instance.TheMultiplayerController.IsConnected())
				{
					await TheGameSingleton.Instance.TheMultiplayerController.DisconnectAsync();
				}

				await TheGameSingleton.Instance.TheMultiplayerController.LeaveAsync();
			}
		}
		
		
		private async void BackButton_OnClicked()
		{
			//Mimic "Leave Multiplayer" button click too
			await ShutdownButton_OnClicked();
			
			TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}
		
		
		private async void OnMultiplayerStateNameChanged(string debugStateName)
		{
			//Debug.Log("Scene OnMultiplayerStateNameChanged() state = " + debugStateName);
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
			await TheGameSingleton.Instance.TheWeb3Controller.GetIsRegisteredAndUpdateModelAsync();
		}
	}
}
