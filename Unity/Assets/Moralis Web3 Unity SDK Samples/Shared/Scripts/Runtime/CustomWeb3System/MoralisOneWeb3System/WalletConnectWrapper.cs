using System;
using UnityEngine;
using WalletConnectSharp.Unity;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class WalletConnectWrapper : MonoBehaviour
	{
		// Properties -------------------------------------
	
		// Properties -------------------------------------
		
		// Fields -----------------------------------------
		[SerializeField] 
		private WalletConnect _walletConnect;

		// Unity Methods -------------------------
		protected void OnDestroy()
		{
			if (_walletConnect != null || WalletConnect.Instance != null)
			{
				Debug.LogWarning("Destroy WalletConnect");
				Destroy(_walletConnect.gameObject);
				Destroy(this.gameObject);
			}
		}

		// General Methods --------------------------------
		
		
		// Event Handlers ---------------------------------
	
	}
}