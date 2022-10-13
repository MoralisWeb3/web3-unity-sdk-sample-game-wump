using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.View.UI;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 1998, CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.View
{
    /// <summary>
    /// The UI for Core Scene Behavior for transferring between players
    /// </summary>
    public class TransferDialogView : MonoBehaviour
    {
        //  Properties ------------------------------------
        [HideInInspector]
        public UnityEvent OnTransferGoldRequested = new UnityEvent();
        
        [HideInInspector]
        public UnityEvent OnTransferPrizeRequested = new UnityEvent();

        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]
        
        [SerializeField]
        private TransferDialogViewUI _ui = null;

        private bool _isAuthenticated = false;
        private bool _isRegistered = false;
        private string _fromAddress = "";
        private string _toAddress = "";
        
        //  Unity Methods----------------------------------
        protected async void Start()
        { 
            _ui.TransferGoldButton.Button.onClick.AddListener(TransferGoldButton_OnClicked);
            _ui.TransferPrizeButton.Button.onClick.AddListener(TransferPrizeButton_OnClicked);
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
            //
            TheGameSingleton.Instance.TheGameController.RegisterTransferDialogView(this);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(TheGameSingleton_OnTheGameModelChanged);
            TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
            
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
            _ui.TransferGoldButton.IsInteractable = _isAuthenticated
                                                    && _isRegistered
                                                    && TheGameSingleton.Instance.TheGameController
                                                        .CanTransferGoldToSelected();

            _ui.TransferPrizeButton.IsInteractable = _isAuthenticated
                                                     && _isRegistered
                                                     && TheGameSingleton.Instance.TheGameController
                                                         .CanTransferPrizeToSelected();

            if (TheGameSingleton.Instance.TheGameController.CanTransferGoldToSelected() ||
                TheGameSingleton.Instance.TheGameController.CanTransferPrizeToSelected())
            {
                _ui.OutputText.text = $"Transfer?\n from : {_toAddress}\n to : {_toAddress}";
            }
            else
            {
                _ui.OutputText.text = $"You many NOT transfer to {_toAddress}.";
            }
            
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
        }
		
        //  Event Handlers --------------------------------
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            if (theGameModel.HasSelectedPlayerView)
            {
                //string address = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e"; 
                CustomPlayerInfo customPlayerInfo = new CustomPlayerInfo(); // based on _theGameModel.SelectedPlayerView.Value.OwnerClientId
                _fromAddress = "0xTempBlah";
                _toAddress = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e";
                
            }
            else
            {
                _toAddress = "";
            }
        }

        
        private void TransferGoldButton_OnClicked()
        {
            OnTransferGoldRequested.Invoke();
        }
        
        
        private void TransferPrizeButton_OnClicked()
        {
            OnTransferPrizeRequested.Invoke();
        }
        
        
        private void CancelButton_OnClicked()
        {
            TheGameSingleton.Instance.TheGameController.UnregisterTransferDialogView(this);
            Destroy(this.gameObject);
        }
    }
}