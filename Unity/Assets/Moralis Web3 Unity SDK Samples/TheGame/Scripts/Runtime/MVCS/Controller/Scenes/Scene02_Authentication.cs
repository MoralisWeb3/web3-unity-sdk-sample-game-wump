using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes
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

        private bool _isAuthenticatedOnStart = false;

        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
            
            RefreshUIAsync();

            _isAuthenticatedOnStart = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            if (_isAuthenticatedOnStart)
            {
                //BUG: Mysteriously the AuthenticationKit will DISCONNECT in this situation
                
                //HACK: So wait one frame, then trigger the desired game-specific response as
                //      if the user clicked a non-existent 'disconnect' button
                await UniTask.NextFrame();
                AuthenticationUI_OnDisconnected();
            }
            else
            {
                _ui.MyAuthenticationKitWrapper.OnConnected.AddListener(AuthenticationUI_OnConnected);
            }
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            _ui.CancelButton.IsInteractable = true;
        }

        //  Event Handlers --------------------------------
        private async void CancelButton_OnClicked()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            //Debug.Log($"CancelButton_OnClicked() wasA = {_isAuthenticatedOnStart}, isA = {isAuthenticated}");
            
            // Stop any processes
            Destroy(_ui.gameObject);
            
            // Leave
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
        
        
        private async void AuthenticationUI_OnConnected()
        {
            //Debug.Log($"AuthenticationUI_OnConnected()");
            CancelButton_OnClicked();
        }

        
        private void AuthenticationUI_OnDisconnected()
        {
           // Debug.Log("AuthenticationUI_OnConnected");
            CancelButton_OnClicked();
        }
    }
}