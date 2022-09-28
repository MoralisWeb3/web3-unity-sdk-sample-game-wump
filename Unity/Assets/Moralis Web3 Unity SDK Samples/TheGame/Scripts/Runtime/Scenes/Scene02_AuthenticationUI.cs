using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.Controller;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.View
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene02_Authentication"/>
    /// </summary>
    public class Scene02_AuthenticationUI : MonoBehaviour
    {
        //  Properties ------------------------------------
        public BaseButtonUI CancelButton { get { return _cancelButton; } }

        public MyAuthenticationKitWrapper MyAuthenticationKitWrapper { get { return _authenticationKit; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private MyAuthenticationKitWrapper _authenticationKit;

        [SerializeField]
        private BaseButtonUI _cancelButton = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}