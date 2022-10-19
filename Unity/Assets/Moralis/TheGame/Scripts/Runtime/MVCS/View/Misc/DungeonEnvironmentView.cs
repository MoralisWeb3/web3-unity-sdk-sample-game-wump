using RMC.Shared.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------

        /// <summary>
        /// The main 3d environment asset
        ///
        /// Also captures any click on the 'background' as a deselection for the <see cref="SelectionManager"/>
        /// </summary>
        public class DungeonEnvironmentView : MonoBehaviour,
            IPointerClickHandler, ISelectionManagerSelectable
        {
            //  Events ----------------------------------------
        
            //  Properties ------------------------------------
            public bool IsSelected { get; set; }
            
            //  Fields ----------------------------------------

            //  Unity Methods ---------------------------------
            
            //  Methods ---------------------------------------
            
            //  Event Handlers --------------------------------
            public void OnPointerClick(PointerEventData eventData)
            {
                if (SelectionManager.Instance.HasSelection())
                {
                    SelectionManager.Instance.ClearSelection();
                }
            }
        }
    }
