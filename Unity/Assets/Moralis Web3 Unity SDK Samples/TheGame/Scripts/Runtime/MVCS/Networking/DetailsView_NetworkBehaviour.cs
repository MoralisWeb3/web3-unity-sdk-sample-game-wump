using System.Text;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace MoralisUnity.Samples.TheGame.MVCS.Networking
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------


        /// <summary>
        /// The core Game UI logic that **IS** networking
        /// 
        /// NOTE: This is instantiated by the <see cref="TheGameSingleton"/>
        ///
        /// Relates to the <see cref="DetailsView"/>
        /// 
        /// </summary>
        public class DetailsView_NetworkBehaviour : NetworkBehaviour
        {
            //  Events ----------------------------------------

            //  Properties ------------------------------------
            [SerializeField] 
            private DetailsView _detailsView;
            
            [SerializeField] 
            private SharedStatus_NetworkBehaviour _sharedStatusNetworkBehaviour;

            private readonly NetworkVariable<int> _playerCountNetworkVariable = new NetworkVariable<int>(0,
                NetworkVariableReadPermission.Everyone);
            private readonly NetworkVariable<int> _connectedClientsNetworkVariable = new NetworkVariable<int>(0,
                NetworkVariableReadPermission.Everyone);
            private readonly NetworkVariable<CustomPlayerInfo> _customPlayerInfo = new NetworkVariable<CustomPlayerInfo>(
                default(CustomPlayerInfo), 
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);

            private string _web3Address = "";
            
            //  Unity Methods ---------------------------------

            public override void OnNetworkSpawn()
            {
                Assert.IsNull(transform.parent, TheGameConstants.NetworkTransformParentMustBeNull); 

                TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(
                    TheGameSingleton_OnTheGameModelChanged);
                TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
                _customPlayerInfo.OnValueChanged += CustomPlayerInfo_OnValueChanged;
                
                // Trigger Early to "Blank out" the text temporarily
                PlayerCountNetworkVariable_OnValueChanged(0, 0);
                _sharedStatusNetworkBehaviour.OnSharedStatusChanged.AddListener(StatusNetworkBehaviour_OnStatusChanged);
                StatusNetworkBehaviour_OnStatusChanged();
                UpdatePlayerDetails();
                
                _detailsView.OnSharedStatusUpdateRequested.AddListener(TheGameView_OnSharedStatusUpdateRequested);
                
				// Trigger Early
                _playerCountNetworkVariable.OnValueChanged += PlayerCountNetworkVariable_OnValueChanged;
                _connectedClientsNetworkVariable.OnValueChanged += ConnectedClientsNetworkVariable_OnValueChanged;

				//Setup
                UpdatePlayerDetails();

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
            
            private void UpdatePlayerDetails()
            {
                StringBuilder _playerDetailsStringBuilder = new StringBuilder();
                _playerDetailsStringBuilder.Clear();
                _playerDetailsStringBuilder.AppendLine("<b>Player Details</b>");
                _playerDetailsStringBuilder.AppendLine($" Host: {IsHost}");
                _playerDetailsStringBuilder.AppendLine($" Client: {IsClient}");
                _playerDetailsStringBuilder.AppendLine($" Server: {IsServer}");
                _playerDetailsStringBuilder.AppendLine($" Web3Address: {_web3Address}");
                
                ulong _playerIndex = PlayerView.GetPlayerIndexByClientId(NetworkManager.Singleton.LocalClientId);
                _playerDetailsStringBuilder.AppendLine($" Player #: {_playerIndex} of " +
                    $"{_playerCountNetworkVariable.Value}/{_connectedClientsNetworkVariable.Value}");
                //
                _detailsView.PlayerName =  _playerDetailsStringBuilder.ToString();

            }
            
           
            //  Event Handlers --------------------------------
            private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
            {
                // Only the owner may SET the value
                if (!IsOwner) return;
                _customPlayerInfo.Value = theGameModel.CustomPlayerInfo.Value;
            }
            
            private void CustomPlayerInfo_OnValueChanged(CustomPlayerInfo old, CustomPlayerInfo customPlayerInfo)
            {
                if (!customPlayerInfo.IsNull())
                {
                    _web3Address = customPlayerInfo.Web3Address.Value;
                }
                UpdatePlayerDetails();
            }
            
            private void StatusNetworkBehaviour_OnStatusChanged(string status = "")
            {
                _detailsView.SharedStatus = status;

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
                UpdatePlayerDetails();
            }
            
            private void ConnectedClientsNetworkVariable_OnValueChanged(int previousvalue, int newvalue)
            {
                UpdatePlayerDetails();
            }
        }
    }
