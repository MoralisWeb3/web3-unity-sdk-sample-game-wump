/*
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;

#pragma warning disable CS1998
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Service
{
	/// <summary>
	/// Handles communication with external sources (e.g. database/servers/contracts)
	///		* See <see cref="TheGameSingleton"/> 
	/// </summary>
	public class TheGameContractService : ITheGameService
	{
		// Properties -------------------------------------
		public PendingMessage PendingMessageActive { get { return _endingMessageActive; }}
		public PendingMessage PendingMessagePassive { get { return _pendingMessagePassive; }}
        public PendingMessage PendingMessageExtraDelay { get { return _pendingMessageExtraDelay; }}
        public bool HasExtraDelay { get { return true; }}
        
		// Fields -----------------------------------------
		private readonly PendingMessage _endingMessageActive = new PendingMessage("Confirm With Your Wallet", 0);
		private readonly PendingMessage _pendingMessagePassive = new PendingMessage("Loading ...", 0);
        private readonly PendingMessage _pendingMessageExtraDelay = new PendingMessage("Waiting For Transaction ...", 0);
		private readonly TheGameContract _theGameContract = null;
        
        // Based on trial and error (and current network traffic)
        //  This is how long it takes for the state to change on the blockchain
        private const int DelayExtraAfterStateChangeMilliseconds = 5000;

		// Initialization Methods -------------------------
		public TheGameContractService()
		{
			_theGameContract = new TheGameContract();
		}

        
        // DELAY Methods -------------------------
        
        // Wait for contract values to sync so the client will see the changes
        public UniTask DoExtraDelayAsync()
        {
            return UniTask.Delay(DelayExtraAfterStateChangeMilliseconds);
        }

        
        // GETTER Methods -------------------------

        public async UniTask<bool> GetIsRegisteredAsync()
        {
            
            //Example of RunContractFunction - Let's look at how that is called.
            
            //TODO: Return real value(s)
            bool result = true;
            return result;
        }
        
        
        public async UniTask<Reward> GetRewardsHistoryAsync()
        {
            //TODO: Return real value(s)
            Reward result = new Reward
            {
                Title = "Temp Gold Title",
                Type = TheGameHelper.GetRewardType(TheGameHelper.RewardGold),
                Price = 33
            };
            return result;
        }


        public async UniTask<int> GetGoldAsync()
        {
            //TODO: Return real value(s)
            int result = 99;
            return result;
        }
        
        
        public async UniTask<List<TreasurePrizeDto>> GetTreasurePrizesAsync()
        {
            //TODO: Return real value(s)
            List<TreasurePrizeDto> treasurePrizeDtos = new List<TreasurePrizeDto>();
            return treasurePrizeDtos;
        }
        
        // SETTER Methods -------------------------
        public async UniTask RegisterAsync()
        {
            //Example of ExecuteContractFunction - Let's look at how that is called
            
            //TODO: Register the user
        }


        public async UniTask StartGameAndGiveRewardsAsync(int goldAmount)
        {
            //TODO: start the game, reward the player randomly with either gold or prizes
        }


        public async UniTask UnregisterAsync()
        {
            //TODO: unregister
        }


        public async UniTask SetGoldAsync(int targetBalance)
        {
            //TODO: Set real values
        }

        
        public async UniTask SetGoldByAsync(int deltaBalance)
        {
            //TODO: Set real values
        }


        public async UniTask AddTreasurePrizeAsync(TreasurePrizeDto treasurePrizeToAdd)
        {
            //TODO: Add the prize
        }


        public async UniTask SellTreasurePrizeAsync(TreasurePrizeDto treasurePrizeDto)
        {
            //TODO: Sell the prizes
        }

        
        public async UniTask DeleteAllTreasurePrizeAsync()
        {
            //TODO: Get list of prizes
            List<TreasurePrizeDto> treasurePrizeDtos = new List<TreasurePrizeDto>();

            //TODO: Delete all treasures
            
        }

        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllTreasurePrizeAsync()
        {
            //TODO: Get list of prizes
            List<TreasurePrizeDto> treasurePrizeDtos = new List<TreasurePrizeDto>();
            
            //TODO: Delete all treasures
            
        }
        
        // Event Handlers ---------------------------------

    }
}
*/
