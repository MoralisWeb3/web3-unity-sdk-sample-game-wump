using MoralisUnity.Samples.Shared.UI;
using TMPro;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.UI
{
    /// <summary>
    /// The UI for Core Scene Behavior for transferring between players
    /// </summary>
    public class TransferDialogViewUI : MonoBehaviour
    {
        //  Properties ------------------------------------
        public TMP_Text OutputText { get { return _outputText; } }
        
        public BaseButtonUI TransferGoldButton { get { return _transferGoldButton; } }

        public BaseButtonUI TransferPrizeButton { get { return _transferPrizeButton; } }

        public BaseButtonUI CancelButton { get { return _cancelButton; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]
        
        [SerializeField]
        private TMP_Text _outputText = null;

        
        [SerializeField]
        private BaseButtonUI _transferGoldButton = null;

        [SerializeField]
        private BaseButtonUI _transferPrizeButton = null;

        [SerializeField]
        private BaseButtonUI _cancelButton = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}