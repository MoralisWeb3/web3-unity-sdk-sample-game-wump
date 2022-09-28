using UnityEngine;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.UI
{
	/// <summary>
	/// Handles the top navigation
	/// </summary>
	public class TopUI : MonoBehaviour
	{
		// Properties -------------------------------------
		public CornerUI GoldCornerUI { get { return _goldCornerUI; }}
		public CornerUI CollectionUI { get { return _collectionUI; } }

		// Fields -----------------------------------------
		[Header("References (Scene)")] 
		[SerializeField] 
		private CornerUI _goldCornerUI = null;

		[SerializeField]
		private CornerUI _collectionUI = null;

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
