using System.Collections.Generic;
using System.Security.Authentication;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using Nft = MoralisUnity.Samples.Shared.Data.Types.Nft;

namespace MoralisUnity.Samples.TheGame.MVCS.Service
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
            bool result = await _theGameContract.getIsRegistered();
            return result;
        }
        
        
        public async UniTask<Reward> GetRewardsHistoryAsync()
        {
            Reward result = await _theGameContract.GetRewardsHistory();
            return result;
        }


        public async UniTask<int> GetGoldAsync()
        {
            int result = await _theGameContract.getGold();
            return result;
        }
        
        
        public async UniTask<List<TreasurePrizeDto>> GetTreasurePrizesAsync()
        {
            // Create Method Return Value
            List<TreasurePrizeDto> treasurePrizeDtos = new List<TreasurePrizeDto>();

            // Check System Status
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.IsAuthenticatedAsync();
            if (!isAuthenticated)
            {
                // Sometimes, ONLY warn
                throw new AuthenticationException();
            }

            // Get NFT Info
            string ethAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
            MyMoralisWrapper.CustomNftOwnerCollection customNftOwnerCollection = await MyMoralisWrapper.Instance.GetNFTsForContract(
                ethAddress,
                _theGameContract.PrizeContractAddress);

            // Create Method Return Value
            foreach (MyMoralisWrapper.CustomNftOwner customNftOwner in customNftOwnerCollection.CustomResult)
            {
                string ownerAddress = customNftOwner.OwnerOf;
                string tokenIdString = customNftOwner.TokenId;
                string metadata = customNftOwner.TokenUri;
                TreasurePrizeDto prizeDto = Nft.CreateNewFromMetadata<TreasurePrizeDto>(ownerAddress, tokenIdString, metadata);
                treasurePrizeDtos.Add(prizeDto);
            }

            // Finalize Method Return Value
            return treasurePrizeDtos;
        }
        
        // SETTER Methods -------------------------
        public async UniTask RegisterAsync()
        {
            string result = await _theGameContract.Register();
            //Debug.Log($"RegisterAsync() result = {result}");
        }


        public async UniTask StartGameAndGiveRewardsAsync(int goldAmount)
        {
            string result = await _theGameContract.StartGameAndGiveRewards(goldAmount);
            //Debug.Log($"StartGameAndGiveRewardsAsync() result = {result}");
        }


        public async UniTask UnregisterAsync()
        {
            string result = await _theGameContract.Unregister();
            //Debug.Log($"UnregisterAsync() result = {result}");
        }


        public async UniTask SetGoldAsync(int targetBalance)
        {
            string result = await _theGameContract.setGold(targetBalance);
            //Debug.Log($"SetGoldAsync() result = {result}");
        }

        
        public async UniTask SetGoldByAsync(int deltaBalance)
        {
            string result = await _theGameContract.setGoldBy(deltaBalance);
            //Debug.Log($"SetGoldByAsync() result = {result}");
        }


        public async UniTask AddTreasurePrizeAsync(TreasurePrizeDto prizeToAdd)
        {
            string result = await _theGameContract.AddTreasurePrize(prizeToAdd);
            //Debug.Log($"AddTreasurePrizeAsync() result = {result}");
        }


        public async UniTask SellTreasurePrizeAsync(TreasurePrizeDto prizeDto)
        {
            string result = await _theGameContract.SellTreasurePrize(prizeDto);
            //Debug.Log($"SellTreasurePrizeAsync() result = {result}");
        }

        
        public async UniTask DeleteAllTreasurePrizeAsync()
        {
            List<TreasurePrizeDto> treasurePrizeDtos = await GetTreasurePrizesAsync();
            string result = await _theGameContract.DeleteAllTreasurePrizes(treasurePrizeDtos);
            //Debug.Log($"DeleteAllTreasurePrizeAsync() result = {result}");
        }

        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllTreasurePrizeAsync()
        {
            List<TreasurePrizeDto> treasurePrizeDtos = await GetTreasurePrizesAsync();
            string result = await _theGameContract.SafeReregisterAndDeleteAllTreasurePrizes(treasurePrizeDtos);
            //Debug.Log($"SafeReregisterDeleteAllTreasurePrizeAsync() result = {result}");
        }
        
        // Event Handlers ---------------------------------

    }
}
