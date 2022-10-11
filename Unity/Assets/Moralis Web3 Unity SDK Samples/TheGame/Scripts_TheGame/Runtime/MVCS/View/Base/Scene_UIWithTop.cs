using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View.UI;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
	/// <summary>
	/// Any shared functionality for all Scene-specific UIs
	/// </summary>
	public class Scene_UIWithTop : Scene_BaseUI
	{
		// Properties -------------------------------------
		public TopUI TopUI { get { return _topUI; } }

		public bool IsObservingOnTheGameModelChanged
		{
			get
			{
				return _isObservingOnTheGameModelChanged;
			}
			set
			{
				_isObservingOnTheGameModelChanged = value;
				if (_isObservingOnTheGameModelChanged)
				{
					TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
				}
			}
		}
	
		// Fields -----------------------------------------
		[Header ("References (Base)")]
		[SerializeField]
		private TopUI _topUI = null;
		
		/// <summary>
		/// Determines if the ui will auto-update.
		/// This is sometimes disabled to build suspense
		/// </summary>
		private bool _isObservingOnTheGameModelChanged = true;
		
		// Unity Methods ----------------------------------
		protected override void Start()
		{
			base.Start();
			
			// AddListener - Update View When Model Changes
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
		}
		
		// General Methods --------------------------------

		
		// Event Handlers ---------------------------------
		public void OnTheGameModelChanged(TheGameModel theGameModel)
		{
			if (!_isObservingOnTheGameModelChanged)
			{
				return;
			}
			_topUI.GoldCornerUI.Text.text = TheGameHelper.FormatGoldCornerText(theGameModel.Gold.Value);
			_topUI.PrizeCornerUI.Text.text = TheGameHelper.FormatPrizeCornerText(theGameModel.Prizes.Value.Count);
		}
	}
}
