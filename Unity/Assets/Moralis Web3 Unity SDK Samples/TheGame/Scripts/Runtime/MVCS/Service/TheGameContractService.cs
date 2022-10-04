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
        
        
        public async UniTask<TransferLog> GetTransferLogHistoryAsync()
        {
            TransferLog result = await _theGameContract.GetTransferLogHistoryAsync();
            return result;
        }


        public async UniTask<int> GetGoldAsync()
        {
            int result = await _theGameContract.getGold();
            return result;
        }
        
        
        public async UniTask<List<Prize>> GetPrizesAsync()
        {
            // Create Method Return Value
            List<Prize> treasurePrizeDtos = new List<Prize>();

            // Check System Status
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
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
                Prize prize = Nft.CreateNewFromMetadata<Prize>(ownerAddress, tokenIdString, metadata);
                treasurePrizeDtos.Add(prize);
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


        public async UniTask UnregisterAsync()
        {
            string result = await _theGameContract.Unregister();
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        public async UniTask TransferGoldAsync()
        {
            string result = await _theGameContract.TransferGold();
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        public async UniTask TransferPrizeAsync()
        {
            string result = await _theGameContract.TransferPrize();
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllPrizesAsync()
        {
            List<Prize> treasurePrizeDtos = await GetPrizesAsync();
            string result = await _theGameContract.SafeReregisterAndDeleteAllPrizes(treasurePrizeDtos);
            //Debug.Log($"SafeReregisterDeleteAllTreasurePrizeAsync() result = {result}");
        }
        
        // Event Handlers ---------------------------------

    }
}
