
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
namespace MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService
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
		UniTask<List<Prize>> GetPrizesAsync();

		// RunContractFunction - Various Return Types
		UniTask<bool> GetIsRegisteredAsync();
		UniTask<int> GetGoldAsync();
		UniTask<TransferLog> GetTransferLogHistoryAsync();

		// ExecuteContractFunction - Must Be String Return Type
		UniTask RegisterAsync();
		UniTask UnregisterAsync();
		UniTask TransferGoldAsync();
		UniTask TransferPrizeAsync(Prize prize);
		UniTask SafeReregisterDeleteAllPrizesAsync();
	}
}
