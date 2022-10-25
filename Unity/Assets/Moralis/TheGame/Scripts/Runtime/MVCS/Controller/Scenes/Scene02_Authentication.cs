
using System.Threading.Tasks;
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
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
            
            //
            _isAuthenticatedOnStart = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            _ui.AuthenticationKit.OnConnected.AddListener(AuthenticationUI_OnConnected);
            _ui.AuthenticationKit.OnStateChanged.AddListener(AuthenticationUI_OnStateChanged);

            RefreshUIAsync();
            
            //IF we start the scene NOTE authed, then mimic the button click
            if (!_isAuthenticatedOnStart)
            {
                AuthenticationButton_OnClicked();
            }
        }




        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            if (_ui.AuthenticationKit)
            {
                _ui.AuthenticationKit.gameObject.SetActive(_hasClickedAuthenticate);
            }
            
            _ui.AuthenticationButtonUI.IsInteractable = !_hasClickedAuthenticate;
            _ui.BackButton.IsInteractable = true;
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
                await KLUGE_LogoutAndReloadScene();
            }
            RefreshUIAsync();
        }

        /// <summary>
        /// KLUGE: Ideally one can call Disconnect() and Connect() but that fails. TBD Why.
        /// This is a workaround
        /// </summary>
        private async UniTask KLUGE_LogoutAndReloadScene()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            await CustomWeb3System.Instance.KLUGE_CloseOpenWalletConnection();
            await SharedHelper.DestroyEveryGameObjectInScene();
            TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync();
        }
        
        /// <summary>
        /// KLUGE: Ideally after QR code connects, we will reach AuthenticationKitState.MoralisLoggedIn.
        ///
        /// However, sometimes it requires a refresh
        /// </summary>
        private async UniTask KLUGE_PerhapsAlreadyLoggedIn_SoReloadScene()
        {
            await SharedHelper.DestroyEveryGameObjectInScene();
            TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync();
        }


        //  Event Handlers --------------------------------
        private async void AuthenticationUI_OnStateChanged(AuthenticationKitState authenticationKitState)
        {
            if (authenticationKitState == AuthenticationKitState.Disconnected)
            {
                await KLUGE_PerhapsAlreadyLoggedIn_SoReloadScene();
            }
        }   

        
        private async void AuthenticationUI_OnConnected()
        {
            await LeaveSceneAsync();
        }

        
        private async void BackButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            await LeaveSceneAsync();
        }
    }
}