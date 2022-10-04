using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
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
            _ui.PlayGameButtonUI.Button.onClick.AddListener(PlayGameButtonUI_OnClicked);
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButtonUI_OnClicked);
  
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;
            _ui.PlayGameButtonUI.IsInteractable = isAuthenticated;
            _ui.AuthenticationButtonUI.IsInteractable = true;
        }

        //  Event Handlers --------------------------------

        private async void AuthenticationButtonUI_OnClicked()
        {
            SceneManager.LoadSceneAsync("Scene02_Authentication", LoadSceneMode.Single);
        }
   
        
        private async void PlayGameButtonUI_OnClicked()
        {
            SceneManager.LoadSceneAsync("Scene03_Game", LoadSceneMode.Single);
        }
    }
}