using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene04_DeveloperConsoleUI"/>
    /// </summary>
    public class Scene04_DeveloperConsole : MonoBehaviour
    {
        //  Properties ------------------------------------
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private Scene04_DeveloperConsoleUI _ui;
        
        private bool _isAuthenticated = false;
        private bool _isRegistered = false;
        private StringBuilder _outputTextStringBuilder = new StringBuilder();
        
        //  Unity Methods----------------------------------
        protected async void Start()
        {
            _ui.IsAuthenticatedButton.Button.onClick.AddListener( async () => await IsAuthenticatedButton_OnClicked());
            _ui.IsRegisteredButton.Button.onClick.AddListener(async () => await IsRegisteredButton_OnClicked());
            _ui.RegisterButton.Button.onClick.AddListener(RegisterButton_OnClicked);
            _ui.UnregisterButton.Button.onClick.AddListener(UnregisterButton_OnClicked);
            _ui.GetPrizesButton.Button.onClick.AddListener(GetPrizesButton_OnClicked);
            _ui.GetTransferLogHistoryButton.Button.onClick.AddListener(GetTransferLogHistoryButton_OnClicked);
            _ui.TransferGoldButton.Button.onClick.AddListener(TransferGoldButton_OnClicked);
            _ui.TransferPrizeButton.Button.onClick.AddListener(TransferPrizeButton_OnClicked);
            _ui.SafeReregisterButton.Button.onClick.AddListener(SafeReregisterButton_OnClicked);
            _ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
            //
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(TheGameSingleton_OnTheGameModelChanged);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
  
            // Mimic button clicks
            await RefreshUIAsync();
            await IsAuthenticatedButton_OnClicked();
            if (_isAuthenticated)
            {
                await IsRegisteredButton_OnClicked();
            }
            await RefreshUIAsync();
        }

        //  General Methods -------------------------------
        private async UniTask RefreshUIAsync()
        {
            // Header content
            StringBuilder summary = new StringBuilder();
            summary.AppendLine($"isAuthenticated = {_isAuthenticated}. isRegistered = {_isRegistered}");
            
            // Recent content
            summary.Append(_outputTextStringBuilder);
            _ui.OutputText.text = summary.ToString();
            
            
            //
            _ui.IsAuthenticatedButton.IsInteractable = true;
            _ui.IsRegisteredButton.IsInteractable = _isAuthenticated;
            _ui.RegisterButton.IsInteractable = _isAuthenticated && !_isRegistered;
            _ui.UnregisterButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.GetPrizesButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.GetTransferLogHistoryButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.TransferGoldButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.TransferPrizeButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.SafeReregisterButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.BackButton.IsInteractable = true;
        }
        
        
        //  Event Handlers --------------------------------
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            //
        }
        
        
        private async UniTask IsAuthenticatedButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            await TheGameSingleton.Instance.TheGameController.ShowMessagePassiveAsync(
                async delegate ()
                {
                    //TODO: Maybe move ALL local isAuth bool into the TheGameModel instead?
                    _isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();

                    await RefreshUIAsync();
                });
            
            
            await RefreshUIAsync();

        }
        
        
        private async UniTask IsRegisteredButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            await TheGameSingleton.Instance.TheGameController.ShowMessagePassiveAsync(
                async delegate ()
                {
                    // Will Also Populate the Top UI
                    _isRegistered = await TheGameSingleton.Instance.TheWeb3Controller.GetIsRegisteredAndUpdateModelAsync();

                    await RefreshUIAsync();
                });
            
            
            await RefreshUIAsync();
        }
        
        
        private async void RegisterButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustNotBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.Registering,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheWeb3Controller.RegisterAndUpdateModelAsync();

                        //Populate UI
                        await IsRegisteredButton_OnClicked();
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        
        private async void UnregisterButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.Unregistering,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheWeb3Controller.UnregisterAsync();

                        //Populate UI
                        await IsRegisteredButton_OnClicked();
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }

        private async void GetPrizesButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.GettingPrizes,
                    async delegate ()
                    {
                        List<Prize> prizes = await TheGameSingleton.Instance.TheWeb3Controller.GetPrizesAndUpdateModelAsync();

                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"GetPrizesAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = {prizes.Count}");
       
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }

        
        private async void GetTransferLogHistoryButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.GettingTransferLogHistory,
                    async delegate ()
                    {
                        TransferLog transferLog = await TheGameSingleton.Instance.TheWeb3Controller.GetTransferLogHistoryAsync();

                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"GetTransferLogHistoryAsync()");

                        if (transferLog == null)
                        {
                            _outputTextStringBuilder.AppendBullet($"result = Null");
                        }
                        else
                        {
                            _outputTextStringBuilder.AppendBullet($"result = {TheGameHelper.GetTransferLogDisplayText(transferLog)}");
                        }
                        
       
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }


        
        private async void TransferGoldButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.TransferingGold,
                    async delegate ()
                    {
                        int gold = await TheGameSingleton.Instance.TheWeb3Controller.GetGoldAndUpdateModelAsync();
                        if (gold < TheGameConstants.GoldOnTransfer)
                        {
                            Debug.LogWarning($"TransferPrizeAsync() failed. gold = {gold}.");
                        }

                        string fakeAddress = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e";
                        await TheGameSingleton.Instance.TheWeb3Controller.TransferGoldAsync(fakeAddress);

                        // Again Update The Model
                        await TheGameSingleton.Instance.TheWeb3Controller.GetGoldAndUpdateModelAsync();
                        
                        // UI
                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"TransferGoldAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = See Game UI");
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        private async void TransferPrizeButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.TransferingPrize,
                    async delegate ()
                    {
                        List<Prize> prizes = await TheGameSingleton.Instance.TheWeb3Controller.GetPrizesAndUpdateModelAsync();
                        if (prizes.Count < TheGameConstants.PrizesOnTransfer)
                        {
                            Debug.LogWarning($"TransferPrizeAsync() failed. prizes.Count = {prizes.Count}.");
                        }
                        
                        string fakeAddress = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e";
                        await TheGameSingleton.Instance.TheWeb3Controller.TransferPrizeAsync(fakeAddress, prizes[0]);

                        // Again Update The Model
                        await TheGameSingleton.Instance.TheWeb3Controller.GetPrizesAndUpdateModelAsync();
                        
                        // UI
                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"TransferGoldAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = See Game UI");
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        private async void SafeReregisterButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                TheGameConstants.SafeReregistering,
                async delegate ()
                {
                    await TheGameSingleton.Instance.TheWeb3Controller.SafeReregisterDeleteAllPrizesAsync();
                    
                    //Populate UI
                    await IsRegisteredButton_OnClicked();
                    await RefreshUIAsync();
                });
            
            
            await RefreshUIAsync();
        }

        private async void BackButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.PlayAudioClipClick();

            
            TheGameSingleton.Instance.TheGameController.LoadSettingsSceneAsync();
        }
    }
}
