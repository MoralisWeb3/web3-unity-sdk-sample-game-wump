using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene03_SettingsUI"/>
    /// </summary>
    public class Scene03_Settings : Scene_UIWithTop
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene03_SettingsUI _ui;

        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
           // _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
  
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.IsAuthenticatedAsync();
           // _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------

        private async void BackButton_OnClicked()
        {
            SceneManager.LoadSceneAsync("Scene01_Intro", LoadSceneMode.Single);
        }
    }
}
