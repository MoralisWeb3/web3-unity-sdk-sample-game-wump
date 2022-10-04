using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.TheGame.MVCS;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.TheGame.Controller
{
    /// <summary>
    /// Core Scene Behavior - Using <see cref="Scene04_DeveloperConsoleUI"/>
    /// </summary>
    public class Scene04_DeveloperConsole : MonoBehaviour
    {
        //  Properties ------------------------------------
 
		
        //  Fields ----------------------------------------
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
            _ui.OutputText.text = _outputTextStringBuilder.ToString();
            //
            _ui.IsAuthenticatedButton.IsInteractable = true;
            _ui.IsRegisteredButton.IsInteractable = _isAuthenticated;
            _ui.RegisterButton.IsInteractable = _isAuthenticated && !_isRegistered;
            _ui.UnregisterButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.TransferGoldButton.IsInteractable = _isAuthenticated && _isRegistered;
            _ui.TransferPrizeButton.IsInteractable = _isAuthenticated && _isRegistered;
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
                    _isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
                    
                    _outputTextStringBuilder.Clear();
                    _outputTextStringBuilder.AppendHeaderLine($"IsAuthenticated()");
                    _outputTextStringBuilder.AppendBullet($"result = {_isAuthenticated}");

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
                    _isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAsync();

                    _outputTextStringBuilder.Clear();
                    _outputTextStringBuilder.AppendHeaderLine($"isRegistered()");
                    _outputTextStringBuilder.AppendBullet($"result = {_isRegistered}");

                    await RefreshUIAsync();
                });
            
            
            await RefreshUIAsync();
        }
        
        
        private async void RegisterButton_OnClicked()
        {
            if (_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(
                    TheGameConstants.MustNotBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.Registering,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheGameController.RegisterAsync();

                        //Populate UI
                        await IsRegisteredButton_OnClicked();
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        
        private async void UnregisterButton_OnClicked()
        {
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.Unregistering,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheGameController.UnregisterAsync();

                        //Populate UI
                        await IsRegisteredButton_OnClicked();
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }

        private async void GetPrizesButton_OnClicked()
        {
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.GettingPrizes,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheGameController.GetPrizesAsync();

                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"GetPrizesAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = see ui above");
       
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }


        
            
        private async void TransferGoldButton_OnClicked()
        {
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.TransferingGold,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheGameController.TransferGoldAsync();

                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"TransferGoldAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = see ui above");
       
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        private async void TransferPrizeButton_OnClicked()
        {
            if (!_isRegistered)
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageCustomAsync(
                    TheGameConstants.MustBeRegistered, 1000);
            }
            else
            {
                await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                    TheGameConstants.TransferingPrize,
                    async delegate ()
                    {
                        await TheGameSingleton.Instance.TheGameController.TransferPrizeAsync();

                        _outputTextStringBuilder.Clear();
                        _outputTextStringBuilder.AppendHeaderLine($"TransferGoldAsync()");
                        _outputTextStringBuilder.AppendBullet($"result = see ui above");
       
                        await RefreshUIAsync();
                    });
            }
            
            await RefreshUIAsync();
        }
        
        private async void SafeReregisterButton_OnClicked()
        {
            await TheGameSingleton.Instance.TheGameController.ShowMessageActiveAsync(
                TheGameConstants.SafeReregistering,
                async delegate ()
                {
                    await TheGameSingleton.Instance.TheGameController.SafeReregisterDeleteAllPrizesAsync();
                    
                    //Populate UI
                    await IsRegisteredButton_OnClicked();
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
