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
            _ui.TransferGoldButton.Text.text = $"Transfer {TheGameConstants.GoldOnTransfer} Gold (ERC20 Token)";
            _ui.TransferPrizeButton.Text.text = $"Transfer {TheGameConstants.PrizesOnTransfer} Prize (ERC721 Nft)";
            
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
                _ui.OutputText.text = $"Would you like to transfer Web3 assets?\n\nFrom: {_fromAddress}\nTo: {_toAddress}";
            }
            else
            {
                _ui.OutputText.text = $"You may NOT transfer Web3 assets.\n\nFrom: {_fromAddress}\nTo: {_toAddress}";
            }
            
            _ui.CancelButton.Button.onClick.AddListener(CancelButton_OnClicked);
        }
		
        //  Event Handlers --------------------------------d
        private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
        {
            if (theGameModel.HasSelectedPlayerView)
            {
                _toAddress = theGameModel.SelectedPlayerView.Value.Web3Address;
            }
            else
            {
                _toAddress = "";
            }
            _fromAddress = theGameModel.CustomPlayerInfo.Value.Web3Address;
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