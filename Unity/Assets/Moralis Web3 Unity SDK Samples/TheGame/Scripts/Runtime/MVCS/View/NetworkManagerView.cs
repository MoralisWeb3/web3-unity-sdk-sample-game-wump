using RMC.Shared.Managers;
using Unity.Netcode;
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
        public class NetworkManagerView : MonoBehaviour
        {
            //  Events ----------------------------------------
        
            //  Properties ------------------------------------
            public NetworkManager NetworkManager { get { return _networkManager;}}
            
            //  Fields ----------------------------------------
            [SerializeField] 
            private NetworkManager _networkManager = null;

            //  Unity Methods ---------------------------------

            //  Methods ---------------------------------------

            //  Event Handlers --------------------------------
        }
    }
