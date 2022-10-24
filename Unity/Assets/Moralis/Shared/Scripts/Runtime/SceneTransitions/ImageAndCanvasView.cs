using MoralisUnity.Samples.Shared.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace MoralisUnity.Samples.Shared.Data.Types.Storage
{
    public class ImageAndCanvasView: MonoBehaviour, IAlpha, IBlocksRaycasts
    {
        //  Properties ------------------------------------
        public Image Image { get { return _image; } }
        
        public float Alpha
        {
            get
            {
                if (!_canvasGroup)
                {
                    return 0;
                }
                return  _canvasGroup.alpha;
            }
            set
            {
                if (!_canvasGroup)
                {
                    return;
                }
                _canvasGroup.alpha = value;
            }
        }
        
        public bool BlocksRaycasts
        {
            get
            {
                return _canvasGroup != null && _canvasGroup.blocksRaycasts;
            }
            set
            {
                if (!_canvasGroup)
                {
                    return;
                }
                _canvasGroup.blocksRaycasts = value;
            }
        }
    
        //  Fields ----------------------------------------
        [SerializeField]
        private Image _image = null;
            
        [SerializeField]
        private CanvasGroup _canvasGroup = null;

        //  Methods ---------------------------------------
        
        
    }
}