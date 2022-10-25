using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService;
using UnityEngine;

#pragma warning disable CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Controller
{
	/// <summary>
	/// Stores data for the game
	///		* See <see cref="TheGameSingleton"/> - Handles the core functionality of the game
	/// </summary>
	public class TheWeb3Controller : IInitializable
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
		public PendingMessage PendingMessageActive { get { return _theGameService.PendingMessageActive; } }
		public PendingMessage PendingMessagePassive { get { return _theGameService.PendingMessagePassive; } }
		public PendingMessage PendingMessageExtraDelay { get { return _theGameService.PendingMessageExtraDelay; } }
		public async UniTask DoExtraDelayAsync () { await _theGameService.DoExtraDelayAsync(); } 

		public bool HasExtraDelay { get { return _theGameService.HasExtraDelay; } }
		
		public bool IsInitialized { get; private set; }

		// Fields -----------------------------------------
		private readonly TheGameModel _theGameModel = null;
		private readonly TheGameController _theGameController = null;
		private readonly ITheGameService _theGameService = null;
		
		// Wait, So click sound is audible before scene changes
		private const int DelayLoadSceneMilliseconds = 100;
		
		// Wait, So click sound is audible before scene changes
		private const int ExtraDelayAfterTransferMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheWeb3Controller(
			TheGameModel theGameModel,
			TheGameController theGameController,
			ITheGameService theGameService )
		{
			_theGameModel = theGameModel;
			_theGameController = theGameController;
			_theGameService = theGameService;
		}

		public void Initialize()
		{
			if (IsInitialized) return;
			
			// MODEL
			_theGameModel.OnTheGameModelChanged.AddListener(TheGameModel_OnTheGameModelChanged);
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
		public void OnDestroy()
		{
			if (!IsInitialized) return;

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
		
		
		// Wait for contract values to sync so the client will see the changes
		private async UniTask DelayExtraAfterStateChangeAsync()
		{
			if (_theGameService.HasExtraDelay)
			{
				_theGameController.UpdateMessageDuringMethod(_theGameService.PendingMessageExtraDelay.Message);
				await _theGameService.DoExtraDelayAsync();
			}
		}
		
		// Event Handlers ---------------------------------
		private void TheGameModel_OnTheGameModelChanged(TheGameModel theGameModel)
		{
			OnTheGameModelChanged.Invoke(_theGameModel);
		}
	}
}
