
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;

namespace MoralisUnity.Samples.TheGame.MVCS.Model
{
	/// <summary>
	/// Wrapper class for a Web3API communication to the Solidity Smart Contract.
	/// </summary>
	public class TheGameContract : Contract
	{

		// Properties -------------------------------------
		public string PrizeContractAddress { get { return _prizeContractAddress; }
}

		// Fields -----------------------------------------
		private const bool IsLogging = true;
		private string _prizeContractAddress = "";


		// Initialization Methods -------------------------

		protected override void SetContractDetails()
		{

		     _prizeContractAddress  = "0x67612ceF7D53f50Dc0E5A82ADCD5a984260CC833";
		     _address  = "0x496745d30CFa7b3734d34C04Ff2Ed603f9F155ED";
		     _abi      = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"goldContractAddress\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"prizeContractAddress\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"tokenURI\",\"type\":\"string\"}],\"name\":\"addPrize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"deleteAllPrizes\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getGold\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"balance\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getIsOwnerOfPrize\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"isOwnerOfPrize\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getIsRegistered\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"isRegistered\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"userAddress\",\"type\":\"address\"}],\"name\":\"getTransferLogHistory\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"transferLogString\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"register\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"safeReregisterAndDeleteAllPrizes\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"targetBalance\",\"type\":\"uint256\"}],\"name\":\"setGold\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"int256\",\"name\":\"delta\",\"type\":\"int256\"}],\"name\":\"setGoldBy\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"toAddress\",\"type\":\"address\"}],\"name\":\"transferGold\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"toAddress\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferPrize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"unregister\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

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
			if (!await MyMoralisWrapper.Instance.IsAuthenticatedAsync())
			{
				return false;
			}
			
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


		public async UniTask<TransferLog> GetTransferLogHistoryAsync()
		{
			string moralisUserEthAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("address", moralisUserEthAddress);

			var result = await RunContractFunctionAsync("getTransferLogHistory", args, IsLogging);
			
			TransferLog transferLog = TheGameHelper.ConvertRewardStringToObject(result);
			//Debug.Log($"getRewardsHistory() result = {reward}");
			return transferLog;
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

		public async UniTask<string> TransferGold()
		{
			//Second account for testing
			string address = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e"; //await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			object[] args =
			{
				address
			};

			string result = await ExecuteContractFunctionAsync("transferGold", args, IsLogging);
			return result;
		}
		
		public async UniTask<string> TransferPrize()
		{
			//Second account for testing
			string address = "0x1FdafeC82b2fcD83BbE74a1cfeC616d57709963e"; //await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
			object[] args =
			{
				address
			};
			
			string result = await ExecuteContractFunctionAsync("transferPrize", args, IsLogging);
			return result;
		}
		

		private int[] GetTokenIds(List<Prize> prizes)
		{
			int[] tokenIds = new int[prizes.Count];
			for (int i = 0; i < prizes.Count; i++)
			{
				int tokenId = prizes[i].TokenId;

				if (tokenId == Prize.NullTokenId)
				{
					throw new Exception("GetTokenIds() failed. tokenId must be NOT null. " +
					          "Was this NFT just created? Leave and return to Scene so it gets loaded from online");
				}

				tokenIds[i] = tokenId;
			}

			return tokenIds;
		}
		
		public async UniTask<string> SafeReregisterAndDeleteAllPrizes(List<Prize> prizes)
		{
			int[] tokenIds = GetTokenIds(prizes);
			object[] args =
			{
				tokenIds
			};

			const bool isLogging = true;
			string result = await ExecuteContractFunctionAsync("safeReregisterAndDeleteAllPrizes", args, isLogging);
			return result;
		}


		// Event Handlers ---------------------------------
		
	}
}
