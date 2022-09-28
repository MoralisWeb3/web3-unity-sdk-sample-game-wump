using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998, 4014, 414
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene05_ViewCollectionUI"/>
    /// </summary>
    public class Scene05_ViewCollection : MonoBehaviour
    {
        //  Properties ------------------------------------

 
        //  Fields ----------------------------------------
        [SerializeField]
        private Scene05_ViewCollectionUI _ui;

        private StringBuilder _titleTextBuilder = new StringBuilder();
        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            bool hasMoralisUser = await MyMoralisWrapper.Instance.HasMoralisUserAsync();
            if (!hasMoralisUser)
            {
                throw new RequiredMoralisUserException();
            }
            
            _ui.BackButtonUI.Button.onClick.AddListener(BackButtonUI_OnClicked);
            
            // Refresh -> CheckRegister -> Refresh Again
            await RefreshUIAsync();
            _titleTextBuilder.Clear();
            _titleTextBuilder.AppendHeaderLine("Collection");
            await TheGameSingleton.Instance.TheGameController.ShowMessagePassiveAsync(
                async delegate ()
                {
                    // Refresh the model
                    bool isRegisteredAsync = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();
                    
                    if (!isRegisteredAsync)
                    {
                        throw new RequiredIsRegisteredException();
                    }

               
                    
                    List<TreasurePrizeDto> treasurePrizeDtos = 
                        await TheGameSingleton.Instance.TheGameController.GetTreasurePrizesAsync();
                    
                    if (treasurePrizeDtos.Count == 0)
                    {
                        _titleTextBuilder.AppendLine("• Your collection is empty");
                        _titleTextBuilder.AppendLine();
                        _titleTextBuilder.AppendLine("Play game to earn prizes!");
                    }
                    else
                    {
                        foreach (TreasurePrizeDto treasurePrizeDto in treasurePrizeDtos)
                        {
                            string goldText = TheGameHelper.FormatTextGold((int)treasurePrizeDto.Price);
                            _titleTextBuilder.AppendLine($"• {treasurePrizeDto.Title} {goldText}");
                        }

                        _titleTextBuilder.AppendLine("");
                        _titleTextBuilder.AppendLine("Selling items is not yet supported.");
                    }
                    
                    //Refresh after async
                    await RefreshUIAsync();
                });
        }

        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            _ui.BackButtonUI.IsInteractable = true;
            _ui.Text.text = _titleTextBuilder.ToString();
        }

        //  Event Handlers --------------------------------
        private void BackButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}