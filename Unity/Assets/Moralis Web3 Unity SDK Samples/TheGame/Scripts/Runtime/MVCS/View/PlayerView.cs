using System;
using System.Collections.Generic;
using MoralisUnity.Samples.TheGame;
using RMC.Shared.Data.Types;
using RMC.Shared.Managers;
using MoralisUnity.Samples.TheGame.MVCS.Networking;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using TMPro;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

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
        public class PlayerView : MonoBehaviour, ISelectionManagerSelectable
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public PlayerViewUnityEvent OnIsWalkingChanged = new PlayerViewUnityEvent();
            
            [HideInInspector]
            public UnityEvent OnPlayerAction = new UnityEvent();

            //  Properties ------------------------------------
            public bool IsLocalPlayer { get { return _playerView_NetworkBehaviour.IsLocalPlayer; } }
   
            public float SpeedMove { get { return _speedMove;} }
            
            public float SpeedSpin { get { return _speedSpin;} }
            
            public PlayerView_NetworkBehaviour PlayerViewNetworkBehaviour { get { return _playerView_NetworkBehaviour; } }

            public Camera Camera { get { return _camera; } }

            public TMP_Text NameText { get { return _nameText; } }

            public CharacterController CharacterController  { get { return _characterController; } }

            public Collider Collider {  get { return _collider; } }

            public bool IsSelected { set { _reticlesView.IsSelected = value; } get { return _reticlesView.IsSelected; } }

            public Observable<bool> IsWalking = new Observable<bool>();
            private const string IdleTrigger = "IdleTrigger";
            private const string WalkTrigger = "WalkTrigger";
            
            //  Fields ----------------------------------------
            [Header("References (Networking)")] 
            [SerializeField]
            private PlayerView_NetworkBehaviour _playerView_NetworkBehaviour;

            
            [Header("References (Local)")] 
            [SerializeField] 
            private TMP_Text _nameText;

            [SerializeField] 
            private NetworkAnimator _networkAnimator;

            [SerializeField] 
            private CharacterController _characterController;

            [SerializeField] 
            private Collider _collider;

            [SerializeField] 
            private ReticlesView _reticlesView;

            [Header("Configuration")]
            
            [SerializeField]
            private float _speedMove = 10;

            [SerializeField]
            private float _speedSpin = 10;
            
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

            //  Unity Methods ---------------------------------

            public void CalledByOnNetworkSpawn()
            {
                IsWalking.OnValueChanged.AddListener(IsWalking_OnValueChanged);
                IsWalking.SetDirty();

                _reticlesView.OnPointerClicked.AddListener(ReticlesView_OnPointerClicked);
                MultiplayerGameSingleton.Instance.RegisterPlayerView(this);
                
                _playerView_NetworkBehaviour.OnPlayerAction.AddListener(PlayerInputNetworkBehaviour_OnPlayerAction);
                
                // Get camera for use in billboarding the PlayerName above his head
                Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                Assert.IsTrue(cameras.Length == 1);
                _camera = cameras[0];
            }



            protected void OnDestroy()
            {
                MultiplayerGameSingleton.Instance.UnregisterPlayerView(this);
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
            //  Event Handlers --------------------------------
            private void PlayerInputNetworkBehaviour_OnPlayerAction()
            {
                //Forward the event out
                OnPlayerAction.Invoke();
            }
            private void ReticlesView_OnPointerClicked()
            {
                // Click avatar in 3d to select it. 
                // Player may not select their own avatar
                if (_playerView_NetworkBehaviour.IsOwner) return;
                
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
