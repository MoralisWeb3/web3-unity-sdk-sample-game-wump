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
                    gameObject.SetActive(value);
                }
                get
                {
                    return gameObject.activeInHierarchy;
                }
            }

            //  Fields ----------------------------------------
            [SerializeField]
            private Renderer _renderer;

            //  Unity Methods ---------------------------------
            
            //  Methods ---------------------------------------
            public void SetColor(Color color)
            {
                _renderer.material.SetColor("_Color", color);
            }

            
            //  Event Handlers --------------------------------

        }
    }
