using MoralisUnity.Samples.Shared.UI;
using UnityEngine;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes
{
	/// <summary>
	/// The UI for Core Scene Behavior of <see cref="Scene03_Settings"/>
	/// </summary>
	public class Scene03_SettingsUI : Scene_UIWithTop
	{
		//  Properties ------------------------------------
		public BaseButtonUI ResetAllDataButtonUI { get { return _resetAllDataButtonUI; } }
		public BaseButtonUI BackButtonUI { get { return _backButtonUI; } }


		//  Fields ----------------------------------------
		[Header("References (Scene)")]

		[SerializeField]
		private BaseButtonUI _resetAllDataButtonUI = null;

		[SerializeField]
		private BaseButtonUI _backButtonUI = null;

		
		//  Unity Methods----------------------------------


		//  General Methods -------------------------------

		
		//  Event Handlers --------------------------------


	}
}
