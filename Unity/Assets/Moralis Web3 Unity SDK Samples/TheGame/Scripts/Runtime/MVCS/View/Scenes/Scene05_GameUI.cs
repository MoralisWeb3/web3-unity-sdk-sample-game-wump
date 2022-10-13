using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Scenes;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.Scenes
{
    /// <summary>
    /// The UI for Core Scene Behavior of <see cref="Scene05_GameOLD"/>
    /// </summary>
    public class Scene05_GameUI : Scene_UIWithTop
    {
        //  Properties ------------------------------------
        public BaseButtonUI BackButton { get { return _backButton; } }

        public WalletConnectWrapper WalletConnectWrapper { get { return _walletConnectWrapper; } }
        
        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

        [SerializeField]
        private BaseButtonUI _backButton = null;

        [SerializeField]
        private WalletConnectWrapper _walletConnectWrapper = null;

        
        //  Unity Methods----------------------------------


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
		
    }
}