using System.Text;
using RMC.TheGame.MVCS.View;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace RMC.TheGame.MVCS.Networking
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------


        /// <summary>
        /// The core Game UI logic that **IS** networking
        /// 
        /// NOTE: This is attached by the <see cref="PlayerView"/>
        ///
        /// Relates to the <see cref="TheGame"/>
        /// 
        /// </summary>
        public class TheGameView_NetworkBehaviour : NetworkBehaviour
        {
            //  Events ----------------------------------------

            //  Properties ------------------------------------
            [SerializeField] 
            private TheGameView _theGameView;
            
            [SerializeField] 
            private SharedStatus_NetworkBehaviour _sharedStatusNetworkBehaviour;

            private readonly NetworkVariable<int> _playerCountNetworkVariable = new NetworkVariable<int>(0,
                NetworkVariableReadPermission.Everyone);
            private readonly NetworkVariable<int> _connectedClientsNetworkVariable = new NetworkVariable<int>(0,
                NetworkVariableReadPermission.Everyone);

            //  Unity Methods ---------------------------------

            public override void OnNetworkSpawn()
            {
                Assert.IsTrue(transform.parent == null, 
                    "NetworkObjects *manually placed* in scene must be on the root.");
                
                // Trigger Early to "Blank out" the text temporarily
                PlayerCountNetworkVariable_OnValueChanged(0, 0);
                _sharedStatusNetworkBehaviour.OnSharedStatusChanged.AddListener(StatusNetworkBehaviour_OnStatusChanged);
                StatusNetworkBehaviour_OnStatusChanged();
                UpdatePlayerName();
                
                _theGameView.OnSharedStatusUpdateRequested.AddListener(TheGameView_OnSharedStatusUpdateRequested);
                
				// Trigger Early
                _playerCountNetworkVariable.OnValueChanged += PlayerCountNetworkVariable_OnValueChanged;
                _connectedClientsNetworkVariable.OnValueChanged += ConnectedClientsNetworkVariable_OnValueChanged;

				//Setup
                UpdatePlayerName();

				// Observe
                if (!IsServer) return;
                NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
                NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnected;
                NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnected;
                
            }



            public override void OnNetworkDespawn()
            {
                if (!IsServer) return;
                NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
                NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnected;
                NetworkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnected;
            }
            
            

            //  Methods ---------------------------------------
            private void TheGameView_OnSharedStatusUpdateRequested()
            {
                //An RPC is used to message **EVERY** client
                _sharedStatusNetworkBehaviour.SharedStatusUpdateRequest();
            }
            
            private void UpdatePlayerName()
            {
                StringBuilder _playerDetailsStringBuilder = new StringBuilder();
                _playerDetailsStringBuilder.Clear();
                _playerDetailsStringBuilder.AppendLine("<b>Player Details</b>");
                _playerDetailsStringBuilder.AppendLine($" Host: {IsHost}");
                _playerDetailsStringBuilder.AppendLine($" Client: {IsClient}");
                _playerDetailsStringBuilder.AppendLine($" Server: {IsServer}");
                ulong _playerIndex = PlayerView.GetPlayerIndexByClientId(NetworkManager.Singleton.LocalClientId);
                _playerDetailsStringBuilder.AppendLine($" Player #: {_playerIndex} of " +
                    $"{_playerCountNetworkVariable.Value}/{_connectedClientsNetworkVariable.Value}");
                //
                _theGameView.PlayerName =  _playerDetailsStringBuilder.ToString();

            }
            
           
            //  Event Handlers --------------------------------
            private void StatusNetworkBehaviour_OnStatusChanged(string status = "")
            {
                _theGameView.SharedStatus = status;

            }
            
            private void NetworkManager_OnServerStarted()
            {
                Debug.Log("NetworkManager_OnServerStarted");
            }
            
            private void NetworkManager_OnClientConnected(ulong obj)
            {
                if (!IsServer) return;
                _playerCountNetworkVariable.Value++;
                _connectedClientsNetworkVariable.Value = NetworkManager.Singleton.ConnectedClients.Count;
            }
            
            
            private void NetworkManager_OnClientDisconnected(ulong obj)
            {
                if (!IsServer) return;
                _playerCountNetworkVariable.Value--;
                _connectedClientsNetworkVariable.Value = NetworkManager.Singleton.ConnectedClients.Count;
            }   
            
            
            private void PlayerCountNetworkVariable_OnValueChanged(int previousvalue, int newvalue)
            {
                UpdatePlayerName();
            }
            
            private void ConnectedClientsNetworkVariable_OnValueChanged(int previousvalue, int newvalue)
            {
                UpdatePlayerName();
            }
        }
    }
