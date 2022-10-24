using System;
using Cysharp.Threading.Tasks;
using PlayFab.CloudScriptModels;
using UnityEngine;
using WalletConnectSharp.Unity;
using Debug = UnityEngine.Debug;

namespace MoralisUnity.Samples.Shared
{
    public class PlayFabAuthenticationKit : MonoBehaviour
    {
        //  Events ----------------------------------------
        [SerializeField]
        private AuthenticationKit _authenticationKit = null;

        public async void Start()
        {
            _authenticationKit.gameObject.SetActive(false);
            _authenticationKit.OnStateChanged.AddListener(StateObservable_OnValueChanged);

            // Wait for BACKEND to be authed
            await CustomWeb3System.Instance.AuthenticateAsync();
            await UniTask.WaitWhile(() => !CustomWeb3System.Instance.CustomBackendSystem.IsAuthenticated);
            
            // Wait for instance. NEEDED?
            await UniTask.WaitWhile(() => WalletConnect.Instance == null);
            //_authenticationKit.gameObject.SetActive(true);
        }

        public async void StateObservable_OnValueChanged(AuthenticationKitState authenticationKitState)
        {
            switch (authenticationKitState)
            {
                case AuthenticationKitState.WalletConnected:

                    //todo: REMOVE CHECK
                    if (!await CustomWeb3System.Instance.HasWeb3UserAddressAsync())
                    {
                        Debug.LogError("Failed");
                    }
                    else
                    {
                        string web3UserAddress = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
                        int chainId = CustomWeb3System.Instance.ChainId;
                    
                       await ChallengeRequestAsync(web3UserAddress, chainId);
                       
                    }
                    break;
            }
        }

        private async UniTask ChallengeRequestAsync(string web3UserAddress, int chainId)
        {
            ExecuteFunctionResult executeFunctionResult = await CustomWeb3System.Instance.ChallengeRequestAsync(web3UserAddress, chainId);
            
            if (executeFunctionResult.FunctionResultTooLarge ?? false)
            {
                Debug.Log( "This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                OnChallengeRequestFailed();
                return;
            }

            // Check if we got a message
            string message = executeFunctionResult.FunctionResult.ToString();
            if (!String.IsNullOrEmpty(message))
            {
                _authenticationKit.State = AuthenticationKitState.WalletSigning;

                string signature = await CustomWeb3System.Instance.EthPersonalSignAsync(web3UserAddress, message);

                if (!string.IsNullOrEmpty(signature))
                {
                    // Send the message and signature to the Authenticate Azure function for validation
                    await ChallengeVerifyAsync(message, signature);
                }
                else
                {
                    // If there is no signature fire the OnFailed event
                    OnChallengeRequestFailed();
                }
            }
            else
            {
                // If the is no message fire the OnFailed event
                OnChallengeRequestFailed();
            }
        }

        private async UniTask ChallengeVerifyAsync(string message, string signature)
        {
            ExecuteFunctionResult executeFunctionResult = await CustomWeb3System.Instance.ChallengeVerifyAsync(message, signature);

            if (executeFunctionResult.Error == null)
            {
                OnChallengeVerifyCompleted();
            }
            else
            {
                Debug.LogError("ok so this was an error");
                OnChallengeVerifyFailed();
            }
        }

        
        private void OnChallengeVerifyCompleted()
        {
            _authenticationKit.State = AuthenticationKitState.WalletSigned;
            _authenticationKit.State = AuthenticationKitState.MoralisLoggingIn;
            _authenticationKit.State = AuthenticationKitState.MoralisLoggedIn;
        }

        
        private void OnChallengeVerifyFailed()
        {
            _authenticationKit.Disconnect();
        }
        
        private void OnChallengeRequestFailed()
        {
            _authenticationKit.Disconnect();
        }
    }
}