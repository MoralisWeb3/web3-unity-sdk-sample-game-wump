using MoralisUnity.Samples.Shared.UI;
using UnityEngine;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller;
using TMPro;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.Scenes
{
	/// <summary>
	/// The UI for Core Scene Behavior of <see cref="Scene05_ViewCollection"/>
	/// </summary>
	public class Scene05_ViewCollectionUI : Scene_UIWithTop
	{
		//  Properties ------------------------------------
		public TMP_Text Text { get { return _text; } }
		public BaseButtonUI BackButtonUI { get { return _backButtonUI; } }


		//  Fields ----------------------------------------
		[Header("References (Scene)")]

		[SerializeField]
		private TMP_Text _text = null;

		[SerializeField]
		private BaseButtonUI _backButtonUI = null;

		
		//  Unity Methods----------------------------------


		//  General Methods -------------------------------

		
		//  Event Handlers --------------------------------
		
	}
}
