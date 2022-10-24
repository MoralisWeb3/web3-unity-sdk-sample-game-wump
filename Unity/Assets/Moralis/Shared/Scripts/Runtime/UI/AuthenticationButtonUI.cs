using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared.UI
{
    /// <summary>
    /// The UI for shared use.
    /// </summary>
    public class AuthenticationButtonUI : BaseButtonUI
    {
        //  Properties  ---------------------------------------
        public bool IsAuthenticated { get { return _isAuthenticated;}}
        private bool _hasCalledAuthenticate = false;

        private async UniTask<bool> AuthenticateAndReturnBoolAsync()
        {
            if (!_isAuthenticated)
            {
                await CustomWeb3System.Instance.AuthenticateAsync();
                _isAuthenticated = await CustomWeb3System.Instance.IsAuthenticatedAsync();
                _hasCalledAuthenticate = true;
            }
            return _isAuthenticated;
        }
      
        //  Fields  ---------------------------------------
        private bool _isAuthenticated = false;
        
        //  Unity Methods  --------------------------------
        public async void Awake()
        {
            await CheckIsAuthenticatedAsync();
        }
        
        //  General Methods -------------------------------
        private async Task CheckIsAuthenticatedAsync()
        {
            //Don't set auth value
            _hasCalledAuthenticate = false;
            await RefreshUI();
            await AuthenticateAndReturnBoolAsync();
            await RefreshUI();
        }
        
        private async Task RefreshUI()
        {
            IsInteractable = true;
            
            if (_isAuthenticated)
            {
                try
                {
                    string web3Address = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
                    web3Address = CustomWeb3System.Instance.ConvertWeb3AddressToShortFormat(web3Address);
                    Text.text = string.Format(SharedConstants.LogoutEthAddress, web3Address);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("RefreshUI() failed " + exception.Message);
                    Text.text = SharedConstants.Authenticate;
                }
            }
            else
            {
                // Fallback
                if (_hasCalledAuthenticate)
                {
                    Text.text = SharedConstants.Authenticate;
                }
                else
                {
                    Text.text = SharedConstants.Loading;
                    IsInteractable = false;
                }
                
            }
        }
		
        //  Event Handlers --------------------------------

    }
}