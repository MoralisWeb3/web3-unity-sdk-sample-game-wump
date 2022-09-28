using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoralisUnity.Samples.Shared.UI
{
    /// <summary>
    /// The UI for shared use.
    /// </summary>
    public class BaseButtonUI : MonoBehaviour
    {
      
        //  Properties  ---------------------------------------
        public TMP_Text Text { get { return _text;}}
        public Button Button { get { return _button;}}

        public bool IsVisible
        {
            get
            {
                return _canvasGroup.GetIsVisible();
            }
            set
            {
                _canvasGroup.SetIsVisible(value);
            }
        }
      
        public bool IsInteractable
        {
            get
            {
                return _canvasGroup.interactable;
            }
            set
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = value;
                }
            }
        }

      
        //  Fields  ---------------------------------------
        [SerializeField] 
        private Button _button = null;
      
        [SerializeField] 
        private CanvasGroup _canvasGroup = null;
      
        [SerializeField] 
        private TMP_Text _text = null;
      
        
        //  Unity Methods  --------------------------------
    }
}