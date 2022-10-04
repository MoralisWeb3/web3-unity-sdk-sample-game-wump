using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.Controller;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene05_Game"/>
    /// </summary>
    public class Scene05_GameUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        public BaseButtonUI BackButton { get { return _backButton; } }


        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private BaseButtonUI _backButton = null;

        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}