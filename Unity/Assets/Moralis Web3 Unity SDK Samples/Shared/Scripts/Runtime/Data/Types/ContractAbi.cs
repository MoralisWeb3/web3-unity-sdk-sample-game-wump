using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Interfaces;
using MoralisUnity.Web3Api.Models;
using Nethereum.Hex.HexTypes;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace MoralisUnity.Samples.Shared.Data.Types
{
	/// <summary>
	/// Wrapper class for a Web3API Eth Contract.
	/// </summary>
	public class ContractAbi
	{
		// Properties -------------------------------------


		// Fields -----------------------------------------
		private object _constructorObject = null;
		private List<object> _methodObjects = new List<object>();

		// Initialization Methods -------------------------
		public ContractAbi()
		{
			
		}

		// General Methods --------------------------------
		public void AddConstructor (List<object> inputParameters)
		{
			if (_constructorObject != null)
			{
				throw new Exception("Must call AddConstructor() exactly one time.");
			}

			// constructor
			_constructorObject = new { inputs = inputParameters.ToArray(), name = "", stateMutability = "nonpayable", type = "constructor" };
		
		}


		public void AddFunction(string functionName, string stateMutability, List<object> inputParameters, List<object> outputParameters)
		{
			_methodObjects.Add(new { 
				inputs = inputParameters.ToArray(), 
				outputs = outputParameters.ToArray(), 
				name = functionName, stateMutability = stateMutability, type = "function" });

		}

		public object[] ToObjectArray()
		{
			if (_constructorObject == null)
			{
				throw new Exception("Must call AddConstructor() exactly one time.");
			}

			if (_methodObjects.Count == 0)
			{
				throw new Exception("Must call AddMethod() 1 or more times.");
			}

			List<object> _objects = new List<object>();
			_objects.Add(_constructorObject);
			_objects.AddRange(_methodObjects);
			return _objects.ToArray();
		}

	}

}
