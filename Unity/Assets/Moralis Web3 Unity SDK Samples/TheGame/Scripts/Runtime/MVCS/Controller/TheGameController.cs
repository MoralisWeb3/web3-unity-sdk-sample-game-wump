using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Components;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using RMC.Shared.Managers;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;

#pragma warning disable CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Controller
{
	/// <summary>
	/// Stores data for the game
	///		* See <see cref="TheGameSingleton"/> - Handles the core functionality of the game
	/// </summary>
	public class TheGameController : IInitializable
	{
		// Events -----------------------------------------
		
		/// <summary>
		/// Used to force a refresh when a UI arrives 'too late' and wants a fresh copy of the model
		/// </summary>
		public void OnTheGameModelChangedRefresh() { TheGameModel_OnTheGameModelChanged(_theGameModel); }
		
		[HideInInspector]
		public readonly TheGameModelUnityEvent OnTheGameModelChanged = new TheGameModelUnityEvent();
		
		[HideInInspector]
		public readonly PlayerViewUnityEvent OnPlayerAction = new PlayerViewUnityEvent();
		
		[HideInInspector]
		public readonly PlayerViewUnityEvent OnRPCSharedStatusChanged = new PlayerViewUnityEvent();

		[HideInInspector]
		public readonly TransferLogUnityEvent OnRPCTransferLogChanged = new TransferLogUnityEvent();

		[HideInInspector]
		public readonly StringUnityEvent OnMultiplayerStateNameChanged = new StringUnityEvent();

		
			
		// Properties -------------------------------------
		public PendingMessage PendingMessageForDeletion { get { return _theGameService.PendingMessageActive; } }
		public PendingMessage PendingMessageForSave { get { return _theGameService.PendingMessagePassive; } }
		public bool IsInitialized { get; private set; }

		// Fields -----------------------------------------
		private readonly TheGameModel _theGameModel = null;
		private readonly TheGameView _theGameView = null;
		private readonly NetworkManagerView _networkManagerView = null;
		private readonly ITheGameService _theGameService = null;
		private readonly IMultiplayerSetupService _multiplayerSetupService = null;
		
		// Wait, So click sound is audible before scene changes
		private const int DelayLoadSceneMilliseconds = 100;
		
		// Wait, So click sound is audible before scene changes
		private const int ExtraDelayAfterTransferMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheGameController(
			TheGameModel theGameModel,
			TheGameView theGameView,
			NetworkManagerView networkManagerView,
			ITheGameService theGameService,
			IMultiplayerSetupService multiplayerSetupService)
		{
			_theGameModel = theGameModel;
			_theGameView = theGameView;
			_networkManagerView = networkManagerView;
			_theGameService = theGameService;
			_multiplayerSetupService = multiplayerSetupService;
			
		}

		public void Initialize()
		{
			if (IsInitialized) return;
			
			// MODEL
			_theGameModel.OnTheGameModelChanged.AddListener(TheGameModel_OnTheGameModelChanged);
			_theGameModel.ResetAllData();
			
			// VIEW
			_theGameView.SceneManagerComponent.Initialize(TheGameConfiguration.Instance.SceneTransition, _theGameView.ImageAndCanvasView);
			_theGameView.SceneManagerComponent.OnSceneLoadingEvent.AddListener(SceneManagerComponent_OnSceneLoadingEvent);
			_theGameView.SceneManagerComponent.OnSceneLoadedEvent.AddListener(SceneManagerComponent_OnSceneLoadedEvent);
			
			// SELECTION
			SelectionManager.Instance.OnSelectionChanged.AddListener(SelectionManager_OnSelectionChanged);

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
			
			SelectionManager.Instance.OnSelectionChanged.RemoveListener(SelectionManager_OnSelectionChanged);

			Debug.Log($"{this.GetType().Name} OnDestroy() {_multiplayerSetupService.IsConnected}"); 
			if (_multiplayerSetupService.IsConnected)
			{
				await _multiplayerSetupService.DisconnectAsync();
			}
	

		}

		// General Methods --------------------------------
		
		///////////////////////////////////////////
		// Related To: MyMoralisWrapper
		///////////////////////////////////////////
		public async UniTask<bool> GetIsAuthenticatedAndUpdateModelAsync()
		{
			bool isAuthenticated =  await CustomWeb3System.Instance.IsAuthenticatedAsync();
			_theGameModel.IsAuthenticated.Value = isAuthenticated;
			return _theGameModel.IsAuthenticated.Value;
		}
		
		
		public async UniTask<string> GetWeb3UserAddressAsync()
		{
			return await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
		}

		///////////////////////////////////////////
		// Related To: Model
		///////////////////////////////////////////


		///////////////////////////////////////////
		// Related To: Service
		///////////////////////////////////////////

		// GETTER Methods -------------------------
		
		/// <summary>
		/// The checks "IsRegistered" *AND* if true, it loads
		/// some other data.
		///
		/// TODO: Does this method load too many things?
		/// </summary>
		/// <returns></returns>
		public async UniTask<bool> GetIsRegisteredAndUpdateModelAsync()
		{
			// Call Service. Sync Model
			bool isRegistered = await _theGameService.GetIsRegisteredAsync();
			_theGameModel.IsRegistered.Value = isRegistered;

			//TODO: Perhaps pass a parameter of useModelCache to bypass these checks EVERY time?
			if (_theGameModel.IsRegistered.Value)
			{
				await GetGoldAndUpdateModelAsync();
				await GetPrizesAndUpdateModelAsync();
				
				if (!_theGameModel.CustomPlayerInfo.Value.HasWeb3Address)
				{
					string web3Address = await GetWeb3UserAddressAsync();
					SetPlayerWeb3AddressAndUpdateModel(web3Address);
				}
				
				if (!_theGameModel.CustomPlayerInfo.Value.HasNickname)
				{
					RandomizeNicknameAndUpdateModel();
				}
			}
			
			return _theGameModel.IsRegistered.Value;
		}

		
		public async UniTask<int> GetGoldAndUpdateModelAsync()
		{
			int gold = await _theGameService.GetGoldAsync();
			_theGameModel.Gold.Value = gold;
			return _theGameModel.Gold.Value;
		}
		
		
		public async UniTask<List<Prize>> GetPrizesAndUpdateModelAsync()
		{
			List<Prize> prizes = await _theGameService.GetPrizesAsync();
			_theGameModel.Prizes.Value = prizes;
			return _theGameModel.Prizes.Value;
		}
		
		
		public async UniTask<TransferLog> GetTransferLogHistoryAsync()
		{
			TransferLog result = await _theGameService.GetTransferLogHistoryAsync();
			return result;
		}
		
		
		public void SetPlayerNicknameAndUpdateModel(string nickname)
		{
			// Set COMPLETE object to properly trigger events
			CustomPlayerInfo customPlayerInfo = _theGameModel.CustomPlayerInfo.Value;
			customPlayerInfo.Nickname = nickname;
			_theGameModel.CustomPlayerInfo.Value = customPlayerInfo;
		}
		
		
		public void SetPlayerWeb3AddressAndUpdateModel(string web3address)
		{
			// Set COMPLETE object to properly trigger events
			CustomPlayerInfo customPlayerInfo = _theGameModel.CustomPlayerInfo.Value;
			customPlayerInfo.Web3Address = web3address;
			_theGameModel.CustomPlayerInfo.Value = customPlayerInfo;
		}
		
		
		public void RandomizeNicknameAndUpdateModel()
		{
			string nickname = TheGameHelper.GetRandomizedNickname();
			SetPlayerNicknameAndUpdateModel(nickname);
		}

		
		// SETTER Methods -------------------------
		public async UniTask RegisterAndUpdateModelAsync()
		{
			_theGameModel.ResetAllData();
			await _theGameService.RegisterAsync();
			_theGameModel.IsRegistered.Value = await GetIsRegisteredAndUpdateModelAsync();
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync(); // Delay 1 of 2
			await DelayExtraAfterStateChangeAsync(); // Delay 2 of 2 -- yes this is a long delay. Apparently needed for this state change
		}

		
		public async UniTask UnregisterAsync()
		{
			_theGameModel.ResetAllData();
			await _theGameService.UnregisterAsync();
			_theGameModel.IsRegistered.Value = await GetIsRegisteredAndUpdateModelAsync();

			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
	
		}


		public async UniTask TransferGoldAsync(string toAddress)
		{
			await _theGameService.TransferGoldAsync(toAddress);
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
			await UniTask.Delay(ExtraDelayAfterTransferMilliseconds);
		}

		
		public async UniTask TransferPrizeAsync(string toAddress, Prize prize)
		{
			if (prize == null)
			{
				throw new ArgumentException("TransferPrizeAsync() failed. prize = {prize}");
			}
			await _theGameService.TransferPrizeAsync(toAddress, prize);
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
			await UniTask.Delay(ExtraDelayAfterTransferMilliseconds);
		}
		
		
		public async UniTask SafeReregisterDeleteAllPrizesAsync()
		{
			_theGameModel.ResetAllData();
			await _theGameService.SafeReregisterDeleteAllPrizesAsync();
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
			
			// Refresh the UI
			await GetIsRegisteredAndUpdateModelAsync();
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
		}
		
		
		///////////////////////////////////////////
		// Related To: Networking
		///////////////////////////////////////////

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
		public async UniTask MultiplayerSetupServiceConnectAsync()
		{
			if (_multiplayerSetupService.IsInitialized && !_multiplayerSetupService.IsConnected)
			{
				_multiplayerSetupService.OnConnectStarted.AddListener(MultiplayerSetupService_OnConnectStarted);
				_multiplayerSetupService.OnConnectCompleted.AddListener( MultiplayerSetupService_OnConnectCompleted);
				_multiplayerSetupService.OnDisconnectStarted.AddListener(MultiplayerSetupService_OnDisconnectStarted);
				_multiplayerSetupService.OnDisconnectCompleted.AddListener(MultiplayerSetupService_OnDisconnectCompleted);
				_multiplayerSetupService.OnStateNameForDebuggingChanged.AddListener( MultiplayerSetupService_OnStateNameChanged);
				await _multiplayerSetupService.Connect();
			}
		}

		
		public bool MultiplayerCanStartAsHost()
		{
			return _multiplayerSetupService.CanStartAsHost();
		}
		
		
		public bool MultiplayerCanJoinAsClient()
		{
			return _multiplayerSetupService.CanJoinAsClient();
		}
		
		
		public bool MultiplayerCanShutdown()
		{
			return _multiplayerSetupService.CanShutdown();
		}

		
		public async UniTask MultiplayerStartAsHostAsync()
		{
			await _multiplayerSetupService.StartAsHost();
		}
		
		
		public async UniTask MultiplayerJoinAsClientAsync()
		{
			await _multiplayerSetupService.JoinAsClient();
		}
		
		
		public async UniTask MultiplayerLeaveAsync()
		{
			await _multiplayerSetupService.Shutdown();
		}

	
		public bool MultiplayerSetupServiceIsConnected()
		{
			return _multiplayerSetupService.IsConnected;
		}
		
		public bool MultiplayerSetupServiceIsHost()
		{
			return _multiplayerSetupService.IsHost;
		}
		
		public async UniTask MultiplayerSetupServiceDisconnectAsync()
		{
			if (_multiplayerSetupService.IsConnected)
			{
				await _multiplayerSetupService.DisconnectAsync();
			}
		}
		
		
		private void PlayerView_OnIsWalkingChanged(PlayerView playerView)
		{
			if (!playerView.IsLocalPlayer) return;
			string status = playerView.IsWalking.Value ? "Walking" : "Idle";
		}
		
		
		private void PlayerView_OnPlayerAction(PlayerView playerView)
		{
			//Event Forwarding To External Scope
			OnPlayerAction.Invoke(playerView);
		}
		
		
		private void PlayerView_OnRPCSharedStatusChanged(PlayerView playerView)
		{
			//Event Forwarding To External Scope
			OnRPCSharedStatusChanged.Invoke(playerView);
		}
		
		
		private void PlayerView_OnRPCTransferLogChanged(TransferLog transferLog)
		{
			//Event Forwarding To External Scope
			OnRPCTransferLogChanged.Invoke(transferLog);
		}
		
		
		public void RegisterView(IRegisterableView registerableView)
		{
			// Check TYPE of class
			switch (registerableView)
			{
				case PlayerView playerView:
					playerView.OnIsWalkingChanged.AddListener(PlayerView_OnIsWalkingChanged);
					playerView.OnPlayerAction.AddListener(PlayerView_OnPlayerAction);
					playerView.OnRPCSharedStatusChanged.AddListener(PlayerView_OnRPCSharedStatusChanged);
					playerView.OnRPCTransferLogChanged.AddListener(PlayerView_OnRPCTransferLogChanged);
					break;
				case TransferDialogView transferDialogView:
					transferDialogView.OnTransferGoldRequested.AddListener(TransferDialogView_OnTransferGoldRequested);
					transferDialogView.OnTransferPrizeRequested.AddListener(TransferDialogView_OnTransferPrizeRequested);
					transferDialogView.OnTransferCancelRequested.AddListener(TransferDialogView_OnTransferCancelRequested);
					break;
				default:
					SwitchDefaultException.Throw(registerableView);
					break;
			}
		}


		public void UnregisterView(IRegisterableView registerableView)
		{
			// Check TYPE of class
			switch (registerableView)
			{
				case PlayerView playerView:
					playerView.OnIsWalkingChanged.RemoveListener(PlayerView_OnIsWalkingChanged);
					break;
				case TransferDialogView transferDialogView:
					transferDialogView.OnTransferGoldRequested.RemoveListener(TransferDialogView_OnTransferGoldRequested);
					transferDialogView.OnTransferPrizeRequested.RemoveListener(TransferDialogView_OnTransferPrizeRequested);
					transferDialogView.OnTransferCancelRequested.RemoveListener(TransferDialogView_OnTransferCancelRequested);
					break;
				default:
					SwitchDefaultException.Throw(registerableView);
					break;
			}
		}
		
		
		public bool CanTransferGoldToSelected()
		{
			if (_theGameModel.HasSelectedPlayerView)
			{
				if (_theGameModel.SelectedPlayerView.Value.Web3Address !=
				    _theGameModel.CustomPlayerInfo.Value.Web3Address)
				{
					if (_theGameModel.Gold.Value >= TheGameConstants.GoldOnTransfer)
					{
						return true;
					}
					else
					{
						//Debug.LogWarning("CanTransfer() failed. Nothing to transfer");
					}
				}
				else
				{
					//Debug.LogWarning("CanTransfer() failed. The from/to cannot be identical");
				}
		
			}
			else
			{
				//Debug.LogWarning("CanTransfer() failed. No one is selected");
			}

			return false;
		}
		

		public bool CanTransferPrizeToSelected()
		{
			if (_theGameModel.HasSelectedPlayerView)
			{
				if (_theGameModel.SelectedPlayerView.Value.Web3Address !=
				    _theGameModel.CustomPlayerInfo.Value.Web3Address)
				{
					if (_theGameModel.Prizes.Value.Count >= TheGameConstants.PrizesOnTransfer)
					{
						return true;
					}
					else
					{
						//Debug.LogWarning("CanTransfer() failed. Nothing to transfer");
					}
				}
				else
				{
					//Debug.LogWarning("CanTransfer() failed. The from/to cannot be identical");
				}
			}
			else
			{
				//Debug.LogWarning("CanTransfer() failed. No one is selected");
			}

			return false;
		}
		
		
		private async void TransferDialogView_OnTransferPrizeRequested(TransferDialogView transferDialogView)
		{
			if (CanTransferPrizeToSelected())
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
					TheGameConstants.TransferingPrize,
					async delegate ()
					{
						_theGameModel.IsTransferPending.Value = true;
						await TransferPrizeAsync(_theGameModel.SelectedPlayerView.Value.Web3Address, 
							_theGameModel.Prizes.Value[0]);

						// Update client UI
						await GetPrizesAndUpdateModelAsync();
						
						//Finish the transfer by mimicking the cancel button
						TransferDialogView_OnTransferCancelRequested(transferDialogView);
					});
			}
		}
		

		private async void TransferDialogView_OnTransferGoldRequested (TransferDialogView transferDialogView)
		{
			if (CanTransferGoldToSelected())
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
					TheGameConstants.TransferingGold,
					async delegate ()
					{
						_theGameModel.IsTransferPending.Value = true;
						await TransferGoldAsync(_theGameModel.SelectedPlayerView.Value.Web3Address);

						// Update client UI
						await GetGoldAndUpdateModelAsync();

						//Finish the transfer by mimicking the cancel button
						TransferDialogView_OnTransferCancelRequested(transferDialogView);
					});
			}
		}
		
		private void TransferDialogView_OnTransferCancelRequested(TransferDialogView transferDialogView)
		{
			TheGameSingleton.Instance.TheGameController.UnregisterView(transferDialogView); 
			GameObject.Destroy(transferDialogView.gameObject);
			SelectionManager.Instance.Selection = null;

			if (_theGameModel.IsTransferPending.Value)
			{
				//There was a transfer completed. Notify all players via RPC
				PlayerView localPlayerView = TheGameHelper.GetLocalPlayerView();
				localPlayerView.SendMessageTransferLogAsync();
			}
			
			_theGameModel.IsTransferPending.Value = false;
			
		}
		
		
		///////////////////////////////////////////
		// Related To: View
		///////////////////////////////////////////
		
		private void SelectionManager_OnSelectionChanged(ISelectionManagerSelectable selection)
		{
			Debug.Log($"OnSelectionChanged() selection = {selection}");
			_theGameModel.SelectedPlayerView.Value = (PlayerView)selection;

			if (_theGameModel.HasSelectedPlayerView)
			{
				TransferDialogView transferDialogView = TheGameHelper.InstantiatePrefab<TransferDialogView>(TheGameConfiguration.Instance.TransferDialogViewPrefab,
				TheGameSingleton.Instance.transform, new Vector3(0, 0, 0));
				RegisterView(transferDialogView);
			}
		}

		
		public void PlayAudioClip(int audioClipIndex)
		{
			_theGameView.PlayAudioClip(audioClipIndex );
		}
		public void PlayAudioClipClick()
		{
			_theGameView.PlayAudioClipClick();
		}


		public async void LoadIntroSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = TheGameConfiguration.Instance.IntroSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}


		public async void LoadAuthenticationSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = TheGameConfiguration.Instance.AuthenticationSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadSettingsSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = TheGameConfiguration.Instance.SettingsSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadDeveloperConsoleSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = TheGameConfiguration.Instance.DeveloperConsoleSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadGameSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = TheGameConfiguration.Instance.GameSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}


		public async void LoadPreviousSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			_theGameView.SceneManagerComponent.LoadScenePrevious();
		}

		/// <summary>
		/// For short async messaging
		/// </summary>
		public async UniTask ShowMessagePassiveAsync(Func<UniTask> task)
		{
			await _theGameView.ShowMessageDuringMethodAsync(
				_theGameService.PendingMessagePassive.Message, task);
		}
		
		/// <summary>
		/// For long async messaging (e.g. "Waiting for wallet...", then "Loading..."
		/// </summary>
		public async UniTask ShowMessageActiveAsync(string title, Func<UniTask> task)
		{
			await _theGameView.ShowMessageDuringMethodAsync(
				$"{title}\n-\n{_theGameService.PendingMessageActive.Message}", task);
		}
		
		// Wait for contract values to sync so the client will see the changes
		private async UniTask DelayExtraAfterStateChangeAsync()
		{
			if (_theGameService.HasExtraDelay)
			{
				_theGameView.UpdateMessageDuringMethod(_theGameService.PendingMessageExtraDelay.Message);
				await _theGameService.DoExtraDelayAsync();
			}
		}
		
		public async UniTask ShowMessageWithDelayAsync(string message, int delayMilliseconds)
		{
			await _theGameView.ShowMessageWithDelayAsync(message, delayMilliseconds);
		}

		public void UpdateMessageDuringMethod(string message)
		{
			_theGameView.UpdateMessageDuringMethod(message, false);
		}
		
		public void HideMessageDuringMethod(bool isAnimated)
		{
			_theGameView.HideMessageDuringMethod(isAnimated);
		}

		// Event Handlers ---------------------------------
		private void TheGameModel_OnTheGameModelChanged(TheGameModel theGameModel)
		{
			OnTheGameModelChanged.Invoke(_theGameModel);
		}
		
		
		private async void MultiplayerSetupService_OnConnectStarted()
		{
			Debug.Log($"OnConnectionStarted() ");
			await ShowMessageWithDelayAsync(TheGameConstants.MultiplayerConnecting, 2000);
		}
			
		
		private async void MultiplayerSetupService_OnConnectCompleted(string debugMessage)
		{
			await UniTask.Delay(1000);
			UpdateMessageDuringMethod(TheGameConstants.MultiplayerConnected);
			await UniTask.Delay(1000);
			HideMessageDuringMethod(true);
		}

		
		private async void MultiplayerSetupService_OnDisconnectStarted()
		{
			Debug.Log($"MultiplayerSetupService_OnDisconnectStarted() ");
			await ShowMessageWithDelayAsync(TheGameConstants.MultiplayerDisconnecting, 2000);
		}
		
		
		private async void MultiplayerSetupService_OnDisconnectCompleted()
		{
			Debug.Log($"MultiplayerSetupService_OnDisconnectionCompleted() ");
			UpdateMessageDuringMethod(TheGameConstants.MultiplayerDisconnected);
			await UniTask.Delay(1000);
			HideMessageDuringMethod(true);
		}
		
		
		private void MultiplayerSetupService_OnStateNameChanged(string stateName)
		{
			//Debug.Log($"OnStateNameChanged() {stateName}");
			//UpdateMessageDuringMethod(TheGameConstants.Multiplayer + " " + stateName);
			OnMultiplayerStateNameChanged.Invoke(stateName);
		}

		
		private void SceneManagerComponent_OnSceneLoadingEvent(SceneManagerComponent sceneManagerComponent, 
			string currentSceneName, string nextSceneName)
		{
			if (_theGameView.BaseScreenCoverUI.IsVisible)
			{
				_theGameView.BaseScreenCoverUI.IsVisible = false;
			}
			
			if (DOTween.TotalPlayingTweens() > 0)
			{
				DOTween.KillAll();
			}


		}

		
		private void SceneManagerComponent_OnSceneLoadedEvent(SceneManagerComponent sceneManagerComponent, 
			string previousSceneName, string currentSceneName)
		{

			// if (TheGameConfiguration.Instance.IsControllingWc)
			// {
			// 	CustomWeb3System.Instance.EnsureInstantiatedWalletConnectInstance();
			// }
			

		}


		public void QuitGame()
		{
			if (Application.isEditor)
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif //UNITY_EDITOR
			}
			else
			{
				Application.Quit();
			}
		}

	}
}
