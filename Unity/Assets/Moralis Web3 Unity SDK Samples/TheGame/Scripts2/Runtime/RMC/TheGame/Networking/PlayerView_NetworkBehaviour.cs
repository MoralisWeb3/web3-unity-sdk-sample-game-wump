using System;
using RMC.TheGame.MVCS.View;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RMC.TheGame.MVCS.Networking
    {
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------


        /// <summary>
        /// The core Player logic that **IS** networking
        /// </summary>
        public class PlayerView_NetworkBehaviour : NetworkBehaviour
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public UnityEvent OnPlayerAction = new UnityEvent();

            //  Properties ------------------------------------
           
            
            //  Fields ----------------------------------------
            [Header("References (Local)")] 
            [SerializeField]
            private PlayerView _playerView;

            private Vector3 _movement;
            private ulong _playerIndex;
   
            //  Unity Methods ---------------------------------
            
            public override void OnNetworkSpawn()
            {
                //Show : 1, 2, 3, etc...
                _playerIndex = PlayerView.GetPlayerIndexByClientId(OwnerClientId);
                _playerView.NameText.text = PlayerView.GetPlayerNameByClientId(OwnerClientId);
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
        }
    }
