using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RMC.Shared.Data.Types;
using RMC.Shared.Managers;
using MoralisUnity.Samples.TheGame.MVCS.Networking;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using TMPro;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Assertions;

namespace MoralisUnity.Samples.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------


        /// <summary>
        /// The core Player logic that is NOT networking
        ///
        /// Relates to <see cref="PlayerView_NetworkBehaviour"/>
        /// 
        /// </summary>
        public class PlayerView : MonoBehaviour, ISelectionManagerSelectable, IRegisterableView 
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public readonly PlayerViewUnityEvent OnIsWalkingChanged = new PlayerViewUnityEvent();
            
            [HideInInspector]
            public readonly PlayerViewUnityEvent OnPlayerAction = new PlayerViewUnityEvent();

            [HideInInspector]
            public readonly PlayerViewUnityEvent OnRPCSharedStatusChanged = new PlayerViewUnityEvent();
            
            [HideInInspector]
            public readonly TransferLogUnityEvent OnRPCTransferLogChanged = new TransferLogUnityEvent();

            
            //  Properties ------------------------------------
            public bool IsLocalPlayer { get { return _playerView_NetworkBehaviour.IsLocalPlayer; } }
            
            public ulong OwnerClientId { get { return _playerView_NetworkBehaviour.OwnerClientId; } }
            
            public string SharedStatus { get { return _rpcMessageBusNetworkBehaviour.SharedStatus; } }
   
            public string Nickname { get { return _playerView_NetworkBehaviour.Nickname;} }
            public string Web3Address { get { return _playerView_NetworkBehaviour.Web3Address;} }
            
            public float SpeedMove { get { return _speedMove;} }
            
            public float SpeedSpin { get { return _speedSpin;} }
            
            public Camera Camera { get { return _camera; } }

            public TMP_Text PlayerNameText { get { return _playerNameText; } }
            
            public string PlayerName { get { return _playerView_NetworkBehaviour.PlayerName; } }
            public ulong PlayerIndex { get { return _playerView_NetworkBehaviour.PlayerIndex; } }
            
            public CharacterController CharacterController  { get { return _characterController; } }

            public bool IsSelected { set { _reticlesView.IsSelected = value; } get { return _reticlesView.IsSelected; } }

            public Observable<bool> IsWalking = new Observable<bool>();
            private const string IdleTrigger = "IdleTrigger";
            private const string WalkTrigger = "WalkTrigger";
            
            //  Fields ----------------------------------------
            [Header("Configuration")]
            
            [SerializeField]
            private float _speedMove = 10;

            [SerializeField]
            private float _speedSpin = 10;
            
            /// <summary>
            /// Set to false in main menu where this avatar is used just for cosmetics
            ///
            /// TRUE
            /// * All systems within are working
            ///
            /// FALSE
            /// * Only idle animation and SetPlayerText are working
            /// 
            /// </summary>
            [SerializeField]
            private bool _isInGameScene = true;

            [Header("References (Networking)")] 
            [SerializeField]
            private PlayerView_NetworkBehaviour _playerView_NetworkBehaviour;

            [SerializeField]
            private RpcMessageBus_NetworkBehaviour _rpcMessageBusNetworkBehaviour;

            [Header("References (Local)")] 
            [SerializeField] 
            private TMP_Text _playerNameText;

            [SerializeField] 
            private NetworkAnimator _networkAnimator;

            [SerializeField] 
            private CharacterController _characterController;

            [SerializeField] 
            private ClientNetworkTransform _clientNetworkTransform;
            
            [SerializeField] 
            private ReticlesView _reticlesView;

            private List<Color> _networkPlayerColors = new List<Color>
            {
                // 
                Color.blue, // <- host
                Color.yellow,
                //
                Color.green,
                Color.cyan,
                Color.red,
                Color.magenta,

            };

            private Camera _camera;
            private bool _isInitialized = false;

            //  Initialization Methods ---------------------------------

            public void CalledByOnNetworkSpawn()
            {
      
                Initialize();
            }
            
            public void Initialize()
            {
                if (!_isInGameScene)
                {
                    return;
                }
                if (_isInitialized)
                {
                    return;
                }
                IsWalking.OnValueChanged.AddListener(IsWalking_OnValueChanged);
                IsWalking.SetDirty();

                _reticlesView.OnPointerClicked.AddListener(ReticlesView_OnPointerClicked);
                TheGameSingleton.Instance.TheGameController.RegisterView(this);
                
                _playerView_NetworkBehaviour.OnPlayerAction.AddListener(PlayerInputNetworkBehaviour_OnPlayerAction);
                _rpcMessageBusNetworkBehaviour.OnRpcSharedStatusChanged.AddListener(SharedStatus_NetworkBehaviour_OnRPCSharedStatusChanged);
                _rpcMessageBusNetworkBehaviour.OnRPCTransferLogChanged.AddListener(SharedStatus_NetworkBehaviour_OnRPCTransferLogChanged);

                _isInitialized = true;
            }
            
            
            //  Unity Methods ---------------------------------
            protected void Awake()
            {
                if (!_isInGameScene)
                {
                    _playerView_NetworkBehaviour.enabled = false;
                    _clientNetworkTransform.enabled = false;
                    _characterController.enabled = false;
                    _rpcMessageBusNetworkBehaviour.enabled = false;
                    _networkAnimator.enabled = false;
                    _reticlesView.gameObject.SetActive(false);

                }
            }

            
            protected void Start()
            {
                // Get camera for use in billboarding the PlayerName above his head
                Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                Assert.IsTrue(cameras.Length == 1);
                _camera = cameras[0];
            }
            
            
            protected void OnDestroy()
            {
                if (!TheGameSingleton.IsShuttingDown)
                {
                    //Debug.Log("NEVER BE IN HERE - unless play mode has STOPPED");
                    TheGameSingleton.Instance.TheGameController.UnregisterView(this);
                }
            }


            //  Methods ---------------------------------------
            private void IsWalking_OnValueChanged(bool isWalkingOld, bool isWalkingNew)
            {
                if (isWalkingNew)
                {
                    _networkAnimator.SetTrigger(WalkTrigger);
                    _networkAnimator.ResetTrigger(IdleTrigger);
                }
                else
                {
                    _networkAnimator.SetTrigger(IdleTrigger);
                    _networkAnimator.ResetTrigger(WalkTrigger);
                }
                OnIsWalkingChanged.Invoke(this);
            }


            public void SetColorsByIndex(ulong playerIndex)
            {
                int nextPlayerIndex = Math.Min((int)playerIndex, _networkPlayerColors.Count - 1);

                // Use NEXT body and RANDOM eyes
                int nextPlayerColorIndex = nextPlayerIndex % _networkPlayerColors.Count;
                Color bodyColor = _networkPlayerColors[nextPlayerColorIndex];
                _reticlesView.SetColor(bodyColor);
                _reticlesView.IsSelected = false;
            }

            
            public static string GetPlayerNameByClientId(ulong clientId)
            {
                return $"P{GetPlayerIndexByClientId(clientId)}"; 
            }


            
            public static ulong GetPlayerIndexByClientId(ulong clientId)
            {
                return clientId + 1; //Display as; 1, 2, 3, etc...
            }
            
            
            /// <summary>
            /// Send RPC message to all clients
            /// </summary>
            public async UniTask SendMessageTransferLogAsync()
            {
                await _rpcMessageBusNetworkBehaviour.SendMessageTransferLogAsync();
            }
            
            /// <summary>
            /// Send RPC message to all clients
            /// </summary>
            public void SendMessageSharedStatus()
            {
                string statusText = $"Hi, from {PlayerName}";
                _rpcMessageBusNetworkBehaviour.SendMessageSharedStatus(statusText);
            }
            
            
            //  Event Handlers --------------------------------
            private void PlayerInputNetworkBehaviour_OnPlayerAction()
            {
                SendMessageSharedStatus();
                
                //Event Forwarding To External Scope
                OnPlayerAction.Invoke(this);
            }

            
            private void SharedStatus_NetworkBehaviour_OnRPCSharedStatusChanged(string status)
            {
                //Event Forwarding To External Scope
                OnRPCSharedStatusChanged.Invoke(this);
            }
            
            private void SharedStatus_NetworkBehaviour_OnRPCTransferLogChanged(TransferLog transferLog)
            {
                //Event Forwarding To External Scope
                OnRPCTransferLogChanged.Invoke(transferLog);
            }
            
            private void ReticlesView_OnPointerClicked()
            {
                // Click avatar in 3d to select it. 
                // Player may not select their own avatar
                if (!_playerView_NetworkBehaviour.IsOwner)
                {
                    if (!SelectionManager.Instance.HasSelection())
                    {
                        SelectionManager.Instance.Selection = this;
                    }
                    else
                    {
                        SelectionManager.Instance.Selection = null;
                    }
                }
            }


        }
    }
