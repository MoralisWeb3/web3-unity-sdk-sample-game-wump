using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace MoralisUnity.Samples.TheGame.MVCS.Networking
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------

        /// <summary>
        /// This is a contrived demo of using RPCs.
        ///
        /// Probably in this specific use case IT WOULD BE BETTER TO USE <see cref="NetworkVariable{T}"/> instead.
        /// However, below is useful to see how RPCs work in this real-world example.
        /// 
        /// </summary>
        public class RpcMessageBus_NetworkBehaviour : NetworkBehaviour
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public readonly StringUnityEvent OnSharedStatusChanged = new StringUnityEvent();
            
            [HideInInspector]
            public readonly UnityEvent OnRPCTransferLogHistoryChanged = new UnityEvent();

                
            public string SharedStatus
            {
                get
                {
                    return _sharedStatus;
                }
                private set
                {
                    _sharedStatus = value; OnSharedStatusChanged.Invoke(_sharedStatus);
                }
            }
            private string _sharedStatus = "";
            
            //  Properties ------------------------------------
            
            //  Fields ----------------------------------------
                
            //  Unity Methods ---------------------------------
            public void SendMessageTransferLogHistoryChanged()
            {
                SendMessageTransferLogHistoryChangedServerRpc();
            }
            
            /// <summary>
            /// **ANY** Client may call the **ONE** server... 
            /// </summary>
            [ServerRpc (RequireOwnership = false)]
            private void SendMessageTransferLogHistoryChangedServerRpc(ServerRpcParams serverRpcParams = default)
            {
                SendMessageTransferLogHistoryChangedClientRpc();
            }
            
            /// <summary>
            /// ... And the **ONE** server then calls **EVERY** client
            /// </summary>
            [ClientRpc (Delivery = RpcDelivery.Reliable)]
            private void SendMessageTransferLogHistoryChangedClientRpc()
            {
                OnRPCTransferLogHistoryChanged.Invoke();
            }
            
            //  SendSharedStatus ---------------------------------------
            public void SendMessageSharedStatus(string statusText)
            {
                SendSharedStatusServerRpc(statusText);
            }
            
            /// <summary>
            /// **ANY** Client may call the **ONE** server... 
            /// </summary>
            [ServerRpc (RequireOwnership = false)]
            private void SendSharedStatusServerRpc(string statusText, ServerRpcParams serverRpcParams = default)
            {
                SendSharedStatusClientRpc(statusText);
            }
            
            /// <summary>
            /// ... And the **ONE** server then calls **EVERY** client
            /// </summary>
            [ClientRpc (Delivery = RpcDelivery.Reliable)]
            private void SendSharedStatusClientRpc(string sharedStatus)
            {
                SharedStatus = sharedStatus;
            }
            
            
            //  Event Handlers --------------------------------
        }
    }
