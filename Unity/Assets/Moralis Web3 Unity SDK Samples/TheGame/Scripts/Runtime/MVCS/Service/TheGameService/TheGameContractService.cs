using System.Collections.Generic;
using System.Security.Authentication;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types;
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
            bool result = await _theGameContract.getIsRegisteredAsync();
            return result;
        }
        
        
        public async UniTask<TransferLog> GetTransferLogHistoryAsync()
        {
            TransferLog transferLog = await _theGameContract.GetTransferLogHistoryAsync();

            if (transferLog != null)
            {
                //TODO: IDEA: The MULTIPLAYER CLIENT can poll every 5 seconds for "What is the log history for My web3 address?"
                // And if there is a result it can send a SERVER RPC to tell EVERYONE to display this message top the screen for 5 seconds
                Debug.Log(TheGameHelper.GetTransferLogDisplayText(transferLog));

            }
         
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
                prizes.Add(prize);
            }

            Debug.Log("GetPrizesAsync() count = " + prizes.Count);
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

        public async UniTask TransferGoldAsync()
        {
            string result = await _theGameContract.TransferGoldAsync();
            //Debug.Log($"UnregisterAsync() result = {result}");
        }

        public async UniTask TransferPrizeAsync(Prize prize)
        {
            string result = await _theGameContract.TransferPrizeAsync(prize);
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
