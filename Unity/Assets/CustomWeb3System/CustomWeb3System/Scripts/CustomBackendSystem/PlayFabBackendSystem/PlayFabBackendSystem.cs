using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Functions;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;
using MoralisUnity.Samples.SharedCustom.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using MoralisUnity.Samples.SharedCustom.Interfaces;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class PlayFabBackendSystem : CustomSingleton<PlayFabBackendSystem>, 
		IInitializable, ICustomSingletonParent, ICustomBackendSystem
	{
		// Properties -------------------------------------
		public bool IsInitialized { get; private set; }

		// Fields -----------------------------------------
		public bool IsAuthenticated { get; private set; }
		public bool HasAuthenticationError { get; private set; }
		//
		private ExecuteContractFunctionSubSystem _executeContractFunctionSubsystem = new ExecuteContractFunctionSubSystem();
		private RunContractFunctionSubsystem _runContractFunctionSubsystem = new RunContractFunctionSubsystem();


		// Initialization Methods -------------------------
		void ICustomSingletonParent.OnInstantiatedChild()
		{
			// Auto-Initialize
			Initialize();
		}

		public async void Initialize()
		{
			if (!IsInitialized)
			{
				_runContractFunctionSubsystem = new RunContractFunctionSubsystem();
				_executeContractFunctionSubsystem = new ExecuteContractFunctionSubSystem();
				
				PlayFabAuthService.OnLoginSuccess += PlayFabAuthService_OnLoginSuccess;
				PlayFabAuthService.OnPlayFabError += PlayFabAuthService_OnPlayFabError;
				PlayFabAuthService.Instance.RememberMe = true;
				IsInitialized = true;
			}
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
		
		public void RequireIsAuthenticated()
		{
			if (!IsAuthenticated)
			{
				throw new NotAuthenticatedException(this);
			}
		}
		
		// General Methods --------------------------------

		public UniTask ClearActiveSessionAsync()
		{
			PlayFabAuthService.Instance.ClearRememberMe();
			return UniTask.DelayFrame(0);
		}

		public UniTask AuthenticateAsync()
		{
			RequireIsInitialized();
			
			if (!IsAuthenticated)
			{
				PlayFabAuthService.Instance.Authenticate(Authtypes.Silent); //Play as guest. Adequate
			}

			return UniTask.WaitWhile(() => !IsAuthenticated && !HasAuthenticationError);
		}




		public async UniTask<String> ExecuteContractFunctionAsync(string contractAddress, string abi,
			string functionName, object[] args, bool isLogging = false)
		{

			RequireIsInitialized();
			RequireIsAuthenticated();

			if (!_executeContractFunctionSubsystem.IsInitialized)
			{
				await _executeContractFunctionSubsystem.InitializeAsync();
			}
			
			//
			string web3UserAddress = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
			string result = await _executeContractFunctionSubsystem.RunAsync(
				web3UserAddress, 
				contractAddress, 
				abi, 
				functionName, 
				args,
				isLogging);
			
			return result;
		}


		// Event Handlers ---------------------------------
		public async UniTask<object> RunContractFunctionAsync(
			string contractAddress,
			string functionName,
			string abi, 
			object args,
			bool isLogging = false)
		{
			
			RequireIsInitialized();
			RequireIsAuthenticated();

			if (!_runContractFunctionSubsystem.IsInitialized)
			{
				_runContractFunctionSubsystem.Initialize();
			}
			
			//TODO: Refactor so this class does not have to reference CustomWeb3System.Instance?
			int chainId = CustomWeb3System.Instance.ChainId;
			
			object result = await _runContractFunctionSubsystem.RunAsync(
				contractAddress,
				chainId,
				functionName,
				abi,
				args,
				isLogging);

			return result;
		}


		public async UniTask<List<NftOwner>> GetNFTsForContractAsync(string contractAddress, bool isLogging = false)
		{
			RequireIsInitialized();
			RequireIsAuthenticated();
			
			List<NftOwner> resultNftOwners = new List<NftOwner>();
			bool hasCompletedExecuteFunction = false;
			
			//TODO: Refactor so this class does not have to reference CustomWeb3System.Instance?
			string web3UserAddress = await CustomWeb3System.Instance.GetWeb3UserAddressAsync();
			int chainId = CustomWeb3System.Instance.ChainId;
			
			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"GetNFTsForContractAsync() Starting ...\n\n");
				Debug.Log(stringBuilder);
			}

			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
			{
				Entity = new PlayFab.CloudScriptModels.EntityKey()
				{
					Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
					Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
				},
				FunctionName = "GetNftsForContract", //This should be the name of your Azure Function that you created.
				FunctionParameter =
					new Dictionary<string, object>() //This is the data that you would want to pass into your function.
					{
						{ "walletAddress", web3UserAddress },
						{ "chainid", chainId } 
					},
				GeneratePlayStreamEvent = true //Set this to true if you would like this call to show up in PlayStream
			}, (ExecuteFunctionResult result) =>
			{
				if (result.FunctionResultTooLarge ?? false)
				{
					Debug.LogError("error 1 for : " + result.FunctionResult);
					//"This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
					// If the is a error fire the OnFailed event
					hasCompletedExecuteFunction = true;
				}

				// If the authentication succeeded the user profile is update and we get the UpdateUserDataAsync return values a response
				// If it failed it returns empty
				if (String.IsNullOrEmpty(result.FunctionResult.ToString()))
				{
					Debug.LogError("error 2 for : " + result.FunctionResult);
					hasCompletedExecuteFunction = true;
				}
				else
				{
					
					List<NftOwner> allNftOwners =
						JsonConvert.DeserializeObject<List<NftOwner>>(result.FunctionResult.ToString());

					if (allNftOwners == null)
					{
						Debug.Log("No owners for this contractAddress");
						hasCompletedExecuteFunction = true;
					}

					foreach (NftOwner nftOwner in allNftOwners)
					{
						try
						{
							// Check if its minted in our contract
							if (string.Equals(nftOwner.TokenAddress, contractAddress, StringComparison.InvariantCultureIgnoreCase))
							{
								resultNftOwners.Add(nftOwner);
							}
						}
						catch (Exception e)
						{
							Debug.LogError("Error with My NFT called: " + e.Message);
							throw;
						}
					}

					hasCompletedExecuteFunction = true;
				}
			}, (PlayFabError error) => { Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}"); });

			
			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"GetNFTsForContractAsync() Pending");
				Debug.Log(stringBuilder);
			}
			
			await UniTask.WaitWhile(() => !hasCompletedExecuteFunction);

			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"GetNFTsForContractAsync() Completed ...\n\n");
				stringBuilder.AppendLine($"result.count = {resultNftOwners.Count}");
				stringBuilder.AppendLine($"\n\n\n");
				Debug.Log(stringBuilder);
			}

			return resultNftOwners;
		}
		
		 public async UniTask<ExecuteFunctionResult> ChallengeRequestAsync(string address, int chainid)
        {
	        RequireIsInitialized();
	        RequireIsAuthenticated();
	        
            Debug.Log("ChallengeRequestAsync message: " + address);
            ExecuteFunctionResult executeFunctionResult = null;
            bool hasCompletedExecuteFunction = false;
            
            // Get message from Moralis with PlayFab Azure Functions 
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                    Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
                },
                FunctionName = "ChallengeRequest", //This should be the name of your Azure Function that you created.
                FunctionParameter =
                    new Dictionary<string, object>() //This is the data that you would want to pass into your function.
                    {
                        { "address", address },
                        { "chainid", chainid }
                    },
                GeneratePlayStreamEvent = true //Set this to true if you would like this call to show up in PlayStream
            }, async (ExecuteFunctionResult result) =>
            {
	            if (result.FunctionResultTooLarge ?? false)
	            {
		            Debug.LogError(
			            "This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
		            // If the is a error fire the OnFailed event
		            // OnFailed.Invoke();
		            hasCompletedExecuteFunction = true;
	            }
	            else
	            {
		            executeFunctionResult =  result;
		            hasCompletedExecuteFunction = true;
	            }
            }, (PlayFabError error) =>
            {
                Debug.LogError($"Oops Something went wrong: {error.GenerateErrorReport()}");
                // If the is a error fire the OnFailed event
                //OnFailed.Invoke();
                hasCompletedExecuteFunction = true;
            });
            
            await UniTask.WaitWhile(() => !hasCompletedExecuteFunction);
            return executeFunctionResult;
        }

		 public async UniTask<ExecuteFunctionResult> ChallengeVerifyAsync(string message, string signature)
        {
	        RequireIsInitialized();
	        RequireIsAuthenticated();
	        
	        Debug.Log("ChallengeVerifyAsync message: " + message);
	        ExecuteFunctionResult executeFunctionResult = null;
	        bool hasCompletedExecuteFunction = false;
	        
            // Send the message and signature to the Authenticate Azure function for validation
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                    Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
                },
                FunctionName = "ChallengeVerify", //This should be the name of your Azure Function that you created.
                FunctionParameter =
                    new Dictionary<string, object>() //This is the data that you would want to pass into your function.
                    {
                        { "message", message },
                        { "signature", signature }
                    },
                GeneratePlayStreamEvent = true //Set this to true if you would like this call to show up in PlayStream
            }, (ExecuteFunctionResult result) =>
            {
                if (result.FunctionResultTooLarge ?? false)
                {
                    Debug.LogError(
                        "This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                    // If the is a error fire the OnFailed event
                   // OnFailed.Invoke();
                   hasCompletedExecuteFunction = true;
                }

                // If the authentication succeeded the user profile is update and we get the UpdateUserDataAsync return values a response
                // If it failed it returns empty
                if (String.IsNullOrEmpty(result.FunctionResult.ToString()))
                {
	                Debug.LogError(
		                "This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
	                // If the is a error fire the OnFailed event
	                // OnFailed.Invoke();
	                hasCompletedExecuteFunction = true;
                }
                else
                {
	                //good
	                executeFunctionResult = result;
	                hasCompletedExecuteFunction = true;
                }
            }, (PlayFabError error) =>
            {
                Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}");
                // If the is a error fire the OnFailed event
                //OnFailed.Invoke();
                hasCompletedExecuteFunction = true;
            });

            await UniTask.WaitWhile(() => !hasCompletedExecuteFunction);
            return executeFunctionResult;
        }


		// Event Handlers ---------------------------------

		
		private void PlayFabAuthService_OnLoginSuccess(LoginResult loginResult)
		{
			Debug.LogWarning($"PlayFabAuthService_OnLoginSuccess () loginResult = {loginResult}");
			RequireIsInitialized();
			IsAuthenticated = true;
			RequireIsAuthenticated();
		}
		
		private void PlayFabAuthService_OnPlayFabError(PlayFabError playFabError)
		{
			RequireIsInitialized();
			Debug.LogError($"PlayFabAuthService_OnPlayFabError () playFabError = {playFabError}");
			HasAuthenticationError = true;
			IsAuthenticated = false;
		}

	}
}

