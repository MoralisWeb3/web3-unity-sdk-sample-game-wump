using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
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

        private string _nickName = "";
        private bool _isAuthenticated = false;
        private bool _isRegistered = false;

        //  Unity Methods----------------------------------
        protected async void Start()
        { 
            _ui.RandomizeNicknameButton.Button.onClick.AddListener(RandomizeNicknameButton_OnClicked);
            _ui.ResetButton.Button.onClick.AddListener(ResetButton_OnClicked);
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(TheGameSingleton_OnTheGameModelChanged);
            
            RefreshUIAsync();
            _isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
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
            _ui.RandomizeNicknameButton.Text.text = $"Randomize Nickname <size=30>({_nickName})</size>";
            _ui.RandomizeNicknameButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.ResetButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            _nickName = theGameModel.CustomPlayerInfo.Value.Nickname.Value;
            RefreshUIAsync();
        }
        
        
        private async void RandomizeNicknameButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.RandomizeNicknameAndUpdateModel();
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
