using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene03_Settings"/>
    /// </summary>
    public class Scene03_SettingsUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        public BaseButtonUI BackButton { get { return _backButton; } }
        public BaseButtonUI ResetButton { get { return _resetButton; } }
        public BaseButtonUI RandomizeNicknameButton { get { return _randomizeNicknameButton; } }
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

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