using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View.UI
{
	/// <summary>
	/// Handles the top navigation
	/// </summary>
	public class CornerUI : MonoBehaviour
	{
		// Properties -------------------------------------
		public TMP_Text Text { get { return _text; }}

		// Fields -----------------------------------------
		[Header("References (Project)")]
		[SerializeField]
		private Sprite _iconSprite = null;

		[Header("References (Scene)")] 
		[SerializeField] 
		private TMP_Text _text = null;

		[SerializeField]
		private Image _iconImage = null;

		//[Header("References (Project)")] 

		//  Unity Methods----------------------------------
		protected void Start()
		{

		}

		protected void OnValidate()
		{
			if (_iconImage.sprite != _iconSprite)
            {
				_iconImage.sprite = _iconSprite;
			}
		}

		protected void Update()
		{

		}
		
		// General Methods --------------------------------

		
		// Event Handlers ---------------------------------

	}
}
