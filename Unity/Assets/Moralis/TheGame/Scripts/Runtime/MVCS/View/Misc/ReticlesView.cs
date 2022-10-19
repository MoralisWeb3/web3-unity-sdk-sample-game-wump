using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------

        /// <summary>
        /// The circles beneath the character
        /// </summary>
        public class ReticlesView : MonoBehaviour, IPointerClickHandler
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public UnityEvent OnPointerClicked = new UnityEvent();
            
            //  Properties ------------------------------------
            public bool IsSelected
            {
                set
                {
                    _isSelected = value;
                    try
                    {
                        _selectionReticleView.IsVisible = _isSelected;
                    }
                    catch (Exception e)
                    {
                        //FIx this
                       Debug.LogWarning(e.Message);
                    }
                    
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
