using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998, 4014
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene03_SettingsUI"/>
    /// </summary>
    public class Scene03_Settings : MonoBehaviour
    {
        //  Properties ------------------------------------
 
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene03_SettingsUI _ui;

        //  Unity Methods----------------------------------
        protected async void Start()
        {
            bool hasMoralisUserAsync = await MyMoralisWrapper.Instance.HasMoralisUserAsync();
            if (!hasMoralisUserAsync)
            {
                throw new RequiredMoralisUserException();
            }

            _ui.ResetAllDataButtonUI.Button.onClick.AddListener(ResetAllDataButtonUI_OnClicked);
            _ui.BackButtonUI.Button.onClick.AddListener(BackButtonUI_OnClicked);
            
            // Refresh -> CheckRegister -> Refresh Again
            RefreshUIAsync();
            await TheGameSingleton.Instance.TheGameController.ShowMessagePassiveAsync(
                async delegate()
                {
                    // Refresh the model
                    bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
                    
                    if (!isRegistered)
                    {
                        throw new RequiredIsRegisteredException();
                    }

                    //Refresh after async
                    RefreshUIAsync();
                    
                });

            
        }
        
        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            _ui.BackButtonUI.IsInteractable = true; 
        }
        
        private async void ResetAllDataAsync()
        {
            await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                TheGameConstants.Resetting,
                async delegate ()
                {
                    await TheGameSingleton.Instance.TheGameController.SafeReregisterDeleteAllTreasurePrizeAsync();
                    
                    // Refresh the model
                    bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
                    
                    if (!isRegistered)
                    {
                        throw new RequiredIsRegisteredException();
                    }

                    //Refresh after async
                    await RefreshUIAsync();
                });
            
        }

        //  Event Handlers --------------------------------


        private void ResetAllDataButtonUI_OnClicked()
        {
            if (Input.GetKey(KeyCode.Space) 
                || Input.GetKey(KeyCode.RightShift) 
                || Input.GetKey(KeyCode.LeftShift))
            {
                // This is a secret menu for developers
                Custom.Debug.LogWarning("SpaceBar Held. Will Open Developer Console");
                TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
                TheGameSingleton.Instance.TheGameController.LoadDeveloperConsoleSceneAsync();
            }
            else
            {
                Custom.Debug.LogWarning("SpaceBar NOT Held. Will Reset All Data");
                TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
                ResetAllDataAsync();
            }
        }
        
        private void BackButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}