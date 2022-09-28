using MoralisUnity.Samples.Shared.Interfaces;
using TMPro;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.UI
{
	/// <summary>
	/// UI element for a "Loading..." type message
	/// </summary>
	public class BaseScreenMessageUI : MonoBehaviour, IIsVisible, IAlpha
	{
		// Properties -------------------------------------
		public bool IsVisible
		{
			get
			{
				return Mathf.Approximately(_canvasGroup.alpha, 1);
			}
			set
			{
				if (value)
				{
					_canvasGroup.alpha = 1;
				}
				else
				{
					_canvasGroup.alpha = 0;
				}
			}
		}
		
		public float Alpha
		{
			get
			{
				return _canvasGroup.alpha;
			}
			set
			{
				_canvasGroup.alpha = value;
			}
		}
		
		public bool BlocksRaycasts
		{
			get
			{
				return _canvasGroup.blocksRaycasts;
			}
			set
			{
				_canvasGroup.blocksRaycasts = value;
			}
		}
		
		public TMP_Text MessageText { get { return _messageText;}}

		public GameObject Panel { get { return _panel; }}

		// Fields -----------------------------------------
		[SerializeField]
		private TMP_Text _messageText = null;
		
		[SerializeField]
		private CanvasGroup _canvasGroup = null;
		
		[SerializeField] 
		private GameObject _panel = null;

		
		// Unity Methods ----------------------------------
		
		
		// General Methods --------------------------------
		
		
		// Event Handlers ---------------------------------
		
	}
}
