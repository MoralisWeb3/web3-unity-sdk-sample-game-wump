using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingletonMonobehaviour;
using UnityEngine;
using WalletConnectSharp.Unity;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class WalletConnectWrapper : CustomSingletonMonobehaviour<WalletConnectWrapper>
	{
		// Properties -------------------------------------
	
		// Properties -------------------------------------
		
		// Fields -----------------------------------------
		[SerializeField] 
		private WalletConnect _walletConnectPrefab;
		private WalletConnect _walletConnectLocallyCreated;

		// Unity Methods -------------------------
		protected void Awake()
		{
		}

		// General Methods --------------------------------
		
		public void EnsureInstantiatedWalletConnectInstance()
		{
			Debug.LogWarning("1 EnsureInstantiatedWalletConnectInstance() before = " + CustomWeb3System.Instance.HasWalletConnectStaticInstance);
			if (CustomWeb3System.Instance.HasWalletConnectStaticInstance) return;
			_walletConnectLocallyCreated = Instantiate(_walletConnectPrefab);
			Debug.LogWarning("2 EnsureInstantiatedWalletConnectInstance() after = " + CustomWeb3System.Instance.HasWalletConnectStaticInstance);

			WalletConnectQRImage walletConnectQrImage = FindObjectOfType<WalletConnectQRImage>();
			Debug.LogWarning("1111: " + walletConnectQrImage);
			if (walletConnectQrImage != null)
			{
				walletConnectQrImage.walletConnect = WalletConnect.Instance;
				Debug.LogWarning(
					"3 EnsureInstantiatedWalletConnectInstance() after = " + walletConnectQrImage.walletConnect);

				
			}

			WalletConnectQRImage[] walletConnectQrImage2 = Resources.FindObjectsOfTypeAll<WalletConnectQRImage>();

			if (walletConnectQrImage2.Length > 0)
			{
				Debug.LogWarning("2222: " + walletConnectQrImage2[0]);
			}
			

		}
		// Event Handlers ---------------------------------
	
	}
}