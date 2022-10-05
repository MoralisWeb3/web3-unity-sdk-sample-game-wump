using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Constants;
using MoralisUnity.Sdk.Interfaces;
using MoralisUnity.Sdk.Utilities;
using MoralisUnity.Web3Api.Models;
using UnityEngine;
using WalletConnectSharp.Unity;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class MyMoralisWrapper : CustomSingleton<MyMoralisWrapper>, IInitializable, ICustomSingletonParent
	{
		// Properties -------------------------------------


		// Fields -----------------------------------------
		public const string PathMoralisSamplesCreateAssetMenu = MoralisConstants.PathMoralisSamplesCreateAssetMenu;
		public const string Web3UnitySDK = MoralisConstants.Web3UnitySDK;

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

		public async Task<string> GetMoralisUserEthAddressAsync()
		{
			MoralisUser moralisUser = await GetMoralisUserAsync();
			return moralisUser.ethAddress;
		}

		private async UniTask<MoralisUser> GetMoralisUserAsync()
		{
			RequireIsInitialized();
			return await Moralis.GetUserAsync();
		}

		public string GetWeb3AddressShortFormat(string str, int n = 6)
		{
			return Formatters.GetWeb3AddressShortFormat(str, n);
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

		public class CustomNftOwnerCollection
		{
			/// <summary>
			/// The syncing status of the address [SYNCING/SYNCED]
			/// example: SYNCING
			/// </summary>
			[DataMember(Name = "status", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "status")]
			public string Status { get; set; }

			/// <summary>
			/// The total number of matches for this query
			/// example: 2000
			/// </summary>
			[DataMember(Name = "total", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "total")]
			public int? Total { get; set; }

			/// <summary>
			/// The page of the current result
			/// example: 2
			/// </summary>
			[DataMember(Name = "page", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "page")]
			public int? Page { get; set; }

			/// <summary>
			/// The number of results per page
			/// example: 100
			/// </summary>
			[DataMember(Name = "page_size", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "page_size")]
			public int? PageSize { get; set; }

			/// <summary>
			/// </summary>
			[DataMember(Name = "result", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "result")]
			public List<CustomNftOwner> CustomResult { get; set; }


			/// <summary>
			/// Get the string presentation of the object
			/// </summary>
			/// <returns>String presentation of the object</returns>
			public override string ToString()
			{
				var sb = new StringBuilder();
				sb.Append("class NftOwnerCollection{");
				sb.Append("  Status ").Append(Status).Append("\n");
				sb.Append("  Total ").Append(Total).Append("\n");
				sb.Append("  Page ").Append(Page).Append("\n");
				sb.Append("  PageSize ").Append(PageSize).Append("\n");
				sb.Append("  Result ").Append(CustomResult).Append("\n");
				sb.Append("}");

				return sb.ToString();
			}

			/// <summary>
			/// Get the JSON string presentation of the object
			/// </summary>
			/// <returns>JSON string presentation of the object</returns>
			public string ToJson()
			{
				return JsonConvert.SerializeObject(this, Formatting.Indented);
			}

		}

		[DataContract]
		public class CustomNftOwner
		{
			/// <summary>
			/// The address of the contract of the NFT
			/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
			/// </summary>
			[DataMember(Name = "token_address", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "token_address")]
			public string TokenAddress { get; set; }

			/// <summary>
			/// The token id of the NFT
			/// example: 15
			/// </summary>
			[DataMember(Name = "token_id", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "token_id")]
			public string TokenId { get; set; }

			/// <summary>
			/// The type of NFT contract standard
			/// example: ERC721
			/// </summary>
			[DataMember(Name = "contract_type", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "contract_type")]
			public string ContractType { get; set; }

			/// <summary>
			/// The address of the owner of the NFT
			/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
			/// </summary>
			[DataMember(Name = "owner_of", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "owner_of")]
			public string OwnerOf { get; set; }

			/// <summary>
			/// The blocknumber when the amount or owner changed
			/// example: 88256
			/// </summary>
			[DataMember(Name = "block_number", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "block_number")]
			public string BlockNumber { get; set; }

			/// <summary>
			/// The blocknumber when the NFT was minted
			/// example: 88256
			/// </summary>
			[DataMember(Name = "block_number_minted", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "block_number_minted")]
			public string BlockNumberMinted { get; set; }

			/// <summary>
			/// The uri to the metadata of the token
			/// </summary>
			[DataMember(Name = "token_uri", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "token_uri")]
			public string TokenUri { get; set; }

			/// <summary>
			/// The metadata of the token
			/// </summary>
			[DataMember(Name = "metadata", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "metadata")]
			public string Metadata { get; set; }

			/// <summary>
			/// when the metadata was last updated
			/// </summary>
			[DataMember(Name = "synced_at", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "synced_at")]
			public string SyncedAt { get; set; }

			/// <summary>
			/// The number of this item the user owns (used by ERC1155)
			/// example: 1
			/// </summary>
			[DataMember(Name = "amount", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "amount")]
			public string Amount { get; set; }

			/// <summary>
			/// The name of the Token contract
			/// example: CryptoKitties
			/// </summary>
			[DataMember(Name = "name", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "name")]
			public string Name { get; set; }

			/// <summary>
			/// The symbol of the NFT contract
			/// example: RARI
			/// </summary>
			[DataMember(Name = "symbol", EmitDefaultValue = false)]
			[JsonProperty(PropertyName = "symbol")]
			public string Symbol { get; set; }


			/// <summary>
			/// Get the string presentation of the object
			/// </summary>
			/// <returns>String presentation of the object</returns>
			public override string ToString()
			{
				var sb = new StringBuilder();
				sb.Append("class NftOwner{");
				sb.Append("  TokenAddress ").Append(TokenAddress).Append("\n");
				sb.Append("  TokenId ").Append(TokenId).Append("\n");
				sb.Append("  ContractType ").Append(ContractType).Append("\n");
				sb.Append("  OwnerOf ").Append(OwnerOf).Append("\n");
				sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
				sb.Append("  BlockNumberMinted ").Append(BlockNumberMinted).Append("\n");
				sb.Append("  TokenUri ").Append(TokenUri).Append("\n");
				sb.Append("  Metadata ").Append(Metadata).Append("\n");
				sb.Append("  SyncedAt ").Append(SyncedAt).Append("\n");
				sb.Append("  Amount ").Append(Amount).Append("\n");
				sb.Append("  Name ").Append(Name).Append("\n");
				sb.Append("  Symbol ").Append(Symbol).Append("\n");
				sb.Append("}");

				return sb.ToString();
			}

			/// <summary>
			/// Get the JSON string presentation of the object
			/// </summary>
			/// <returns>JSON string presentation of the object</returns>
			public string ToJson()
			{
				return JsonConvert.SerializeObject(this, Formatting.Indented);
			}
		}
	}
}

