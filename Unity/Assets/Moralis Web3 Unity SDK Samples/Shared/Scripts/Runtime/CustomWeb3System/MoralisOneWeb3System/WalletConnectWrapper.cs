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
		private WalletConnect _walletConnectPrefab;

		private bool _hasWalletConnectOnStart = false;
		private bool _hasWalletConnectOnUpdate = false;
		private bool _hasReportedLoss = false;
		private WalletConnect _walletConnectLocallyCreated;

		// Unity Methods -------------------------

		public void EnsureWalletConnectExists()
		{
			if (WalletConnect.Instance == null)
			{
				Debug.Log("8888 Manually Creating WalletConnect");
				_walletConnectLocallyCreated = Instantiate<WalletConnect>(_walletConnectPrefab);
			}

			WalletConnectQRImage walletConnectQRImage = FindObjectOfType<WalletConnectQRImage>();
			Debug.Log("1");
			if (walletConnectQRImage != null)
			{
				Debug.Log("2");
				if (walletConnectQRImage.walletConnect == null)
				{
					Debug.Log("3");
					walletConnectQRImage.walletConnect = WalletConnect.Instance;
				}
			}
		}
		
		public void EnsureWallectConnectIsDestroyed()
		{
			if (_walletConnectLocallyCreated != null)
			{
				Debug.Log("8888 Manually Destroying WalletConnect");
				Destroy(_walletConnectLocallyCreated.gameObject);
			}
		}
		
		protected void OnDestroy()
		{
			Debug.Log($"88888 OnDestroy:");
			// if (_walletConnect != null || WalletConnect.Instance != null)
			// {
			// 	Debug.LogWarning("Destroy WalletConnect");
			// 	Destroy(_walletConnect.gameObject);
			// 	Destroy(this.gameObject);
			// }
		}
		// General Methods --------------------------------
		
		
		// Event Handlers ---------------------------------
	
	}
}