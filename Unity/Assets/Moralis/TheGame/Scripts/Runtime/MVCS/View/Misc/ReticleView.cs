using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------

        /// <summary>
        /// The circle beneath the character
        /// </summary>
        public class ReticleView : MonoBehaviour
        {
            //  Events ----------------------------------------
        
            //  Properties ------------------------------------
            public bool IsVisible
            {
                set
                {
                    if (value)
                    {
                        //HACK: Toggling renderer.enabled makes it lose a reference, so do this...
                        transform.localScale = _rendererLocalScaleOnAwake;
                    }
                    else
                    {
                        transform.localScale = _RendererLocalWhenInvisible;
                    }
                    
                }
                get
                {
                    return transform.localScale != _RendererLocalWhenInvisible;
                }
            }

            //  Fields ----------------------------------------
            [SerializeField]
            private Renderer _renderer;

            private static readonly Vector3 _RendererLocalWhenInvisible = Vector3.zero;
            private Vector3 _rendererLocalScaleOnAwake = new Vector3(0.001f, 0.001f, 0.001f); // small but NON zero

            //  Unity Methods ---------------------------------
            protected void Awake()
            {
                _rendererLocalScaleOnAwake = _renderer.transform.localScale;
            }

            
            //  Methods ---------------------------------------
            public void SetColor(Color color)
            {
                _renderer.material.SetColor("_Color", color);
            }

            
            //  Event Handlers --------------------------------

        }
    }
