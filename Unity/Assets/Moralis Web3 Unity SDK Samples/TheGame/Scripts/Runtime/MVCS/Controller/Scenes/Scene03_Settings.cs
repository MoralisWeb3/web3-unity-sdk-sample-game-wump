using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998, CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes
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

        private CustomPlayerInfo _customPlayerInfo = new CustomPlayerInfo();
        private bool _isAuthenticated = false;
        private bool _isRegistered = false;

        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.PlayerView.PlayerNameText.text = TheGameHelper.SetPlayerTextLikeMenuHeading("Settings"); 
            _ui.RandomizeNicknameButton.Button.onClick.AddListener(RandomizeNicknameButton_OnClicked);
            _ui.ResetButton.Button.onClick.AddListener(ResetButton_OnClicked);
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(TheGameSingleton_OnTheGameModelChanged);
            
            RefreshUIAsync();
            _isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            if (_isAuthenticated)
            {
                // Populate the top UI
                _isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
                RefreshUIAsync();
            }
        }

        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            string nickname = "";
            if (_customPlayerInfo.HasNickname)
            {
                nickname = $"({_customPlayerInfo.Nickname})";
            }
            _ui.RandomizeNicknameButton.Text.text = $"Randomize Nickname {nickname}";
            _ui.RandomizeNicknameButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.ResetButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            _customPlayerInfo = theGameModel.CustomPlayerInfo.Value;
            RefreshUIAsync();
        }
        
        
        private async void RandomizeNicknameButton_OnClicked()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Debug.LogWarning("User held SHIFT key during button click. Opening secret menu.");
                //Secret Menu
                TheGameSingleton.Instance.TheGameController.LoadDeveloperConsoleSceneAsync();
            }
            else
            {
                //Normal
                TheGameSingleton.Instance.TheGameController.RandomizeNicknameAndUpdateModel();
            }
            
        }
        
        
        private async void ResetButton_OnClicked()
        {
            await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                TheGameConstants.SafeReregistering,
                async delegate ()
                {
                    await TheGameSingleton.Instance.TheGameController.SafeReregisterDeleteAllPrizesAsync();
                    await RefreshUIAsync();
                });
            
            await RefreshUIAsync();
        }

        
        private async void BackButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}
