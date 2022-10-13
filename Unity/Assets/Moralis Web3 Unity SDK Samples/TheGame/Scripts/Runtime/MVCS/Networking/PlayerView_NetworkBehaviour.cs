using System;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace MoralisUnity.Samples.TheGame.MVCS.Networking
    {
        //  Namespace Properties ------------------------------

        //  Class Attributes ----------------------------------

        /// <summary>
        /// The core Player logic that **IS** networking
        ///
        /// Relates to <see cref="PlayerView"/>
        /// 
        /// </summary>
        public class PlayerView_NetworkBehaviour : NetworkBehaviour
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public UnityEvent OnPlayerAction = new UnityEvent();

            //  Properties ------------------------------------
            public string PlayerName { get { return _playerName; } }
            public ulong PlayerIndex { get { return _playerIndex; } }
            
            public string Nickname { get { return _nicknameStringNetworkVariable.Value.ToString(); } }
            public string Web3Address { get { return _web3AddressStringNetworkVariable.Value.ToString(); } }
            
            //  Fields ----------------------------------------
            [Header("References (Local)")] 
            [SerializeField]
            private PlayerView _playerView;

            private readonly NetworkVariable<FixedString128Bytes> _nicknameStringNetworkVariable = 
                new NetworkVariable<FixedString128Bytes> (
                    default(FixedString128Bytes), 
                    NetworkVariableReadPermission.Everyone, 
                    NetworkVariableWritePermission.Owner);
            private readonly NetworkVariable<FixedString128Bytes> _web3AddressStringNetworkVariable = 
                new NetworkVariable<FixedString128Bytes> (
                    default(FixedString128Bytes), 
                    NetworkVariableReadPermission.Everyone, 
                    NetworkVariableWritePermission.Owner);
            
            private Vector3 _movement;
            private ulong _playerIndex;
            private string _playerName = "";
 

            //  Unity Methods ---------------------------------
            
            public override void OnNetworkSpawn()
            {
                Assert.IsNull(transform.parent, TheGameConstants.NetworkTransformParentMustBeNull); 
                
                _nicknameStringNetworkVariable.OnValueChanged += NicknameStringNetworkVariable_OnValueChanged;
                _web3AddressStringNetworkVariable.OnValueChanged += Web3AddressStringNetworkVariable_OnValueChanged;

                TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(
                    TheGameSingleton_OnTheGameModelChanged);
                TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
                SetPlayerViewNameText();
        
                //Show : 1, 2, 3, etc...
                _playerIndex = PlayerView.GetPlayerIndexByClientId(OwnerClientId);
                _playerName = PlayerView.GetPlayerNameByClientId(OwnerClientId);
                
                _playerView.SetColorsByIndex(_playerIndex);
                
                 if (_playerIndex == 0)
                 {
                     throw new Exception("This value is not allowed");
                 }
                 
                 // Put FIRST player in center
                 else if (_playerIndex == 1)
                 {
                     ChangePlayerPosition(new Vector3(
                         0,
                         0,
                         1));
                 }
                 // Onscreen - FIRST Player NEAR center
                 else if (_playerIndex == 2)
                 {
                     ChangePlayerPosition(new Vector3(
                         0,
                         0,
                         -3));
                 }
                 // Line up the others so that MANY can join
                 else
                 {
                     _playerView.transform.position = new Vector3(
                         -4 + (int)_playerIndex,
                         0,
                         -5);
                     
                     ChangePlayerPosition(new Vector3(
                         -4 + (int)_playerIndex,
                         0,
                         -5));
                 }
                 
                 // Don't await 
                 SetPlayerViewNameText();
                
                //Set each player facing camera
                _playerView.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                ChangePlayerPosition(new Vector3());
                
                //
                _playerView.CalledByOnNetworkSpawn();

            }




            protected void Update()
            {
                if (!IsOwner) return;

                _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                //TODO: Remove this feature?
                // Did player 1 click 1
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    OnPlayerAction.Invoke();
                }
            }

            
            protected void FixedUpdate()
            {
                if (IsOwner)
                {
                    ChangePlayerPosition(_movement * (Time.deltaTime * _playerView.SpeedMove));
                
                    // If moving "enough" then SPIN towards direction of travel
                    if (_movement.magnitude > 0)
                    {
                        ChangePlayerRotation(Quaternion.Slerp(
                            _playerView.CharacterController.transform.rotation,
                            Quaternion.LookRotation(_movement),
                            Time.deltaTime * _playerView.SpeedSpin));

                        _playerView.IsWalking.Value = true;
      
                    }
                    else
                    {
                        _playerView.IsWalking.Value = false;
                    }

                }
                
                
                //HACK: OWNER OR NOT OWNER - Force the y to the ground. Fixes mysterious bug
                _playerView.transform.position = new Vector3(
                    _playerView.transform.position.x,
                    0,
                    _playerView.transform.position.z);
            }

            
            //Orient the camera after all movement is completed this frame to avoid jittering
            protected void LateUpdate()
            {
                _playerView.NameText.transform.LookAt(
                    _playerView.NameText.transform.position - _playerView.Camera.transform.position, 
                    Vector3.up);
            }
            
            private void SetPlayerViewNameText()
            {
                // NetworkVariable: EVERY client may GET the value
                string line1 = $"<size=5>{_playerName} {_nicknameStringNetworkVariable.Value}</size>";
                
                string line2 = "";
                if (!string.IsNullOrEmpty(_web3AddressStringNetworkVariable.Value.ToString()))
                {
                    line2 = $"<size=4>({_web3AddressStringNetworkVariable.Value})</size>";
                }
                
                _playerView.NameText.text = $"{line1}\n" +
                                            $"{line2}";
            }
            
            //  Methods ---------------------------------------
      
            
            private void ChangePlayerRotation(Quaternion quaternion)
            {
                _playerView.CharacterController.transform.rotation = quaternion;
            }

            
            private void ChangePlayerPosition(Vector3 deltaVector3)
            {
                _playerView.CharacterController.Move(deltaVector3);
            }

            //  Event Handlers --------------------------------
            
            private void TheGameSingleton_OnTheGameModelChanged(TheGameModel theGameModel)
            {
                // NetworkVariable: Only the owner may SET the value
                if (!IsOwner) return;
                _nicknameStringNetworkVariable.Value = theGameModel.CustomPlayerInfo.Value.Nickname;
                _web3AddressStringNetworkVariable.Value = theGameModel.CustomPlayerInfo.Value.Web3Address;
            }
            
            
            private void NicknameStringNetworkVariable_OnValueChanged(FixedString128Bytes oldValue,
                FixedString128Bytes newValue)
            {
                SetPlayerViewNameText();
            }
            

            private void Web3AddressStringNetworkVariable_OnValueChanged(FixedString128Bytes oldValue, 
                FixedString128Bytes newValue)
            {
                SetPlayerViewNameText();
            }
        }

    }
