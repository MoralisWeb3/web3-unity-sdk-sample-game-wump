using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller
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
            _ui.ViewCollectionButtonUI.Button.onClick.AddListener(ViewCollectionButtonUI_OnClicked);
            _ui.AuthenticationButtonUI.Button.onClick.AddListener(AuthenticationButtonUI_OnClicked);
            _ui.SettingsButtonUI.Button.onClick.AddListener(SettingsButtonUI_OnClicked);
  
          
            bool isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;
            if (isAuthenticated)
            {
                TheGameSingleton.Debug.Log($"isAuthenticated = {isAuthenticated}");
                bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
                
                if (!isRegistered)
                {
                    // Refresh -> CheckRegister -> Refresh Again
                    RefreshUIAsync();
                    await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                        TheGameConstants.Registering,
                        async delegate()
                        {
                            // Wait extra for wallet connect
                            await UniTask.Delay(1000);
                            
                            // Refresh the model
                            await TheGameSingleton.Instance.TheGameController.RegisterAsync();

                            //Refresh after async
                            RefreshUIAsync();
                    
                        });
                }

            }
            else
            {
                TheGameSingleton.Debug.Log($"isAuthenticated = {isAuthenticated}. User must click authenticate.");
            }
            
            RefreshUIAsync();

        }




        //  General Methods -------------------------------
        private async void RefreshUIAsync()
        {
            bool isAuthenticated = _ui.AuthenticationButtonUI.IsAuthenticated;
            _ui.PlayGameButtonUI.IsInteractable = isAuthenticated;
            _ui.ViewCollectionButtonUI.IsInteractable = isAuthenticated;
            _ui.SettingsButtonUI.IsInteractable = isAuthenticated;
            _ui.AuthenticationButtonUI.IsInteractable = true;
            
        }

        //  Event Handlers --------------------------------

        private async void AuthenticationButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            TheGameSingleton.Instance.TheGameController.LoadAuthenticationSceneAsync(); 
        }
   
        
        private async void ViewCollectionButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            TheGameSingleton.Instance.TheGameController.LoadViewCollectionSceneAsync();
        }
        
        
        private async void SettingsButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }


        private async void PlayGameButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            TheGameSingleton.Instance.TheGameController.LoadGameSceneAsync();
        }
    }
}