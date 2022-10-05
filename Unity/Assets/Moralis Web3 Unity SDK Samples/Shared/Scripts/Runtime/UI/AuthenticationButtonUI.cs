using System.Threading.Tasks;
using MoralisUnity.Sdk.Utilities;

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
            _isAuthenticated = await MyMoralisWrapper.Instance.IsAuthenticatedAsync();
            await RefreshUI();
        }
        
        private async Task RefreshUI()
        {
            if (_isAuthenticated)
            {
                Text.text = SharedConstants.Logout;
                string moralisUserEthAddressAsync = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
                moralisUserEthAddressAsync = Formatters.GetWeb3AddressShortFormat(moralisUserEthAddressAsync);
                Text.text = string.Format(SharedConstants.LogoutEthAddress, moralisUserEthAddressAsync);
            }
            else
            {
                Text.text = SharedConstants.Authenticate;
            }
        }
		
        //  Event Handlers --------------------------------

    }
}