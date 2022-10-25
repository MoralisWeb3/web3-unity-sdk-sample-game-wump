using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Components;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.View;
using RMC.Shared.Managers;
using UnityEngine;
using NotInitializedException = MoralisUnity.Samples.Shared.Exceptions.NotInitializedException;
using SwitchDefaultException = MoralisUnity.Samples.Shared.Exceptions.SwitchDefaultException;

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

			
		// Properties -------------------------------------
		public bool IsInitialized { get; private set; }

		public TheWeb3Controller TheWeb3Controller
		{
			set
			{
				//Must set this value before init
				RequireIsNotInitialized();
				_theWeb3Controller = value;
			}
		}

		// Fields -----------------------------------------
		private readonly TheGameModel _theGameModel = null;
		private readonly TheGameView _theGameView = null;
		private TheWeb3Controller _theWeb3Controller = null;
		
		// Wait, So click sound is audible before scene changes
		private const int DelayLoadSceneMilliseconds = 100;
		
		// Wait, So click sound is audible before scene changes
		private const int ExtraDelayAfterTransferMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheGameController(
			TheGameModel theGameModel,
			TheGameView theGameView)
		{
			_theGameModel = theGameModel;
			_theGameView = theGameView;
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
		
		public void RequireIsNotInitialized()
		{
			if (IsInitialized)
			{
				throw new InitializedException(this);
			}
		}
		
		// Unity Methods --------------------------------
		public void OnDestroy()
		{
			if (!IsInitialized) return;
			
			SelectionManager.Instance.OnSelectionChanged.RemoveListener(SelectionManager_OnSelectionChanged);

		}

		// General Methods --------------------------------
		
		///////////////////////////////////////////
		// Related To: MyMoralisWrapper
		///////////////////////////////////////////
		public async UniTask<bool> GetIsAuthenticatedAndUpdateModelAsync()
		{
			// Call auth, the get value
			await CustomWeb3System.Instance.AuthenticateAsync();
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
					playerView.OnPlayerAction.RemoveListener(PlayerView_OnPlayerAction);
					playerView.OnRPCSharedStatusChanged.RemoveListener(PlayerView_OnRPCSharedStatusChanged);
					playerView.OnRPCTransferLogChanged.RemoveListener(PlayerView_OnRPCTransferLogChanged);
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
						await _theWeb3Controller.TransferPrizeAsync(_theGameModel.SelectedPlayerView.Value.Web3Address, 
							_theGameModel.Prizes.Value[0]);

						// Update client UI
						await _theWeb3Controller.GetPrizesAndUpdateModelAsync();
						
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
						await _theWeb3Controller.TransferGoldAsync(_theGameModel.SelectedPlayerView.Value.Web3Address);

						// Update client UI
						await _theWeb3Controller.GetGoldAndUpdateModelAsync();

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
			_theGameModel.SelectedPlayerView.Value = (PlayerView)selection;

			if (_theGameModel.HasSelectedPlayerView)
			{
				TransferDialogView transferDialogView = TheGameHelper.InstantiatePrefab<TransferDialogView>(TheGameConfiguration.Instance.TransferDialogViewPrefab,
				TheGameSingleton.Instance.transform, new Vector3(0, 0, 0));
				RegisterView(transferDialogView);
			}
		}

		public void PlayAudioClipClick()
		{
			_theGameView.PlayAudioClipClick();
		}

		public bool WasActiveSceneLoadedDirectly()
		{
			return _theGameView.SceneManagerComponent.WasActiveSceneLoadedDirectly();
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


		/// <summary>
		/// For short async messaging
		/// </summary>
		public async UniTask ShowMessagePassiveAsync(Func<UniTask> task)
		{
			await _theGameView.ShowMessageDuringMethodAsync(
				_theWeb3Controller.PendingMessagePassive.Message, task);
		}
		
		/// <summary>
		/// For long async messaging (e.g. "Waiting for wallet...", then "Loading..."
		/// </summary>
		public async UniTask ShowMessageActiveAsync(string title, Func<UniTask> task)
		{
			await _theGameView.ShowMessageDuringMethodAsync(
				$"{title}\n-\n{_theWeb3Controller.PendingMessageActive.Message}", task);
		}
		
		// Wait for contract values to sync so the client will see the changes
		private async UniTask DelayExtraAfterStateChangeAsync()
		{
			if (_theWeb3Controller.HasExtraDelay)
			{
				_theGameView.UpdateMessageDuringMethod(_theWeb3Controller.PendingMessageExtraDelay.Message);
				await _theWeb3Controller.DoExtraDelayAsync();
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
			SharedHelper.SafeQuitGame();
		}

	}
}
