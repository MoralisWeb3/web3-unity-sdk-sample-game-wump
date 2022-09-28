using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.Controller;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.View
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene01_Intro"/>
    /// </summary>
    public class Scene01_IntroUI : MonoBehaviour
    {
        //  Properties ------------------------------------
        public BaseButtonUI PlayGameButtonUI { get { return _playGameButtonUI; } }
        public AuthenticationButtonUI AuthenticationButtonUI { get { return _authenticationButtonUI; } }


        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private BaseButtonUI _playGameButtonUI = null;

        [SerializeField]
        private AuthenticationButtonUI _authenticationButtonUI = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}