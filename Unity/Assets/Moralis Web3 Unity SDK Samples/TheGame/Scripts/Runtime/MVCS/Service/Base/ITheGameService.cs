
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;

namespace MoralisUnity.Samples.TheGame.MVCS.Service
{
	/// <summary>
	/// Handles communication with external sources (e.g. database/servers/contracts)
	///		* See <see cref="TheGameSingleton"/> 
	/// </summary>
	public interface ITheGameService 
	{
		// Properties -------------------------------------
		PendingMessage PendingMessageActive { get; }
		PendingMessage PendingMessagePassive { get; }
		PendingMessage PendingMessageExtraDelay { get; }
		bool HasExtraDelay { get; }

		// General Methods --------------------------------
		
		// Wait for contract values to sync so the client will see the changes
		UniTask DoExtraDelayAsync();

		// Web3 API Call - Various Return Types
		UniTask<List<TreasurePrizeDto>> GetTreasurePrizesAsync();

		// RunContractFunction - Various Return Types
		UniTask<bool> GetIsRegisteredAsync();
		UniTask<int> GetGoldAsync();
		UniTask<Reward> GetRewardsHistoryAsync();

		// ExecuteContractFunction - Must Be String Return Type
		UniTask RegisterAsync();
		UniTask UnregisterAsync();
		UniTask SetGoldAsync(int targetBalance);
		UniTask SetGoldByAsync(int deltaBalance);
		UniTask StartGameAndGiveRewardsAsync(int goldAmount);
		UniTask AddTreasurePrizeAsync(TreasurePrizeDto prizeToAdd);
		UniTask SellTreasurePrizeAsync(TreasurePrizeDto prizeDto);
		UniTask DeleteAllTreasurePrizeAsync();
		UniTask SafeReregisterDeleteAllTreasurePrizeAsync();
	}
}
