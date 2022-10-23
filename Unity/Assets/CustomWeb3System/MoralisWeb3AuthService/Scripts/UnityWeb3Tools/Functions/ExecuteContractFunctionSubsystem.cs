using System;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Interfaces;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using UnityEngine;
using WalletConnectSharp.Core;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Other;
using MoralisUnity.Samples.SharedCustom.Exceptions;

namespace MoralisUnity.Samples.Shared.UnityWeb3Tools.Functions
{
     public class ExecuteContractFunctionSubSystem : IInitializableAsync
     {
         private Web3 Web3Client { get; set; }
         public bool IsInitialized { get; private set; }
         
         public async UniTask InitializeAsync()
         {
             if (!IsInitialized)
             {
                 await UniTask.RunOnThreadPool(() =>
                 {
                     WalletConnectSession client = WalletConnect.Instance.Session;
                     // Create a web3 client using Wallet Connect as write client and a dummy client as read client.
                     Web3Client = new Web3(client.CreateProvider(new DummyRpcClient(Debug.LogError)));
                 });
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

         public async UniTask<string> RunAsync(
             string web3UserAddress, 
             string contractAddress,
             string abi,
             string functionName,
             object[] args,
             bool isLogging = false)
         {
             			
             // Lazy initialize here. Only when Run is called
             if (!IsInitialized)
             {
                 await InitializeAsync();
             }
             
             // Set gas estimate by default
             HexBigInteger value = new HexBigInteger(0);
             HexBigInteger gas = new HexBigInteger(0);

             string result = null;

             try
             {
                 Contract contract = Web3Client.Eth.GetContract(abi, contractAddress);
                 Function function = contract.GetFunction(functionName);
                 
                 if (isLogging)
                 {
                     StringBuilder stringBuilder = new StringBuilder();
                     stringBuilder.AppendLine($"RunAsync() Starting ... \n\n");
                     stringBuilder.AppendLine($"web3Address = {web3UserAddress}");
                     stringBuilder.AppendLine($"contractAddress = {contractAddress}");
                     stringBuilder.AppendLine($"abi = {abi}");
                     stringBuilder.AppendLine($"functionName = {functionName}");
                     stringBuilder.AppendLine($"args = {args}");
                     stringBuilder.AppendLine($"contract = {contract}");
                     stringBuilder.AppendLine($"function = {function}");
                     stringBuilder.AppendLine($"value = {value}");
                     stringBuilder.AppendLine($"gas = {gas}");
                     stringBuilder.AppendLine($"\n\n\n");
                     Debug.Log(stringBuilder);
                 }
                 
                 if (isLogging)
                 {
                     StringBuilder stringBuilder = new StringBuilder();
                     stringBuilder.AppendLine($"RunAsync() Pending");
                     Debug.Log(stringBuilder);
                 }
                 
                 if (function != null)
                 {
                     result = await function.SendTransactionAsync(web3UserAddress, gas, value, args);
                 }
             }
             catch (Exception exp)
             {
                 Debug.Log($"Call to {functionName} failed due to: {exp.Message}");
             }
             
             
             if (isLogging)
             {
                 StringBuilder stringBuilder = new StringBuilder();
                 stringBuilder.AppendLine($"RunAsync() Completed ...\n\n");
                 stringBuilder.AppendLine($"result = {result}");
                 stringBuilder.AppendLine($"\n\n\n");
                 Debug.Log(stringBuilder);
             }

             return result; 
         }
     } 
}
