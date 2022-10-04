using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene03_SettingsUI"/>
    /// </summary>
    public class Scene03_Settings : Scene_UIWithTop
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene03_SettingsUI _ui;

        
        //  Unity Methods----------------------------------
        protected override async void Start()
        {
           // _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
  
            await RefreshUIAsync();
        }


        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
           // _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------

        private async void BackButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}
