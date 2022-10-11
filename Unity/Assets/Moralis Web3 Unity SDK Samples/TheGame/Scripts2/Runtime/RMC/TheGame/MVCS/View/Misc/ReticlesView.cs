using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RMC.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------

        /// <summary>
        /// The circles beneath the character
        /// </summary>
        public class ReticlesView : MonoBehaviour, IPointerClickHandler
        {
            //  Events ----------------------------------------
            [SerializeField]
            public UnityEvent OnPointerClicked = new UnityEvent();
            
            //  Properties ------------------------------------
            public bool IsSelected
            {
                set
                {
                    _isSelected = value;
                    _selectionReticleView.IsVisible = _isSelected;
                }
                get
                {
                    return _isSelected;
                }
            }
            
            
            //  Fields ----------------------------------------
            [SerializeField]
            private ReticleView _identityReticleView;

            [SerializeField]
            private ReticleView _selectionReticleView;

            private bool _isSelected = false;
            
            //  Unity Methods ---------------------------------

            //  Methods ---------------------------------------
            public void SetColor(Color color)
            {
                _identityReticleView.SetColor(color);
                _selectionReticleView.SetColor(Color.white);
                IsSelected = false;
                
            }
            
            //  Event Handlers --------------------------------

            public void OnPointerClick(PointerEventData eventData)
            {
                OnPointerClicked.Invoke();
            }
        }
    }
