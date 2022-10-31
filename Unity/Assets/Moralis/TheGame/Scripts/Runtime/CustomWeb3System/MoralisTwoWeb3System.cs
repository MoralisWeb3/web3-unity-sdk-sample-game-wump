using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Interfaces;
using UnityEngine;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;
using MoralisUnity.Samples.SharedCustom.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using PlayFab.CloudScriptModels;

#pragma warning disable 1998, CS4014
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class MoralisTwoWeb3System : CustomSingleton<MoralisTwoWeb3System>, 
		IInitializableAsync, ICustomSingletonParent, ICustomWeb3System
	{
		// Properties -------------------------------------
		
		public bool IsInitialized { get; private set; }
		public bool IsAuthenticated { get; private set; }
		
		public ICustomWeb3WalletSystem CustomWeb3WalletSystem 
		{
			set
			{
				//To encourage vigilant use of this method,
				//it is required to be set early in lifecycle
				RequireIsNotInitialized();
				_customWeb3WalletSystem = value;
			}
			get
			{
				return _customWeb3WalletSystem;
			}
		}
		
		public ICustomBackendSystem CustomBackendSystem 
		{
			set
			{
				//To encourage vigilant use of this method,
				//it is required to be set early in lifecycle
				RequireIsNotInitialized();
				_customBackendSystem = value;
			}
			get
			{
				return _customBackendSystem;
			}
		}
		
		public int ChainId
		{
			get
			{
				return _customWeb3WalletSystem.ChainId;
			}
		}


		// Fields -----------------------------------------
		private ICustomWeb3WalletSystem _customWeb3WalletSystem;
		private ICustomBackendSystem _customBackendSystem;

		// Unity Methods ----------------------------------

		// Initialization Methods -------------------------
		void ICustomSingletonParent.OnInstantiatedChild()
		{
			//Do not InitializeAsync here
			//Wait for external scope to call, 	IsAuthenticatedAsync() which will do so
		}

		public async UniTask InitializeAsync()
		{
			if (!IsInitialized)
			{
				// Do initialize / do not auth
				_customWeb3WalletSystem.Initialize();
				if (!_customWeb3WalletSystem.IsInitialized)
				{
					Debug.Log($"{GetType().Name}.InitializeAsync() failed. Error 0001");
				}
				// Do initialize
				_customBackendSystem.Initialize();
				if (!_customBackendSystem.IsInitialized)
				{
					Debug.Log($"{GetType().Name}.InitializeAsync() failed. Error 0002");
				}
				
				// Do auth
				await _customBackendSystem.AuthenticateAsync();
				if (!_customBackendSystem.IsAuthenticated)
				{
					Debug.Log($"{GetType().Name}.InitializeAsync() failed. Error 0003");
				}

			}
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
		
		public async void RequireIsAuthenticated()
		{
			if (!IsAuthenticated)
			{
				throw new NotAuthenticatedException(this);
			}
		}

		//Statics cannot be overriden, but this is the proper
		//solution to get parent static functionality in local scope. Keep.
		public new static void OnEnteredEditMode()
		{
			Debug.Log("OnEnteredEditMode() !!!!!! ");
			//Clear out statics when play stops
			Uninstantiate();
		}


		// General Methods --------------------------------


		public async UniTask<string> EthPersonalSignAsync(string web3UserAddress, string message)
		{
			return await _customWeb3WalletSystem.EthPersonalSignAsync(web3UserAddress, message);
		}

		public async UniTask KLUGE_CloseOpenWalletConnection()
		{
			await _customWeb3WalletSystem.KLUGE_CloseOpenWalletConnection();
		}

		public async UniTask<ExecuteFunctionResult> ChallengeRequestAsync(string web3UserAddress,int chainId)
		{
			return await _customBackendSystem.ChallengeRequestAsync(web3UserAddress, chainId);
		}

		public async UniTask<ExecuteFunctionResult> ChallengeVerifyAsync(string message, string signature)
		{
			return await _customBackendSystem.ChallengeVerifyAsync(message, signature);
		}

		public bool HasActiveSession
		{
			get
			{
				return _customWeb3WalletSystem.HasActiveSession;
			}
		}

		
		public async UniTask<bool> IsAuthenticatedAsync()
		{
			if (!IsInitialized)
			{
				await InitializeAsync();
			}
			RequireIsInitialized();
			
			//Recheck every time
			IsAuthenticated = await HasWeb3UserAddressAsync();

			return IsAuthenticated;
		}
		
		
		public async UniTask AuthenticateAsync()
		{
			// Do initialize
			if (!_customWeb3WalletSystem.IsConnected)
			{
				
				// Call without await
				if (_customWeb3WalletSystem.IsConnected)
				_customWeb3WalletSystem.ConnectAsync();
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				cancellationTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(2)); // 2sec timeout is enough

				try
				{
					await UniTask.WaitWhile(
						() =>
						{
							return !_customWeb3WalletSystem.IsConnected;
						}, PlayerLoopTiming.Update, cancellationTokenSource.Token);
				}
				catch (OperationCanceledException)
				{
					//Called for timeout. No problem.
				}

			}
			await IsAuthenticatedAsync();
		}

		
		public async UniTask<bool> HasWeb3UserAddressAsync()
		{
			RequireIsInitialized();
			return await _customWeb3WalletSystem.HasWeb3UserAddressAsync();
		}

		
		public async UniTask<string> GetWeb3UserAddressAsync()
		{
			RequireIsInitialized();
			return await _customWeb3WalletSystem.GetWeb3UserAddressAsync();
		}

		
		public string ConvertWeb3AddressToShortFormat(string web3Address)
		{
			RequireIsInitialized();
			const int n = 6;
			if (string.IsNullOrEmpty(web3Address))
			{
				return string.Empty;
			}
        
			if (web3Address.Length < n)
			{
				return web3Address;
			}

			return $"{web3Address.Substring(0, n)}...{web3Address.Substring(web3Address.Length - n)}";
		}

		
		public async UniTask<String> ExecuteContractFunctionAsync(string contractAddress, string abi,
			string functionName, object[] args, bool isLogging = false)
		{

			RequireIsInitialized();
			RequireIsAuthenticated();

			string result = await _customBackendSystem.ExecuteContractFunctionAsync(
				contractAddress,
				abi,
				functionName,
				args,
				isLogging);
			return result;
		}

		// Event Handlers ---------------------------------
		public async UniTask<object> RunContractFunctionAsync(
			string contractAddress,
			string functionName,
			string abi, 
			object args,
			bool isLogging = false)
		{
			
			RequireIsInitialized();
			RequireIsAuthenticated();

			object result = await _customBackendSystem.RunContractFunctionAsync(
				contractAddress,
				functionName,
				abi,
				args,
				isLogging);
			
			return result;
		}


		public async UniTask<List<NftOwner>> GetNFTsForContractAsync(string contractAddress, bool isLogging = false)
		{
			RequireIsInitialized();
			RequireIsAuthenticated();
			//
			string web3UserAddress = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
			int chainid = this.ChainId;

			List<NftOwner> resultNftOwners = await _customBackendSystem.GetNFTsForContractAsync(
				contractAddress,
				isLogging);
			return resultNftOwners;
		}
	}
}

