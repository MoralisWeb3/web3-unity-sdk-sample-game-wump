using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene05_Game"/>
    /// </summary>
    public class Scene05_GameUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        public BaseButtonUI BackButton { get { return _backButton; } }
        public BaseButtonUI StartAsHostButton { get { return _startAsHostButton; } }
        public BaseButtonUI JoinAsClientButton { get { return _joinAsClientButton; } }
        
        public BaseButtonUI ShutdownButton { get { return _shutdownButton; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private BaseButtonUI _startAsHostButton = null;

        [SerializeField]
        private BaseButtonUI _joinAsClientButton = null;

        [SerializeField]
        private BaseButtonUI _shutdownButton = null;

        [SerializeField]
        private BaseButtonUI _backButton = null;

        
        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}