using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.Controller;
using MoralisUnity.Samples.TheGame.MVCS.View.UI;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene04_DeveloperConsole"/>
    /// </summary>
    public class Scene04_DeveloperConsoleUI : Scene_UIWithTop
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