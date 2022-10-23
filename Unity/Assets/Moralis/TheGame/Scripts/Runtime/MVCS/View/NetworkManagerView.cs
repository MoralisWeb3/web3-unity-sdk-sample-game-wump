using RMC.Shared.Managers;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;

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
            public RuntimeNetStatsMonitor RuntimeNetStatsMonitor { get { return _runtimeNetStatsMonitor;}}
            
            //  Fields ----------------------------------------
            [SerializeField] 
            private NetworkManager _networkManager = null;

            [SerializeField] 
            private RuntimeNetStatsMonitor _runtimeNetStatsMonitor = null;

            //  Unity Methods ---------------------------------

            //  Methods ---------------------------------------

            //  Event Handlers --------------------------------
        }
    }
