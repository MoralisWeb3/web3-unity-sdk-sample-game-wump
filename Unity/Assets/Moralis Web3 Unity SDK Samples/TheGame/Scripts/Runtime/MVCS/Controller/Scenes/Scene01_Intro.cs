using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998
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

        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.PlayGameButton.Button.onClick.AddListener(PlayGameButtonUI_OnClicked);
            _ui.SettingsButton.Button.onClick.AddListener(SettingsButton_OnClicked);
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButtonUI_OnClicked);
  
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;

            _ui.PlayGameButton.IsInteractable = isAuthenticated;
            _ui.SettingsButton.IsInteractable = isAuthenticated;
            _ui.AuthenticationButtonUI.IsInteractable = true;
            
            if (isAuthenticated)
            {
                // Populate the top UI
                await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
            }
        }

        //  Event Handlers --------------------------------

        private async void PlayGameButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadGameSceneAsync();
        }
   
        
        private async void SettingsButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }
        
        
        private async void AuthenticationButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync();
        }
    }
}