using Cysharp.Threading.Tasks;
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
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButtonUI_OnClicked);
            _ui.Registerbutton.Button.onClick.AddListener(Registerbutton_OnClicked);
            _ui.PlayGameButton.Button.onClick.AddListener(PlayGameButtonUI_OnClicked);
            _ui.SettingsButton.Button.onClick.AddListener(SettingsButton_OnClicked);
            
            RefreshUIAsync();
            
            _isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;
            if (_isAuthenticated)
            {
                _isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
            }
            
            RefreshUIAsync();
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
                    await TheGameSingleton.Instance.TheGameController.RegisterAsync();

                    await RefreshUIAsync();
                });
        }

        
        private async void SettingsButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }
    }
}