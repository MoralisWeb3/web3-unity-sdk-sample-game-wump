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
    
        //  Fields ----------------------------------------
        [SerializeField]
        private Image _image = null;
            
        [SerializeField]
        private CanvasGroup _canvasGroup = null;

        //  Methods ---------------------------------------
        
        
    }
}