using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene04_DeveloperConsoleUI"/>
    /// </summary>
    public class Scene04_DeveloperConsole : MonoBehaviour
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene04_DeveloperConsoleUI _ui;

        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(TheGameSingleton_OnTheGameModelChanged);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
  
            RefreshUIAsync();
        }




        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.IsAuthenticatedAsync();
            Debug.Log("isAuthenticated: " + isAuthenticated);
            
            if (!isAuthenticated)
            {
                return;
            }
            
            bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
            Debug.Log("isRegistered: " + isRegistered);

            if (!isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.RegisterAsync();
            }
            else
            {
                // MAKE ABI OBJECTS *BEFORE* CALLING THIS
                //await TheGameSingleton.Instance.TheGameController.UnregisterAsync();
            }
            
            bool isRegistered2 = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
            Debug.Log("isRegistered2: " + isRegistered2);
            
            _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            //
        }

        private async void BackButton_OnClicked()
        {
            SceneManager.LoadSceneAsync("Scene01_Intro", LoadSceneMode.Single);
        }
    }
}
