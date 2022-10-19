using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
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
            public readonly StringUnityEvent OnRpcSharedStatusChanged = new StringUnityEvent();
            
            [HideInInspector]
            public readonly TransferLogUnityEvent OnRPCTransferLogChanged = new TransferLogUnityEvent();

                
            public string SharedStatus
            {
                get
                {
                    return _sharedStatus;
                }
                private set
                {
                    _sharedStatus = value; OnRpcSharedStatusChanged.Invoke(_sharedStatus);
                }
            }
            private string _sharedStatus = "";
            
            //  Properties ------------------------------------
            
            //  Fields ----------------------------------------
                
            //  Unity Methods ---------------------------------
            public async UniTask SendMessageTransferLogAsync()
            {
                TransferLog transferLog = await TheGameSingleton.Instance.TheGameController.GetTransferLogHistoryAsync();
                
                //CONVERT: 1 OF 2
                //RPC needs serializable data. Unity supports custom, serializable classes.
                //INSTEAD: Since I already have string-to-object methods for use in web3, I'll use them here
                string transferLogString = TheGameHelper.ConvertTransferLogObjectToString(transferLog);
                
                SendMessageTransferLogHistoryChangedServerRpc(transferLogString);
            }
            
            /// <summary>
            /// **ANY** Client may call the **ONE** server... 
            /// </summary>
            [ServerRpc (RequireOwnership = false)]
            private void SendMessageTransferLogHistoryChangedServerRpc(string transferLogString, ServerRpcParams serverRpcParams = default)
            {
                SendMessageTransferLogHistoryChangedClientRpc(transferLogString);
            }
            
            /// <summary>
            /// ... And the **ONE** server then calls **EVERY** client
            /// </summary>
            [ClientRpc (Delivery = RpcDelivery.Reliable)]
            private void SendMessageTransferLogHistoryChangedClientRpc(string transferLogString)
            {
                //CONVERT: 2 OF 2
                TransferLog transferLog = TheGameHelper.ConvertTransferLogStringToObject(transferLogString);
                
                OnRPCTransferLogChanged.Invoke(transferLog);
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
