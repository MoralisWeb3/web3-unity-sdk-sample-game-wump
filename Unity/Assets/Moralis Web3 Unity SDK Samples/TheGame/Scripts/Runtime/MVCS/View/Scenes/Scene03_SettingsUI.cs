using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using TMPro;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene03_Settings"/>
    /// </summary>
    public class Scene03_SettingsUI : Scene_UIWithTop
    {
        
        //  Properties ------------------------------------
        public WalletConnectWrapper WalletConnectWrapper { get { return _walletConnectWrapper; } }

        public TMP_Text KeyText { get { return _keyText; } }

        public PlayerView PlayerView { get { return _playerView; } }
        public BaseButtonUI BackButton { get { return _backButton; } }
        public BaseButtonUI ResetButton { get { return _resetButton; } }
        public BaseButtonUI RandomizeNicknameButton { get { return _randomizeNicknameButton; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private WalletConnectWrapper _walletConnectWrapper = null;

        [SerializeField]
        private PlayerView _playerView = null;
        
        [SerializeField]
        private TMP_Text _keyText = null;
        
        [SerializeField]
        private BaseButtonUI _resetButton = null;
        
        [SerializeField]
        private BaseButtonUI _backButton = null;

        [SerializeField]
        private BaseButtonUI _randomizeNicknameButton = null;

        
        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}