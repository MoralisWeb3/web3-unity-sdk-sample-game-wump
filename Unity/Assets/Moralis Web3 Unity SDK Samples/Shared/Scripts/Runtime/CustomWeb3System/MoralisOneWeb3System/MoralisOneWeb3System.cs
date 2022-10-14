using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.Shared.Interfaces;
using UnityEngine;

//////////////////////
using MoralisUnity.Web3Api.Models;
using WalletConnectSharp.Unity;
using MoralisUnity.Platform.Objects;
using Nethereum.Hex.HexTypes; //Keep this
//////////////////////


#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class MoralisOneWeb3System : CustomSingleton<MoralisOneWeb3System>, 
		IInitializable, ICustomSingletonParent, ICustomWeb3System
	{
		// Properties -------------------------------------


		// Fields -----------------------------------------

		public bool IsInitialized { get; private set; }
		

		public bool HasWalletConnectInstance
		{
			get { return WalletConnect.Instance != null; }
		}

		public void DestroyWalletConnectInstance()
		{
			GameObject.Destroy(WalletConnect.Instance.gameObject);
		}

		// Initialization Methods -------------------------
		void ICustomSingletonParent.OnInstantiatedChild()
		{
			//TODO: Move screen sizing to some OTHER centralized code placement
			//Set screen size for Standalone
#if UNITY_STANDALONE
			Screen.SetResolution(675, 1000, false);
			Screen.fullScreen = false;
#endif

			// Auto-Initialize
			Initialize();

		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				return;
			}

			Debug.Log("Init ))))))))))))))))))))))))))");
			// HasMoralisUserAsync may be called early. So ensure this is called
			Moralis.Start();
			IsInitialized = true;
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}


		// General Methods --------------------------------
		public async UniTask<bool> IsAuthenticatedAsync()
		{
			RequireIsInitialized();

			return await HasMoralisUserAsync();
		}
		
		private async UniTask<bool> HasMoralisUserAsync()
		{
			RequireIsInitialized();

			// Determines if Moralis is logged in with an active user.
			MoralisUser moralisUser = await GetMoralisUserAsync();
			return moralisUser != null;
		}

		public async Task<string> GetWeb3UserAddressAsync()
		{
			MoralisUser moralisUser = await GetMoralisUserAsync();
			return moralisUser.ethAddress;
		}
		
		public string ConvertWeb3AddressToShortFormat(string web3Address)
		{
			const int n = 6;
			if (string.IsNullOrEmpty(web3Address))
			{
				return string.Empty;
			}
        
			if (web3Address.Length < n)
			{
				return web3Address;
			}

			return $"{web3Address.Substring(0, n)}...{web3Address.Substring(web3Address.Length - n)}";
		}
		
		
		public async UniTask<MoralisUser> GetMoralisUserAsync()
		{
			RequireIsInitialized();
			return await Moralis.GetUserAsync();
		}

		public async UniTask<String> ExecuteContractFunction(string _address, string _abi,
			string functionName, object[] args, bool isLogging)
		{

			MoralisUser moralisUser = await GetMoralisUserAsync();

			if (moralisUser == null)
			{
				throw new RequiredMoralisUserException();
			}


			if (!HasWalletConnectInstance)
			{
				throw new NullReferenceException("ExecuteContractFunction() failed. " +
				                                 SharedConstants.WalletConnectNullReferenceException);
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
			return await Moralis.ExecuteContractFunction(_address, _abi, functionName, args, value, gas, gasPrice);
		}

		// Event Handlers ---------------------------------


		public async UniTask<T> RunContractFunction<T>(string address, string functionName,
			object[] abiObject, Dictionary<string, object> args, bool isLogging)
		{

			MoralisUser moralisUser = await GetMoralisUserAsync();

			if (moralisUser == null)
			{
				throw new RequiredMoralisUserException();
			}

			// Prepare the contract request
			RunContractDto runContractDto = new RunContractDto()
			{
				Abi = abiObject,
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
				stringBuilder.AppendLine($"\taddress		= {address}");
				stringBuilder.AppendLine($"\tfunctionName	= {functionName}");
				stringBuilder.AppendLine($"\trunContractDto.Abi.Length	= {abiLength}");
				stringBuilder.AppendLine($"\trunContractDto.Params.Length	= {paramsLength}");
				Custom.Debug.Log($"{stringBuilder.ToString()}");
			}


			///////////////////////////////////////////
			// Execute: RunContractFunction	
			///////////////////////////////////////////
			MoralisClient moralisClient = Moralis.GetClient();

			return await Moralis.Client.Web3Api.Native.RunContractFunction<T>(address, functionName,
				runContractDto, MoralisUnity.Web3Api.Models.ChainList.mumbai);
		}

		public async UniTask<CustomNftOwnerCollection> GetNFTsForContract(string ethAddress, string contractAddress)
		{

			NftOwnerCollection nftOwnerCollection = await Moralis.Web3Api.Account.GetNFTsForContract(
				ethAddress,
				contractAddress,
				ChainList.mumbai);

			CustomNftOwnerCollection customNftOwnerCollection = new CustomNftOwnerCollection();
			customNftOwnerCollection.Page = nftOwnerCollection.Page;
			customNftOwnerCollection.Total = nftOwnerCollection.Total;
			customNftOwnerCollection.PageSize = nftOwnerCollection.PageSize;
			customNftOwnerCollection.Status = nftOwnerCollection.Status;
			customNftOwnerCollection.Page = nftOwnerCollection.Page;

			List<CustomNftOwner> customNftOwners = new List<CustomNftOwner>();
			foreach (NftOwner nftOwner in nftOwnerCollection.Result)
			{
				CustomNftOwner customNftOwner = new CustomNftOwner();
				customNftOwner.Amount = nftOwner.Amount;
				customNftOwner.Metadata = nftOwner.Metadata;
				customNftOwner.Name = nftOwner.Name;
				customNftOwner.Symbol = nftOwner.Symbol;
				customNftOwner.BlockNumber = nftOwner.BlockNumber;
				customNftOwner.ContractType = nftOwner.ContractType;
				customNftOwner.OwnerOf = nftOwner.OwnerOf;
				customNftOwner.SyncedAt = nftOwner.SyncedAt;
				customNftOwner.TokenAddress = nftOwner.TokenAddress;
				customNftOwner.TokenId = nftOwner.TokenId;
				customNftOwner.TokenUri = nftOwner.TokenUri;
				customNftOwner.BlockNumberMinted = nftOwner.BlockNumberMinted;
				customNftOwners.Add(customNftOwner);
			}

			customNftOwnerCollection.CustomResult = customNftOwners;
			return customNftOwnerCollection;

		}
	}
}

