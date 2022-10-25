using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene02_Authentication"/>
    /// </summary>
    public class Scene02_AuthenticationUI : MonoBehaviour
    {
        //  Properties ------------------------------------
        public PlayerView PlayerView { get { return _playerView; } }
        
        public AuthenticationButtonUI AuthenticationButtonUI { get { return _authenticationButtonUI; } }
        
        public BaseButtonUI BackButton { get { return _backButton; } }
        
        public AuthenticationKit AuthenticationKit { get { return _authenticationKit; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private AuthenticationKit _authenticationKit = null;

        [SerializeField]
        private PlayerView _playerView = null;

        [SerializeField]
        private AuthenticationButtonUI _authenticationButtonUI = null;

        [SerializeField]
        private BaseButtonUI _backButton = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}