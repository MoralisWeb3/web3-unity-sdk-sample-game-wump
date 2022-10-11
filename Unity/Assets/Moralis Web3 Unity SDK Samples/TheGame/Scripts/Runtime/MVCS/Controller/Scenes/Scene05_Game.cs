using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene05_GameUI"/>
    /// </summary>
    public class Scene05_Game : MonoBehaviour
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene05_GameUI _ui;

        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
  
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
            if (isAuthenticated)
            {
                // Populate the top UI
                bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
            }
        }
        
        
        //  Event Handlers --------------------------------

        private async void BackButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}
