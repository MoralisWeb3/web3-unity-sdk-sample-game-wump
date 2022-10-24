
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
        private bool _hasClickedAuthenticate = false;

        //  Unity Methods----------------------------------

        protected async void Start()
        {
            RefreshUIAsync();
            
            //
            _ui.PlayerView.PlayerNameText.text = TheGameHelper.GetPlayerNameAsSceneTitle("Web3 Auth", 5); 
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButton_OnClicked);
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
            
            //
            _isAuthenticatedOnStart = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            _ui.AuthenticationKit.OnConnected.AddListener(AuthenticationUI_OnConnected);
            RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            if (_ui.AuthenticationKit)
            {
                _ui.AuthenticationKit.gameObject.SetActive(_hasClickedAuthenticate);
            }
            
            _ui.AuthenticationButtonUI.IsInteractable = !_hasClickedAuthenticate;
            _ui.CancelButton.IsInteractable = true;
        }
        
        private async UniTask LeaveSceneAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            //Debug.Log($"CancelButton_OnClicked() wasA = {_isAuthenticatedOnStart}, isA = {isAuthenticated}");
            
            // Stop any processes
            if (_ui && _ui.gameObject)
            {
                Destroy(_ui.gameObject);
            }
            
            // Leave
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
        

        private async void AuthenticationButton_OnClicked()
        {
            _hasClickedAuthenticate = true;
            if (_isAuthenticatedOnStart)
            {
                // Logout
                TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
                await CustomWeb3System.Instance.ClearActiveSessionAsync();
                await CustomWeb3System.Instance.CloseActiveSessionAsync();
                await SharedHelper.DestroyEveryGameObjectInScene();

                TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync();
            }
            RefreshUIAsync();
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