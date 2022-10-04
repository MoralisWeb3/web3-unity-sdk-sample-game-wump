using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.View;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene02_AuthenticationUI"/>
    /// </summary>
    public class Scene02_Authentication : MonoBehaviour
    {
        //  Properties ------------------------------------


        //  Fields ----------------------------------------
        [SerializeField]
        private Scene02_AuthenticationUI _ui;

        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
  
            RefreshUIAsync();

            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.IsAuthenticatedAsync();
            if (isAuthenticated)
            {
                _ui.MyAuthenticationKitWrapper.OnDisconnected.AddListener(AuthenticationUI_OnDisconnected);
            }
            else
            {
                _ui.MyAuthenticationKitWrapper.OnConnected.AddListener(AuthenticationUI_OnConnected);
            }
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.IsAuthenticatedAsync();
            _ui.CancelButton.IsInteractable = true;
        }

        //  Event Handlers --------------------------------
        private void CancelButton_OnClicked()
        {
            // Stop any processes
            Destroy(_ui.gameObject);
            
            // Leave
            SceneManager.LoadSceneAsync("Scene01_Intro", LoadSceneMode.Single);
        }
        
        
        private void AuthenticationUI_OnConnected()
        {
            Debug.Log("AuthenticationUI_OnConnected");
            CancelButton_OnClicked();
        }

        
        private void AuthenticationUI_OnDisconnected()
        {
            Debug.Log("AuthenticationUI_OnConnected");
            CancelButton_OnClicked();
        }
    }
}