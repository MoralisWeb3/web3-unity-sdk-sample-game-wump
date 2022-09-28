using System.Threading.Tasks;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared.UI
{
    /// <summary>
    /// The UI for shared use.
    /// </summary>
    public class AuthenticationButtonUI : BaseButtonUI
    {
        //  Properties  ---------------------------------------
        public bool IsAuthenticated { get { return _hasMoralisUser;}}
      
        //  Fields  ---------------------------------------
        private bool _hasMoralisUser = false;
        
        //  Unity Methods  --------------------------------
        public async void Awake()
        {
            await CheckHasMoralisUserAsync();
        }
        
        //  General Methods -------------------------------
        private async Task CheckHasMoralisUserAsync()
        {
            _hasMoralisUser = await MyMoralisWrapper.Instance.HasMoralisUserAsync();
            await RefreshUI();
        }
        
        private async Task RefreshUI()
        {
            if (_hasMoralisUser)
            {
                Text.text = SharedConstants.Logout;
            }
            else
            {
                Text.text = SharedConstants.Authenticate;
            }
        }
		
        //  Event Handlers --------------------------------

    }
}