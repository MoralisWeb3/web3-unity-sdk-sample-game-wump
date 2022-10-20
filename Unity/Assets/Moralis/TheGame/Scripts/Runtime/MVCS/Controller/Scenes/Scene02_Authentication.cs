using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
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
        [Header ("References (Scene)")]
        [SerializeField]
        private Scene02_AuthenticationUI _ui;

        private bool _isAuthenticatedOnStart = false;

        //  Unity Methods----------------------------------

        protected async void Start()
        {
            _ui.AuthenticationKit.gameObject.SetActive(true);
            
            _ui.PlayerView.PlayerNameText.text = TheGameHelper.GetPlayerNameAsSceneTitle("Web3 Auth", 5); 
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
                _ui.AuthenticationKit.gameObject.SetActive(true);
                _ui.AuthenticationKit.OnConnected.AddListener(AuthenticationUI_OnConnected);
            }
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            _ui.CancelButton.IsInteractable = true;
        }
        
        private async UniTask LeaveSceneAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            //Debug.Log($"CancelButton_OnClicked() wasA = {_isAuthenticatedOnStart}, isA = {isAuthenticated}");
            
            // Stop any processes
            Destroy(_ui.gameObject);
            
            // Leave
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
        


        //  Event Handlers --------------------------------
        private async void CancelButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            await LeaveSceneAsync();

        }
        
        
        private async void AuthenticationUI_OnConnected()
        {
            await LeaveSceneAsync();
        }

        
        private async void AuthenticationUI_OnDisconnected()
        {
            await LeaveSceneAsync();
        }
    }
}