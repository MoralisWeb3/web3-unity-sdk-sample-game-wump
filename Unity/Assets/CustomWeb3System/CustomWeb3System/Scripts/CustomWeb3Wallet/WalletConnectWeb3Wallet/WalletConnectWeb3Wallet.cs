using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using UnityEngine;
using WalletConnectSharp.Unity;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class WalletConnectWeb3WalletSystem : ICustomWeb3WalletSystem
	{
		// Properties -------------------------------------
		public bool IsInitialized { get; private set; }


		public bool HasActiveSession
		{
			get
			{
				//Allow false without throwing exception
				return HasWalletConnectInstance && WalletConnect.ActiveSession != null;
			}
		}


		public bool IsConnected
		{
			get
			{
				//Allow false without throwing exception
				return HasWalletConnectInstance && WalletConnect.ActiveSession != null &&
				       WalletConnect.ActiveSession.Connected;
			}
		}


		public int ChainId
		{
			get
			{
#if !UNITY_WEBGL
				RequireIsInitialized();
				return WalletConnect.ActiveSession.ChainId;
#else
				return Web3GL.ChainId();;
#endif
			}
		}


		//Keep this private
		private bool HasWalletConnectInstance
		{
			get { return WalletConnectInstance != null; }
		}


		//Keep this private
		private WalletConnect WalletConnectInstance
		{
			get
			{
				// Internally, we ALWAYS 're-get' this instead of assuming it still exists. It is brittle.
				return WalletConnect.Instance;
			}
		}


		// Fields -----------------------------------------


		// Initialization Methods -------------------------
		public void Initialize()
		{
			if (!IsInitialized)
			{
				IsInitialized = HasWalletConnectInstance;
				if (!IsInitialized)
				{
					Debug.LogError("Initialize() failed. HasWalletConnectInstance must not equal false");
				}
			}
		}


		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}


		// General Methods --------------------------------
		public async UniTask ConnectAsync()
		{
			RequireIsInitialized();

			// CLear out the session so it is re-establish on sign-in.
			WalletConnectInstance.CLearSession();

			// Enable auto save to remember the session for future use 
			WalletConnectInstance.autoSaveAndResume = true;

			// Don't start a new session on disconnect automatically
			WalletConnectInstance.createNewSessionOnSessionDisconnect = false;

			await WalletConnectInstance.Connect();
		}

		public async UniTask DisconnectAsync()
		{
			try
			{
				// Close the WalletConnect Transport Session
				await WalletConnectInstance.Session.Transport.Close();

				// Disconnect the WalletConnect session
				await WalletConnectInstance.Session.Disconnect();
			}
			catch (Exception e)
			{
				//Reason for Aborted warning is unknown, but expected. 
				if (e.Message != "Aborted")
				{
					// Send error to the log but not as an error as this is expected behavior from W.C.
					Debug.LogWarning($"[WalletConnect] Error = {e.Message}");
				}
			}
		}


		public async UniTask<bool> HasWeb3UserAddressAsync()
		{
			RequireIsInitialized();
			string web3UserAddress = await GetWeb3UserAddressAsync();
			return !string.IsNullOrEmpty(web3UserAddress);
		}


		public async UniTask ClearActiveSessionAsync()
		{
			RequireIsInitialized();
			WalletConnectInstance.CLearSession();
			
			await UniTask.WaitWhile(() => !HasActiveSession);
		}
		
		


		/// <summary>
		/// KLUGE: Before connecting a HARD reset was needed.
		/// </summary>
		public async UniTask KLUGE_CloseOpenWalletConnection()
		{
			try
			{
				//NOTE: Repeated calls here are PURPOSEFUL and required based on trial/error.
				//      No idea why.
				await ClearActiveSessionAsync(); //1
				await CloseActiveSessionAsync(); //2
				await ClearActiveSessionAsync(); //3
				await CloseActiveSessionAsync(); //4
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Known Issue: {e.Message}");
			}
		}

		private async UniTask CloseActiveSessionAsync(bool willImmediatelyReconnect = false)
		{
			RequireIsInitialized();
			WalletConnectInstance.CloseSession(willImmediatelyReconnect);
			if (willImmediatelyReconnect)
			{
				await WalletConnectInstance.Connect();
			}
			await UniTask.NextFrame();
		}

		public async UniTask<string> EthPersonalSignAsync(string web3UserAddress, string message)
		{
						                
#if !UNITY_WEBGL
			// Sign the message with WalletConnect
			string signature = await WalletConnect.ActiveSession.EthPersonalSign(web3UserAddress, message);
#else
            // Sign the message with Web3
            string signature = await Web3GL.Sign(message);
#endif
			return signature;
		}


		public async UniTask<string> GetWeb3UserAddressAsync()
		{
			
#if !UNITY_WEBGL
			//Do not require AUTH here, because this method IS PART of the auth-check
			//Do not require INIT here, because this method IS PART of the auth-check
			if (HasWalletConnectInstance && HasActiveSession)
			{
				if (WalletConnect.ActiveSession.Accounts != null && 
				    WalletConnect.ActiveSession.Accounts.Length > 0)
				{
					try
					{
						return WalletConnect.ActiveSession.Accounts[0].ToLower();
					}
					catch (Exception e)
					{
						Debug.LogWarning($"GetWeb3UserAddressAsync() failed. e = {e.Message}");
					}
				}
			}
			return string.Empty;
#else
			return Web3GL.Account().ToLower();
#endif

		}
		
		// Event Handlers ---------------------------------

	}
}

