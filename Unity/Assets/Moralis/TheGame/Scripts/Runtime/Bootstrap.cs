using UnityEditor;
using UnityEngine;

namespace MoralisUnity.Samples.Shared
{
    public class MoralisTwoWeb3SystemFactory : ICustomWeb3SystemInstanceFactory
    {
        public ICustomWeb3System Create()
        {
            Debug.LogWarning("Create");
            // Optional: Manually update this one-liner to change implementation
            ICustomWeb3System customWeb3System = MoralisTwoWeb3System.Instantiate();
            customWeb3System.CustomWeb3WalletSystem = new WalletConnectWeb3WalletSystem();
            customWeb3System.CustomBackendSystem = new PlayFabBackendSystem();
            customWeb3System.AuthenticateAsync();
            return customWeb3System;
        }
    }
    
    [InitializeOnLoad] 
    public static class Bootstrap
    {
        static Bootstrap()
        {
            Debug.LogWarning("Bootstrap");
            CustomWeb3System.SetICustomWeb3SystemInstanceFactory(new MoralisTwoWeb3SystemFactory());
        }
    }
}