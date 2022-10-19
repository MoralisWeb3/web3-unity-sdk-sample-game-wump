using System;
using Cysharp.Threading.Tasks;
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
		public bool HasActiveSession
		{
			get
			{
				return WalletConnect.Instance != null && WalletConnect.ActiveSession != null;
			}
		}
		
		public bool IsConnected
		{
			get
			{
				return WalletConnect.Instance != null && WalletConnect.ActiveSession != null && WalletConnect.ActiveSession.Connected;
			}
		}
		
		public int ChainId
		{
			get
			{
				return WalletConnect.ActiveSession.ChainId;
			}
		}
		// Fields -----------------------------------------

		// Initialization Methods -------------------------
		
		// General Methods --------------------------------
		public async UniTask ConnectAsync()
		{
			await WalletConnect.ActiveSession.Connect();
		}
		
		
		public async UniTask<bool> HasWeb3UserAddressAsync()
		{
			string web3UserAddress = await GetWeb3UserAddressAsync();
			return !string.IsNullOrEmpty(web3UserAddress);
		}


		public async UniTask ClearActiveSession()
		{
			WalletConnect.Instance.CLearSession();
			
			await UniTask.WaitWhile(() => !HasActiveSession);
		}

		public async UniTask<string> GetWeb3UserAddressAsync()
		{
			//Do not require AUTH here, because this method IS PART of the auth-check
			if (HasActiveSession)
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
		}
		
		// Event Handlers ---------------------------------
	}
}

