
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model
{
	/// <summary>
	/// Wrapper class for a Web3API communication to the Solidity Smart Contract.
	/// </summary>
	public class TheGameContract : Contract
	{

		// Properties -------------------------------------
		public string TreasurePrizeContractAddress { get { return _treasurePrizeContractAddress; }
}

		// Fields -----------------------------------------
		private const bool IsLogging = true;
		private string _treasurePrizeContractAddress = "";


		// Initialization Methods -------------------------

       protected override void SetContractDetails()
       {

         _treasurePrizeContractAddress  = "0x44017567746A761c4E943b22734DF96fD5aA43eB";
         _address  = "0x217a14780BcB6E9b69300D20eEb725288Df10Df4";
         _abi      = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"goldContractAddress\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"treasurePrizeContractAddress\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"tokenURI\",\"type\":\"string\"}],\"name\":\"addTreasurePrize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"deleteAllTreasurePrizes\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addressIn\",\"type\":\"address\"}],\"name\":\"getAddress\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"addressOut\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getGold\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"balance\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getIsRegistered\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"isRegistered\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"messageIn\",\"type\":\"string\"}],\"name\":\"getMessage\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"messageOut\",\"type\":\"string\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getRewardsHistory\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"rewardString\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"register\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"safeReregisterAndDeleteAllTreasurePrizes\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"sellTreasurePrize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"targetBalance\",\"type\":\"uint256\"}],\"name\":\"setGold\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"int256\",\"name\":\"delta\",\"type\":\"int256\"}],\"name\":\"setGoldBy\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"goldAmount\",\"type\":\"uint256\"}],\"name\":\"startGameAndGiveRewards\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unregister\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

       }



		/// <summary>
		/// Format for ABI:
		///		*  ExecuteContractFunction - requires string
		///		*  RunContractFunction - requires object[]. This must be manually created from the string below
		/// </summary>
		/// <returns></returns>
		protected override object[] GetAbiObject()
        {
			ContractAbi contractAbi = new ContractAbi();
			List<object> cInputParams = new List<object>();
			cInputParams.Add(new { internalType = "address", name = "goldContractAddress", type = "address" });
			cInputParams.Add(new { internalType = "address", name = "treasurePrizeContractAddress", type = "address" });
			contractAbi.AddConstructor(cInputParams);
			
			
			//NOTE: Its ONLY required to manually recreate the methods
			//		here which you choose to call via **RunContractFunction**
			
			// getIsRegistered
			List<object> isRegistered_Input = new List<object>();
			isRegistered_Input.Add(new { internalType = "address", name = "address", type = "address" });
			List<object> isRegistered_Output = new List<object>();
			isRegistered_Output.Add(new { internalType = "bool", name = "isRegistered", type = "bool" });
			contractAbi.AddFunction("getIsRegistered", "view", isRegistered_Input, isRegistered_Output);

			// getGold
			List<object> getGold_Input = new List<object>();
			getGold_Input.Add(new { internalType = "address", name = "address", type = "address" });
			List<object> getGold_Output = new List<object>();
			getGold_Output.Add(new { internalType = "uint256", name = "balance", type = "uint256" });
			contractAbi.AddFunction("getGold", "view", getGold_Input, getGold_Output);

			// getRewardsHistory
			List<object> getRewardsHistory_Input = new List<object>();
			getRewardsHistory_Input.Add(new { internalType = "address", name = "address", type = "address" });
			List<object> getRewardsHistory_Output = new List<object>();
			getRewardsHistory_Output.Add(new { internalType = "string", name = "rewardString", type = "string" });
			contractAbi.AddFunction("getRewardsHistory", "view", getRewardsHistory_Input, getRewardsHistory_Output);

			return contractAbi.ToObjectArray();
		}

		///////////////////////////////////////////////////////////
		// RunContractFunctionAsync
		///////////////////////////////////////////////////////////

		
		// General Methods --------------------------------
		public async UniTask<bool> getIsRegistered()
		{
			string moralisUserEthAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("address", moralisUserEthAddress);

			string result = await RunContractFunctionAsync("getIsRegistered", args, IsLogging);
			bool resultBool = bool.Parse(result);
			return resultBool;
		}

		
		public async UniTask<int> getGold()
		{
			string moralisUserEthAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("address", moralisUserEthAddress);

			string goldString = await RunContractFunctionAsync("getGold", args, IsLogging);
			int goldInt = Int32.Parse(goldString);
			return goldInt;
		}


		public async UniTask<Reward> GetRewardsHistory()
		{
			string moralisUserEthAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("address", moralisUserEthAddress);

			var result = await RunContractFunctionAsync("getRewardsHistory", args, IsLogging);
			
			Reward reward = TheGameHelper.ConvertRewardStringToObject(result);
			//Debug.Log($"getRewardsHistory() result = {reward}");
			return reward;
		}


		///////////////////////////////////////////////////////////
		// ExecuteContractFunctionAsync
		///////////////////////////////////////////////////////////
		public async UniTask<string> Register()
		{
			object[] args =
			{
			};

			string result = await ExecuteContractFunctionAsync("register", args, IsLogging);

			return result;
		}

		
		public async UniTask<string> Unregister()
		{
			object[] args =
			{
			};

			string result = await ExecuteContractFunctionAsync("unregister", args, IsLogging);
			return result;
		}


		public async UniTask<string> setGold(int targetBalance2)
		{
			int targetBalance = targetBalance2;
			object[] args =
			{
				targetBalance
			};

			string result = await ExecuteContractFunctionAsync("setGold", args, IsLogging);

			return result;
		}

		
		public async UniTask<string> setGoldBy(int deltaBalance)
		{
			int delta = deltaBalance;
			object[] args =
			{
				delta
			};

			string result = await ExecuteContractFunctionAsync("setGoldBy", args, IsLogging);

			return result;
		}


		public async UniTask<string> AddTreasurePrize (TreasurePrizeDto treasurePrizeDto)
		{
			string metadata = treasurePrizeDto.Metadata;
			object[] args =
			{
				metadata
			};
			
			string result = await ExecuteContractFunctionAsync("addTreasurePrize", args, IsLogging);
			return result;
		}
		

		public async UniTask<string> SellTreasurePrize(TreasurePrizeDto treasurePrizeDto)
		{
			int tokenId = treasurePrizeDto.TokenId;
			
			if (tokenId == TreasurePrizeDto.NullTokenId)
			{
				TheGameSingleton.Debug.Log("BurnNftAsync() failed. tokenId must be NOT null. " +
				          "Was this NFT just created? Leave and return to Scene so it gets loaded from online");
				return "";
			}
				
			object[] args =
			{
				tokenId
			};
			
			const bool isLogging = true;
			return await ExecuteContractFunctionAsync("sellTreasurePrize", args, isLogging);
		}

		
		private int[] GetTokenIds(List<TreasurePrizeDto> treasurePrizeDtos)
		{
			int[] tokenIds = new int[treasurePrizeDtos.Count];
			for (int i = 0; i < treasurePrizeDtos.Count; i++)
			{
				int tokenId = treasurePrizeDtos[i].TokenId;

				if (tokenId == TreasurePrizeDto.NullTokenId)
				{
					throw new Exception("GetTokenIds() failed. tokenId must be NOT null. " +
					          "Was this NFT just created? Leave and return to Scene so it gets loaded from online");
				}

				tokenIds[i] = tokenId;
			}

			return tokenIds;
		}
		
		
		public async UniTask<string> DeleteAllTreasurePrizes(List<TreasurePrizeDto> treasurePrizeDtos)
		{
			int[] tokenIds = GetTokenIds(treasurePrizeDtos);
			object[] args =
			{
				tokenIds
			};

			const bool isLogging = true;
			string result = await ExecuteContractFunctionAsync("deleteAllTreasurePrizes", args, isLogging);
			return result;
		}
		
		
		public async UniTask<string> SafeReregisterAndDeleteAllTreasurePrizes(List<TreasurePrizeDto> treasurePrizeDtos)
		{
			int[] tokenIds = GetTokenIds(treasurePrizeDtos);
			object[] args =
			{
				tokenIds
			};

			const bool isLogging = true;
			string result = await ExecuteContractFunctionAsync("safeReregisterAndDeleteAllTreasurePrizes", args, isLogging);
			return result;
		}


		public async UniTask<string> StartGameAndGiveRewards(int goldAmount)
		{
			object[] args =
			{
				goldAmount
			};

			string result = await ExecuteContractFunctionAsync("startGameAndGiveRewards", args, IsLogging);
			return result;
		}

		
		// Event Handlers ---------------------------------
		
	}
}
