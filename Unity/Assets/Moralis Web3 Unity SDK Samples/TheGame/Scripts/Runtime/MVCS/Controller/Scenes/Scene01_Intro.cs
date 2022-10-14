using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998, CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene01_IntroUI"/>
    /// </summary>
    public class Scene01_Intro : MonoBehaviour
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene01_IntroUI _ui;

        private bool _isAuthenticated = false;
        private bool _isRegistered = false;


        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.WalletConnectWrapper.EnsureWalletConnectExists();
            _ui.PlayerView.PlayerNameText.text = TheGameHelper.SetPlayerTextLikeMenuHeading("Intro"); 
            
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButtonUI_OnClicked);
            _ui.Registerbutton.Button.onClick.AddListener(Registerbutton_OnClicked);
            _ui.PlayGameButton.Button.onClick.AddListener(PlayGameButtonUI_OnClicked);
            _ui.SettingsButton.Button.onClick.AddListener(SettingsButton_OnClicked);
            
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChange);
            
            RefreshUIAsync();
            
            _isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;
            if (_isAuthenticated)
            {
                _isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
            }
            
            RefreshUIAsync();
        }



        protected void OnDestroy()
        {
            _ui.WalletConnectWrapper.EnsureWallectConnectIsDestroyed();
        }

        

        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            _ui.AuthenticationButtonUI.IsInteractable = true;
            _ui.Registerbutton.IsInteractable = _isAuthenticated && !_isRegistered;
            _ui.PlayGameButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.SettingsButton.IsInteractable = _isAuthenticated && _isRegistered;
           
        }

        //  Event Handlers --------------------------------
        private async void AuthenticationButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync();
        }
        
        
        private async void PlayGameButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadGameSceneAsync();
        }

        
        private async void Registerbutton_OnClicked()
        {

            await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                TheGameConstants.Registering,
                async delegate()
                {
                    //After this happens, the OnTheGameModelChange() will be called automatically
                    await TheGameSingleton.Instance.TheGameController.RegisterAndUpdateModelAsync();
                    
                    //Registering is crucial. Ensure the user sees the result with some extra stuff here...
                    await UniTask.Delay(3000);
                    _isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
                    RefreshUIAsync();

                });
        }

        
        private void OnTheGameModelChange(TheGameModel theGameModel)
        {
            _isRegistered = theGameModel.IsRegistered.Value;
            RefreshUIAsync();
        }
        
        
        private async void SettingsButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }
    }
}