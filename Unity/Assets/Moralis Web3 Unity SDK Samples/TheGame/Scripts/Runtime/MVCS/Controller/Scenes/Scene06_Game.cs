using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.Exceptions;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;
using UnityEngine;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes;

#pragma warning disable 1998, 4014, 414
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene06_GameUI"/>
    /// </summary>
    public class Scene06_Game : MonoBehaviour
    {
        //  Properties ------------------------------------
        private bool _isReadyForUserToClickPlay  {  get { return _observableGameState.Value == GameState.WaitForUser; } }

        //  Fields ----------------------------------------
        [Header("Debugging")] [SerializeField]
        private ObservableGameState _observableGameState = new ObservableGameState();

        [Header("References (Scene)")] [SerializeField]
        private Scene06_GameUI _ui;

        [Header("Reference Points (Scene)")]
        private readonly List<int> _goldCostPerPlay = new List<int> { 10, 30, 150 };

        private int _lastGoldOwned = 0;
        private int _lastGoldSpent = 0;
        private Reward _lastReward;
        
        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            // 1. Listen to back button
            _ui.Play01ButtonUI.Button.onClick.AddListener(Play01ButtonUI_OnClicked);
            _ui.Play02ButtonUI.Button.onClick.AddListener(Play02ButtonUI_OnClicked);
            _ui.Play03ButtonUI.Button.onClick.AddListener(Play03ButtonUI_OnClicked);
            _ui.BackButtonUI.Button.onClick.AddListener(BackButtonUI_OnClicked);

            // Dynamic text
            string goldText0 = TheGameHelper.FormatTextGold(_goldCostPerPlay[0]);
            TheGameHelper.SetButtonText(_ui.Play01ButtonUI.Button, $"Play {goldText0}");
            
            string goldText1 = TheGameHelper.FormatTextGold(_goldCostPerPlay[1]);
            TheGameHelper.SetButtonText(_ui.Play02ButtonUI.Button, $"Play {goldText1}");
            
            string goldText2 = TheGameHelper.FormatTextGold(_goldCostPerPlay[2]);
            TheGameHelper.SetButtonText(_ui.Play03ButtonUI.Button, $"Play {goldText2}");

            // 2. Check for user
            bool hasMoralisUser = await MyMoralisWrapper.Instance.HasMoralisUserAsync();
            if (!hasMoralisUser)
            {
                throw new RequiredMoralisUserException();
            }

            // AddListener - Update View When Model Changes
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChanged);
            _observableGameState.OnValueChanged.AddListener(ObservableGameState_OnValueChanged);
            
            //Refresh before async
            _observableGameState.Value = GameState.Null;
            RefreshUIAsync();
            
            //Do async
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
        private async void StartGame(int goldSpent)
        {
            _lastGoldSpent = goldSpent;
            _observableGameState.Value = GameState.TreasureChestPreOpening;

        }

        private async UniTask RefreshUIAsync()
        {
            _ui.BackButtonUI.IsInteractable = true;
            _ui.Play01ButtonUI.IsInteractable = _isReadyForUserToClickPlay && _lastGoldOwned >= _goldCostPerPlay[0];
            _ui.Play02ButtonUI.IsInteractable = _isReadyForUserToClickPlay && _lastGoldOwned >= _goldCostPerPlay[1];
            _ui.Play03ButtonUI.IsInteractable = _isReadyForUserToClickPlay && _lastGoldOwned >= _goldCostPerPlay[2];
        }


        //  Event Handlers --------------------------------
        private async void ObservableGameState_OnValueChanged(GameState gameState)
        {
            //Debug.Log("gameState: " + gameState);
            switch (gameState)
            {
                case GameState.Null:
                    _observableGameState.Value = GameState.TreasureChestEntering;
                    break;
                case GameState.TreasureChestEntering:
                    _observableGameState.Value = GameState.TreasureChestEntered;
                    break;
                case GameState.TreasureChestEntered:
                    _observableGameState.Value = GameState.TreasureChestIdle;
                    break;
                case GameState.TreasureChestIdle:
                    _observableGameState.Value = GameState.WaitForUser;
                    break;
                case GameState.WaitForUser:
                    //wait for user click
                    break;
                case GameState.TreasureChestPreOpening:

                    await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                        TheGameConstants.Opening,
                        async delegate()
                        {
                            
                            // Toggle auto update. This builds suspense for user
                            _ui.IsObservingOnTheGameModelChanged = false;
                            
                            _lastReward = await TheGameSingleton.Instance.TheGameController.StartGameAndGiveRewardsAsync(_lastGoldSpent);
                            if (_lastReward == null)
                            {
                                //UI Button toggling will likely prevent this...
                                Custom.Debug.LogWarning("Not enough gold. visit 'View Collection' and sell something.");
                            }
                            else
                            {
                                _observableGameState.Value = GameState.TreasureChestOpening;
                            }
                            
                            // Refresh the data - Don't await this
                            TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();

                            // Refresh the ui
                            await RefreshUIAsync();

                        });
                    break;
                case GameState.TreasureChestOpening:
                    _observableGameState.Value = GameState.TreasureChestOpened;
                    break;
                case GameState.TreasureChestOpened:
                    _observableGameState.Value = GameState.CardsEntering;
                    break;
                case GameState.CardsEntering:
                    
                    TheGameSingleton.Instance.TheGameController.PlayAudioClip(TheGameHelper.GetAudioClipIndexWinSound());
                    
                    _observableGameState.Value = GameState.CardsEntered;
                    break;
                case GameState.CardsEntered:
                    _observableGameState.Value = GameState.CardsIdle;
                    break;

                case GameState.CardsIdle:

                    string theTypeName = TheGameHelper.GetRewardTypeNameByType(_lastReward.Type);
                    string message =
                        $"Congratulations!\nYou Spent {_lastGoldSpent} and\nwon `{theTypeName}` worth {_lastReward.Price}.";

                    // Do NOT await
                    TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(message,
                        5000);
                    
                    TheGameSingleton.Instance.TheGameController.PlayAudioClip(TheGameHelper.GetAudioClipIndexByReward(_lastReward));
                    await UniTask.Delay((500));
                    
                    // Toggle auto update. This builds suspense for user
                    _ui.IsObservingOnTheGameModelChanged = true;
                    
                    break;
            }
        }


        // Event Handlers ---------------------------------
        private async void OnTheGameModelChanged(TheGameModel theGameModel)
        {
            _lastGoldOwned = theGameModel.Gold.Value;
            await RefreshUIAsync();
        }


        private void Play01ButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            StartGame(_goldCostPerPlay[0]);
        }

        private void Play02ButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            StartGame(_goldCostPerPlay[1]);
        }


        private void Play03ButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            StartGame(_goldCostPerPlay[2]);
        }



        private void BackButtonUI_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
        }
    }
}
