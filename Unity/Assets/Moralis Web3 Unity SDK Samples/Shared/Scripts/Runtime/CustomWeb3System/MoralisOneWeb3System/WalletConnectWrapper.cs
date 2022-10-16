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
		protected void Start()
		{
			EnsureInstantiatedWalletConnectInstance();
		}

		// General Methods --------------------------------
		
		public void EnsureInstantiatedWalletConnectInstance()
		{
			Debug.LogWarning("1 EnsureInstantiatedWalletConnectInstance() before = " + CustomWeb3System.Instance.HasWalletConnectStaticInstance);
			if (CustomWeb3System.Instance.HasWalletConnectStaticInstance) return;
			_walletConnectLocallyCreated = Instantiate(_walletConnectPrefab);
			Debug.LogWarning("2 EnsureInstantiatedWalletConnectInstance() after = " + CustomWeb3System.Instance.HasWalletConnectStaticInstance);

			

		}
		// Event Handlers ---------------------------------
	
	}
}