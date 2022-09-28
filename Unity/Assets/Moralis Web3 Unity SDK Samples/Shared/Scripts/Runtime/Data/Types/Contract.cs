using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Interfaces;
using MoralisUnity.Web3Api.Models;
using Nethereum.Hex.HexTypes;
using WalletConnectSharp.Unity;

namespace MoralisUnity.Samples.Shared.Data.Types
{
	/// <summary>
	/// Wrapper class for a Web3API Eth Contract.
	/// </summary>
	public abstract class Contract: IInitializable
	{
		// Properties -------------------------------------
		public string Address { get { return _address; } }
		public string Abi { get { return _abi; } }
        public bool IsInitialized { get { return _isInitialized; } protected set { _isInitialized = value; } }
		public virtual ChainList ChainList 
		{ 
			get 
			{
				// Must override in sublcass
				throw new NotImplementedException();
			} 
		}

		// Fields -----------------------------------------
		protected string _address;
		protected string _abi;
		private bool _isInitialized = false;

		// Initialization Methods -------------------------
		public Contract()
		{
			Initialize();
		}


		public virtual void Initialize ()
		{
			if (IsInitialized)
			{
				return;
			}
			SetContractDetails();
			_isInitialized = true;
		}


		protected virtual void SetContractDetails()
		{
			// Must override in sublcass
			throw new NotImplementedException();
		}


		public void RequireIsInitialized()
		{
			if (!IsInitialized)
            {
				throw new NotInitializedException(this);
			}
		}


		// General Methods --------------------------------
		protected virtual object[] GetAbiObject ()
        {
			// Must override in sublcass
			throw new NotImplementedException();
		}

		protected async UniTask<string> ExecuteContractFunctionAsync(string functionName, object[] args, bool isLogging)
		{

			RequireIsInitialized();

			MoralisUser moralisUser = await Moralis.GetUserAsync();

			if (moralisUser == null)
			{
				throw new RequiredMoralisUserException();
			}


			if (WalletConnect.Instance == null)
			{
				throw new NullReferenceException("ExecuteContractFunction() failed. " + SharedConstants.WalletConnectNullReferenceException);
			}

			await Moralis.SetupWeb3();


			// Estimate the gas
			HexBigInteger value = new HexBigInteger(0);
			HexBigInteger gas = new HexBigInteger(0);
			HexBigInteger gasPrice = new HexBigInteger(0);

			if (isLogging)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"{functionName} ExecuteContractFunction() START, ...");
				stringBuilder.AppendLine($"");
				stringBuilder.AppendLine($"\tmoralisUser.ethAddress	= {moralisUser.ethAddress}");
				stringBuilder.AppendLine($"\taddress		= {_address}");
				stringBuilder.AppendLine($"\tabi.Length	= {_abi.Length}");
				stringBuilder.AppendLine($"\tfunctionName	= {functionName}");
				stringBuilder.AppendLine($"\targs		= {args}");
				stringBuilder.AppendLine($"\tvalue		= {value}");
				stringBuilder.AppendLine($"\tgas		= {gas}");
				stringBuilder.AppendLine($"\tgasPrice	= {gasPrice}");
				Custom.Debug.Log($"{stringBuilder.ToString()}");
				
			}
			
			// Related Documentation
			// Call Method (Read/Write) - https://docs.moralis.io/moralis-dapp/web3/blockchain-interactions-unity
			// Call Method (Read Only) - https://docs.moralis.io/moralis-dapp/web3-api/native#runcontractfunction
			string result = await Moralis.ExecuteContractFunction(_address, _abi, functionName, args, value, gas, gasPrice);

			if (isLogging)
			{
				Custom.Debug.Log($"{functionName} ExecuteContractFunction() FINISH, result = {result}");
			}

			return result;
		}

		public async UniTask<string> RunContractFunctionAsync(string functionName, Dictionary<string, object>  args, bool isLogging)
		{
			RequireIsInitialized();

			MoralisUser moralisUser = await Moralis.GetUserAsync();

			if (moralisUser == null)
			{
				throw new RequiredMoralisUserException();
			}

			object[] abi = GetAbiObject();

			// Prepare the contract request
			RunContractDto runContractDto = new RunContractDto()
			{
				Abi = abi,
				Params = args
			};

			if (isLogging)
			{
				int abiLength = 0;
				if (runContractDto.Abi != null)
                {
					abiLength = runContractDto.Abi.ToString().Length;
                }

				int paramsLength = 0;
				if (runContractDto.Params != null)
				{
					paramsLength = runContractDto.Params.ToString().Length;
				}

				//Debug.Log("runContractDto: " + runContractDto.ToJson());

				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"{functionName} RunContractFunction() START, ...");
				stringBuilder.AppendLine($"");
				stringBuilder.AppendLine($"\tmoralisUser.ethAddress	= {moralisUser.ethAddress}");
				stringBuilder.AppendLine($"\taddress		= {_address}");
				stringBuilder.AppendLine($"\tfunctionName	= {functionName}");
				stringBuilder.AppendLine($"\trunContractDto.Abi.Length	= {abiLength}");
				stringBuilder.AppendLine($"\trunContractDto.Params.Length	= {paramsLength}");
				stringBuilder.AppendLine($"\tchainList	= {ChainList}");
				Custom.Debug.Log($"{stringBuilder.ToString()}");
			}


			///////////////////////////////////////////
			// Execute: RunContractFunction	
			///////////////////////////////////////////
			MoralisClient moralisClient = Moralis.GetClient();

			string result = await Moralis.Client.Web3Api.Native.RunContractFunction<string>(_address, functionName,
				runContractDto, ChainList);

			if (isLogging)
			{
				Custom.Debug.Log($"{functionName} RunContractFunction() FINISH, result = {result}");
			}

			return result;
		}


    }
	
}
