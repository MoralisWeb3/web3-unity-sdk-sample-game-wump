using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.Controller;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.View
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene03_Game"/>
    /// </summary>
    public class Scene03_GameUI : MonoBehaviour
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