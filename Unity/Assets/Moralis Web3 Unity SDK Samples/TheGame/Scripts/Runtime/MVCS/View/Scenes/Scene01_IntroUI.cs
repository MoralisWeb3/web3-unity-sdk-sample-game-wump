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
        public BaseButtonUI PlayGameButton { get { return _playGameButton; } }
        public BaseButtonUI SettingsButton { get { return _settingsButton; } }
        public AuthenticationButtonUI AuthenticationButtonUI { get { return _authenticationButtonUI; } }


        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private BaseButtonUI _playGameButton = null;

        [SerializeField]
        private BaseButtonUI _settingsButton = null;

        
        [SerializeField]
        private AuthenticationButtonUI _authenticationButtonUI = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}