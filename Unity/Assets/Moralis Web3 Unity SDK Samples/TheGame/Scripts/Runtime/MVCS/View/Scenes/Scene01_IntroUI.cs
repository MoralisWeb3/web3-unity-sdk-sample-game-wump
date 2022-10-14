using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene01_Intro"/>
    /// </summary>
    public class Scene01_IntroUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        public PlayerView PlayerView { get { return _playerView; } }
        public WalletConnectWrapper WalletConnectWrapper { get { return _walletConnectWrapper; } }
        public AuthenticationButtonUI AuthenticationButtonUI { get { return _authenticationButtonUI; } }
        public BaseButtonUI Registerbutton { get { return _registerButton; } }
        public BaseButtonUI PlayGameButton { get { return _playGameButton; } }
        public BaseButtonUI SettingsButton { get { return _settingsButton; } }
  

        //  Fields ----------------------------------------
        [Header ("References (Scene)")]
        
        [SerializeField]
        private PlayerView _playerView = null;

        [SerializeField]
        private WalletConnectWrapper _walletConnectWrapper = null;
        
        [SerializeField]
        private AuthenticationButtonUI _authenticationButtonUI = null;

        [SerializeField]
        private BaseButtonUI _registerButton = null;

        [SerializeField]
        private BaseButtonUI _playGameButton = null;

        [SerializeField]
        private BaseButtonUI _settingsButton = null;
   
        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}