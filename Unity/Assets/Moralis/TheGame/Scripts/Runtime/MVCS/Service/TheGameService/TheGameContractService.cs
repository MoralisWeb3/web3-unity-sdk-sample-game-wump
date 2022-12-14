using System.Collections.Generic;
using System.Security.Authentication;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using UnityEngine;
using Nft = MoralisUnity.Samples.Shared.Data.Types.Nft;

namespace MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService
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
        private const int DelayExtraAfterStateChangeMilliseconds = 7000; // I tested with 5000 and it die not always capture changes. Use more - srivello

        
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
            bool result = await _theGameContract.getIsRegisteredAsync();
            return result;
        }
        
        
        public async UniTask<TransferLog> GetTransferLogHistoryAsync()
        {
            TransferLog transferLog = await _theGameContract.GetTransferLogHistoryAsync();
            return transferLog;
        }


        public async UniTask<int> GetGoldAsync()
        {
            int result = await _theGameContract.getGoldAsync();
            return result;
        }
        
        public async UniTask<List<Prize>> GetPrizesAsync()
        {
            
            // Create Method Return Value
            List<Prize> prizes = new List<Prize>();

            // Check System Status
            bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAndUpdateModelAsync();
            if (!isAuthenticated)
            {
                // Sometimes, ONLY warn
                throw new AuthenticationException();
            }

            // Get NFT Info
            List<NftOwner> nftOwners = await CustomWeb3System.Instance.GetNFTsForContractAsync(
                _theGameContract.PrizeContractAddress);
            
            // Create Method Return Value
            foreach (NftOwner nftOwner in nftOwners)
            {
                string ownerAddress = nftOwner.OwnerOf;
                string tokenIdString = nftOwner.TokenId;
                string metadata = nftOwner.TokenUri;
                Prize prize = Nft.CreateNewFromMetadata<Prize>(ownerAddress, tokenIdString, metadata);
                prizes.Add(prize);
            }
            
            // Finalize Method Return Value
            return prizes;
        }
        
        
        // SETTER Methods -------------------------
        public async UniTask RegisterAsync()
        {
            string result = await _theGameContract.RegisterAsync();
            //Debug.Log($"RegisterAsync() result = {result}");
        }


        public async UniTask UnregisterAsync()
        {
            List<Prize> prizes = await GetPrizesAsync();
            string result = await _theGameContract.UnregisterAsync(prizes);
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        
        public async UniTask TransferGoldAsync(string toAddress)
        {
            string result = await _theGameContract.TransferGoldAsync(toAddress);
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        
        public async UniTask TransferPrizeAsync(string toAddress, Prize prize)
        {
            string result = await _theGameContract.TransferPrizeAsync(toAddress, prize);
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        
        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllPrizesAsync()
        {
            List<Prize> prizes = await GetPrizesAsync();
            string result = await _theGameContract.SafeReregisterAndDeleteAllPrizesAsync(prizes);
            //Debug.Log($"SafeReregisterDeleteAllPrizesAsync() result = {result}");
        }
        
        
        // Event Handlers ---------------------------------

    }
}
