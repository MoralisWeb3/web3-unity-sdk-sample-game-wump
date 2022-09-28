using MoralisUnity.Samples.Shared.UI;
using UnityEngine;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.UI;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes
{
	/// <summary>
	/// The UI for Core Scene Behavior of <see cref="Scene01_Intro"/>
	/// </summary>
	public class Scene01_IntroUI : Scene_UIWithTop
	{
		//  Properties ------------------------------------
		public BaseButtonUI PlayGameButtonUI { get { return _playGameButtonUI; } }
		public BaseButtonUI ViewCollectionButtonUI { get { return _viewCollectionButtonUI; } }
		public AuthenticationButtonUI AuthenticationButtonUI { get { return _authenticationButtonUI; } }
		public BaseButtonUI SettingsButtonUI { get { return _settingsButtonUI; } }


        //  Fields ----------------------------------------
        [Header ("References (Scene)")]

		[SerializeField]
		private BaseButtonUI _playGameButtonUI = null;

		[SerializeField]
		private BaseButtonUI _viewCollectionButtonUI = null;

		[SerializeField]
		private AuthenticationButtonUI _authenticationButtonUI = null;

		[SerializeField]
		private BaseButtonUI _settingsButtonUI = null;

		
		//  Unity Methods----------------------------------


		//  General Methods -------------------------------

		
		//  Event Handlers --------------------------------
		
	}
}
