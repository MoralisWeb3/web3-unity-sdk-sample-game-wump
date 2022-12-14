using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using TMPro;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene04_DeveloperConsole"/>
    /// </summary>
    public class Scene04_DeveloperConsoleUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        
        //  Fields ----------------------------------------
 

        public TMP_Text OutputText { get { return _outputText; } }
        public BaseButtonUI IsAuthenticatedButton { get { return _isAuthenticatedButton; } }
        public BaseButtonUI IsRegisteredButton { get { return _isRegisteredButton; } }
        public BaseButtonUI RegisterButton { get { return _registerButton; } }
        public BaseButtonUI UnregisterButton { get { return _unregisterButton; } }
        public BaseButtonUI TransferGoldButton { get { return _transferGoldButton; } }
        public BaseButtonUI TransferPrizeButton { get { return _transferPrizeButton; } }
        public BaseButtonUI SafeReregisterButton { get { return _safeReregisterButton; } }
        public BaseButtonUI GetPrizesButton { get { return _getPrizesButton; } }
        public BaseButtonUI GetTransferLogHistoryButton { get { return _getTransferLogHistoryButton; } }
        
        public BaseButtonUI BackButton { get { return _backButton; } }

        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private TMP_Text _outputText = null;
        
        [SerializeField]
        private BaseButtonUI _isAuthenticatedButton = null;

        [SerializeField]
        private BaseButtonUI _isRegisteredButton = null;

        [SerializeField]
        private BaseButtonUI _registerButton = null;

        [SerializeField]
        private BaseButtonUI _unregisterButton = null;

        [SerializeField]
        private BaseButtonUI _getPrizesButton = null;

        [SerializeField]
        private BaseButtonUI _transferGoldButton = null;

        [SerializeField]
        private BaseButtonUI _getTransferLogHistoryButton = null;

        
        [SerializeField]
        private BaseButtonUI _transferPrizeButton = null;

        [SerializeField]
        private BaseButtonUI _safeReregisterButton = null;
        
        [SerializeField]
        private BaseButtonUI _backButton = null;

        
        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}