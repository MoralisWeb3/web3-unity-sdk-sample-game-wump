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
using UnityEngine;

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
		public TheGameModelUnityEvent OnTheGameModelChanged = new TheGameModelUnityEvent();
		public void OnTheGameModelChangedRefresh() { OnTheGameModelChanged.Invoke(_theGameModel); }

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
		private readonly DetailsView _detailsView = null;
		
		// Wait, So click sound is audible before scene changes
		private const int DelayLoadSceneMilliseconds = 100;

		// Initialization Methods -------------------------
		public TheGameController(
			TheGameModel theGameModel,
			TheGameView theGameView,
			DetailsView detailsView,
			NetworkManagerView networkManagerView,
			ITheGameService theGameService,
			IMultiplayerSetupService multiplayerSetupService)
		{
			_theGameModel = theGameModel;
			_theGameView = theGameView;
			_networkManagerView = networkManagerView;
			_detailsView = detailsView;
			_theGameService = theGameService;
			_multiplayerSetupService = multiplayerSetupService;
			
			_detailsView.gameObject.SetActive(false);

		}

		public void Initialize()
		{
			if (IsInitialized) return;
			
			//
			_multiplayerSetupService.OnConnectionStarted.AddListener(MultiplayerSetupService_OnConnectionStarted);
			_multiplayerSetupService.OnConnectionCompleted.AddListener( MultiplayerSetupService_OnConnectionCompleted);
			_multiplayerSetupService.OnStateNameChanged.AddListener( MultiplayerSetupService_OnStateNameChanged);
			
			
			//
			_theGameView.SceneManagerComponent.OnSceneLoadingEvent.AddListener(SceneManagerComponent_OnSceneLoadingEvent);
			_theGameView.SceneManagerComponent.OnSceneLoadedEvent.AddListener(SceneManagerComponent_OnSceneLoadedEvent);
			SelectionManager.Instance.OnSelectionChanged.AddListener(SelectionManager_OnSelectionChanged);
			
			//TODO: Change this to have ONE invoker inside the model that itself knows when to invoke?
			_theGameModel.Gold.OnValueChanged.AddListener((a) => OnTheGameModelChangedRefresh());
			_theGameModel.Prizes.OnValueChanged.AddListener((a) => OnTheGameModelChangedRefresh());
			_theGameModel.IsRegistered.OnValueChanged.AddListener((a) => OnTheGameModelChangedRefresh());
			_theGameModel.ResetAllData();

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
		public void OnGUI()
		{
			if (IsInitialized && TheGameConfiguration.Instance.MultiplayerIsGuiVisible)
			{
				_multiplayerSetupService.OnGUI();
			}
		}
		
		
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
		public async UniTask<bool> GetIsAuthenticatedAsync()
		{
			return await CustomWeb3System.Instance.IsAuthenticatedAsync();
		}
		
		
		public async UniTask<string> GetWeb3UserAddressAsync(bool useShortFormat)
		{
			return await CustomWeb3System.Instance.GetWeb3UserAddressAsync(useShortFormat);
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
				
				if (_theGameModel.CustomPlayerInfo.Value.IsNullWeb3Address())
				{
					string web3Address = await GetWeb3UserAddressAsync(true);
					SetPlayerWeb3AddressAndUpdateModel(web3Address);
				}
				
				if (_theGameModel.CustomPlayerInfo.Value.IsNullNickname())
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
			string n = _theGameModel.CustomPlayerInfo.Value.Nickname.Value;
			string w = _theGameModel.CustomPlayerInfo.Value.Web3Address.Value;
			_theGameModel.CustomPlayerInfo.Value = new CustomPlayerInfo { Nickname = nickname, Web3Address = w };
			OnTheGameModelChangedRefresh();
		}
		
		
		public void SetPlayerWeb3AddressAndUpdateModel(string web3address)
		{
			string n = _theGameModel.CustomPlayerInfo.Value.Nickname.Value;
			_theGameModel.CustomPlayerInfo.Value = new CustomPlayerInfo { Nickname = n, Web3Address = web3address };
			OnTheGameModelChangedRefresh();
		}
		
		
		public void RandomizeNicknameAndUpdateModel()
		{
			string nickname = TheGameHelper.GetRandomizedNickname();
			SetPlayerNicknameAndUpdateModel(nickname);
		}

		
		// SETTER Methods -------------------------
		public async UniTask RegisterAsync()
		{
			await _theGameService.RegisterAsync();
			_theGameModel.IsRegistered.Value = await GetIsRegisteredAndUpdateModelAsync();
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
		}

		
		public async UniTask UnregisterAsync()
		{
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
		}
		
		
		public async UniTask SafeReregisterDeleteAllPrizesAsync()
		{
			_theGameModel.ResetAllData();
			await _theGameService.SafeReregisterDeleteAllPrizesAsync();
			
			// Wait for contract values to sync so the client will see the changes
			await DelayExtraAfterStateChangeAsync();
		}
		
		
		///////////////////////////////////////////
		// Related To: Networking
		///////////////////////////////////////////

		public void MultiplayerSetupServiceConnect()
		{
			_detailsView.gameObject.SetActive(false);
			_multiplayerSetupService.Connect();
		}
		
		public bool MultiplayerSetupServiceIsConnected()
		{
			return _multiplayerSetupService.IsConnected;
		}
		
		public async void MultiplayerSetupServiceDisconnectAsync()
		{
			_detailsView.gameObject.SetActive(false);
			await _multiplayerSetupService.DisconnectAsync();
		}
		
		public void RegisterPlayerView(PlayerView playerView)
		{
			playerView.OnIsWalkingChanged.AddListener(PlayerView_OnIsWalkingChanged);
			playerView.OnPlayerAction.AddListener(PlayerView_OnPlayerAction);
		}

		
		public void UnregisterPlayerView(PlayerView playerView)
		{
			playerView.OnIsWalkingChanged.RemoveListener(PlayerView_OnIsWalkingChanged);
		}

		
		private void PlayerView_OnIsWalkingChanged(PlayerView playerView)
		{
			if (!playerView.IsLocalPlayer) return;
			DetailsView.Instance.LocalStatus = playerView.IsWalking.Value ? "Walking" : "Idle";
		}
		
		
		private void PlayerView_OnPlayerAction()
		{
			DetailsView.Instance.SharedStatusUpdateRequest();
		}
		
		
		public void RegisterTransferDialogView(TransferDialogView transferDialogView)
		{
			transferDialogView.OnTransferGoldRequested.AddListener(TransferDialogView_OnTransferGoldRequested);
			transferDialogView.OnTransferPrizeRequested.AddListener(TransferDialogView_OnTransferPrizeRequested);
		}


		public void UnregisterTransferDialogView(TransferDialogView transferDialogView)
		{
			transferDialogView.OnTransferGoldRequested.RemoveListener(TransferDialogView_OnTransferGoldRequested);
			transferDialogView.OnTransferPrizeRequested.RemoveListener(TransferDialogView_OnTransferPrizeRequested);
			SelectionManager.Instance.Selection = null;
		}
		
		
		public bool CanTransferGoldToSelected()
		{
			if (_theGameModel.HasSelectedPlayerView)
			{
				if (_theGameModel.Gold.Value >= TheGameConstants.GoldOnTransfer)
				{
					return true;
				}
				else
				{
					Debug.LogWarning("Nothing to transfer");
				}
			}
			else
			{
				Debug.LogWarning("No one is selected");
			}

			return false;
		}
		

		public bool CanTransferPrizeToSelected()
		{
			if (_theGameModel.HasSelectedPlayerView)
			{
				if (_theGameModel.Prizes.Value.Count >= TheGameConstants.PrizesOnTransfer)
				{
					return true;
				}
				else
				{
					Debug.LogWarning("Nothing to transfer");
				}
			}
			else
			{
				Debug.LogWarning("No one is selected");
			}

			return false;
		}
		
		
		private async void TransferDialogView_OnTransferPrizeRequested()
		{
			if (CanTransferPrizeToSelected())
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
					TheGameConstants.TransferingPrize,
					async delegate ()
					{
						//string address = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e"; 
						CustomPlayerInfo customPlayerInfo = new CustomPlayerInfo(); // based on _theGameModel.SelectedPlayerView.Value.OwnerClientId
						TransferPrizeAsync(customPlayerInfo.Web3Address.Value, _theGameModel.Prizes.Value[0]);
					});
			}
		}
		
		private async void TransferDialogView_OnTransferGoldRequested ()
		{
			if (CanTransferGoldToSelected())
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
					TheGameConstants.TransferingGold,
					async delegate ()
					{
						//string address = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e"; 
						CustomPlayerInfo customPlayerInfo = new CustomPlayerInfo(); // based on _theGameModel.SelectedPlayerView.Value.OwnerClientId
						TransferGoldAsync(customPlayerInfo.Web3Address.Value);
					});
			}
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

			string sceneName = _theGameModel.TheGameConfiguration.IntroSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}


		public async void LoadAuthenticationSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = _theGameModel.TheGameConfiguration.AuthenticationSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadSettingsSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = _theGameModel.TheGameConfiguration.SettingsSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadDeveloperConsoleSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = _theGameModel.TheGameConfiguration.DeveloperConsoleSceneData.SceneName;
			_theGameView.SceneManagerComponent.LoadScene(sceneName);
		}

		public async void LoadGameSceneAsync()
		{
			// Wait, So click sound is audible before scene changes
			await UniTask.Delay(DelayLoadSceneMilliseconds);

			string sceneName = _theGameModel.TheGameConfiguration.GameSceneData.SceneName;
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
				
		private void MultiplayerSetupService_OnConnectionStarted()
		{
			Debug.Log($"OnConnectionStarted() ");
			ShowMessageWithDelayAsync(TheGameConstants.MultiplayerConnecting, 10000);
		}
		
		private void MultiplayerSetupService_OnStateNameChanged(string stateName)
		{
			Debug.Log($"OnStateNameChanged() {stateName}");
			DetailsView.Instance.LocalStatus = stateName;
			UpdateMessageDuringMethod(TheGameConstants.Multiplayer + " " + stateName);
		}
		
		private async void MultiplayerSetupService_OnConnectionCompleted(string debugMessage)
		{
			await UniTask.Delay(1000);
			UpdateMessageDuringMethod(TheGameConstants.MultiplayerConnected);
			await UniTask.Delay(1000);
			HideMessageDuringMethod(true);
		}
		
		private void SceneManagerComponent_OnSceneLoadingEvent(SceneManagerComponent sceneManagerComponent)
		{
			if (_theGameView.BaseScreenCoverUI.IsVisible)
			{
				_theGameView.BaseScreenCoverUI.IsVisible = false;
			}
			
			// HACK: The WalletConnect prefab is not a robust Singleton pattern.
			// It does not work well if the prefab is in 2 or more scenes that are used at runtime. The 2 or more instances conflict.
			// So I manually delete the current one BEFORE the next scene loads. Works 100%
			if (CustomWeb3System.Instance.HasWalletConnectInstance)
			{
				CustomWeb3System.Instance.DestroyWalletConnectInstance();
			}

			if (DOTween.TotalPlayingTweens() > 0)
			{
				DOTween.KillAll();
			}
		}

		
		private void SceneManagerComponent_OnSceneLoadedEvent(SceneManagerComponent sceneManagerComponent)
		{
			// Do anything?
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
