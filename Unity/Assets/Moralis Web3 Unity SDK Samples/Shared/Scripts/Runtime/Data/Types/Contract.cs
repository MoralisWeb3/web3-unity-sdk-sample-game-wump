using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Interfaces;
using MoralisUnity.Web3Api.Models;

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

			string result = await CustomWeb3System.Instance.ExecuteContractFunction(_address, _abi, 
				functionName, args, isLogging);

			if (isLogging)
			{
				Custom.Debug.Log($"{functionName} ExecuteContractFunction() FINISH, result = {result}");
			}

			return result;
		}

		public async UniTask<string> RunContractFunctionAsync(string functionName, 
			Dictionary<string, object>  args, bool isLogging)
		{
			RequireIsInitialized();

			object[] abiObject = GetAbiObject();
			string result = await CustomWeb3System.Instance.RunContractFunction<string>(_address, functionName, 
				abiObject, args, isLogging);

			if (isLogging)
			{
				Custom.Debug.Log($"{functionName} RunContractFunction() FINISH, result = {result}");
			}

			return result;
		}


    }
	
}
