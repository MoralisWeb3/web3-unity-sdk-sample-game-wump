using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using MoralisUnity.Samples.SharedCustom.Interfaces;
using UnityEngine;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.CloudScriptModels;

namespace MoralisUnity.Samples.Shared.UnityWeb3Tools.Functions
{
     public class RunContractFunctionSubsystem : IInitializable
     {
         public bool IsInitialized { get; private set; }
         
         public void Initialize()
         {
             if (!IsInitialized)
             {
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
         
		public async UniTask<object> RunAsync(
			string contractAddress, 
			int chainId,
			string functionName,
			string abi, 
			object args, 
			bool isLogging = false)
		{
			RequireIsInitialized();
			
			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"RunAsync() Starting ...\n\n");
				stringBuilder.AppendLine($"contractAddress = {contractAddress}");
				stringBuilder.AppendLine($"chainId = {chainId}");
				stringBuilder.AppendLine($"functionName = {functionName}");
				stringBuilder.AppendLine($"abi = {abi}");
				stringBuilder.AppendLine($"args = {args}");
				stringBuilder.AppendLine($"\n\n\n");
				Debug.Log(stringBuilder);
			}
			
			string functionParamsJson = JsonConvert.SerializeObject(args);

			object finalResult = null;
			bool hasCompletedExecuteFunction = false;

			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
			{
				Entity = new PlayFab.CloudScriptModels.EntityKey()
				{
					Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
					Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
				},
				FunctionName = "RunContractFunction", //This should be the name of your Azure Function that you created.
				FunctionParameter =
					new Dictionary<string, object>() //This is the data that you would want to pass into your function.
					{
						{ "contractAddress", contractAddress },
						{ "functionAbi", abi },
						{ "functionName", functionName },
						{ "functionParams", functionParamsJson },
						{
							"chainid", chainId
						} // We are supposing the contract is deployed in the same chain we're using for authenticating. Mumbai in this case.
					},
				GeneratePlayStreamEvent = true //Set this to true if you would like this call to show up in PlayStream
			}, (ExecuteFunctionResult result) =>
			{
				if (result.FunctionResultTooLarge ?? false)
				{
					Debug.Log(
						"This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
					// If the is a error fire the OnFailed event
					hasCompletedExecuteFunction = true;
				}

				// If the authentication succeeded the user profile is update and we get the UpdateUserDataAsync return values a response
				// If it failed it returns empty
				if (String.IsNullOrEmpty(result.FunctionResult.ToString()))
				{
					Debug.LogError(result.FunctionResult);
				}
				hasCompletedExecuteFunction = true;
				finalResult = result.FunctionResult;

			}, (PlayFabError error) =>
			{
				//Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}");
				Debug.LogError(error);
				hasCompletedExecuteFunction = true;
			});

			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"RunAsync() Pending");
				Debug.Log(stringBuilder);
			}
			
			await UniTask.WaitWhile(() => !hasCompletedExecuteFunction);

			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"RunAsync() Completed ...\n\n");
				stringBuilder.AppendLine($"result = {finalResult}");
				stringBuilder.AppendLine($"result.type = {finalResult.GetType().Name}");
				stringBuilder.AppendLine($"\n\n\n");
				Debug.Log(stringBuilder);
			}

			return finalResult;
		}

     } 
}
