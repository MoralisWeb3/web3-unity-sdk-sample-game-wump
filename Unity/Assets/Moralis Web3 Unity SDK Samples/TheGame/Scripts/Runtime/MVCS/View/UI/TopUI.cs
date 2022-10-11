using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View.UI
{
	/// <summary>
	/// Handles the top navigation
	/// </summary>
	public class TopUI : MonoBehaviour
	{
		// Properties -------------------------------------
		public CornerUI GoldCornerUI { get { return _goldCornerUI; }}
		public CornerUI PrizeCornerUI { get { return _prizeCornerUI; } }

		// Fields -----------------------------------------
		[Header("References (Scene)")] 
		[SerializeField] 
		private CornerUI _goldCornerUI = null;

		[SerializeField]
		private CornerUI _prizeCornerUI = null;

		//[Header("References (Project)")] 

		//  Unity Methods----------------------------------
		protected void Start()
		{

		}
		
		protected void Update()
		{

		}
		
		// General Methods --------------------------------

		
		// Event Handlers ---------------------------------

	}
}
