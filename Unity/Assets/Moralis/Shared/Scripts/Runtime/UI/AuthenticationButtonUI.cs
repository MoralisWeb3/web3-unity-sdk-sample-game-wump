using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

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

        public async UniTask<bool> IsAuthenticatedAsync()
        {
            if (!_isAuthenticated)
            {
                await CustomWeb3System.Instance.AuthenticateAsync();
                _isAuthenticated = await CustomWeb3System.Instance.IsAuthenticatedAsync();
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
            _isAuthenticated = await CustomWeb3System.Instance.IsAuthenticatedAsync();
            await RefreshUI();
        }
        
        private async Task RefreshUI()
        {
            if (_isAuthenticated)
            {
                Text.text = SharedConstants.Logout;
                string web3Address = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
                web3Address = CustomWeb3System.Instance.ConvertWeb3AddressToShortFormat(web3Address);
                Text.text = string.Format(SharedConstants.LogoutEthAddress, web3Address);
            }
            else
            {
                Text.text = SharedConstants.Authenticate;
            }
        }
		
        //  Event Handlers --------------------------------

    }
}