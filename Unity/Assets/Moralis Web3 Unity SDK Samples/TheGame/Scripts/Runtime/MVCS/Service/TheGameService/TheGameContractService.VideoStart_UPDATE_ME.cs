/*
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
	        //TODO: Implement this...
        }

        
        // GETTER Methods -------------------------
        public async UniTask<bool> GetIsRegisteredAsync()
        {
	        //TODO: Implement this...
        }
        
        
        public async UniTask<TransferLog> GetTransferLogHistoryAsync()
        {
	        //TODO: Implement this...
        }


        public async UniTask<int> GetGoldAsync()
        {
	        //TODO: Implement this...
        }
        
        
        public async UniTask<List<Prize>> GetPrizesAsync()
        {
	        //TODO: Implement this...
        }
            
        
        
        // SETTER Methods -------------------------
        public async UniTask RegisterAsync()
        {
            //TODO: Implement this...
        }


        public async UniTask UnregisterAsync()
        {
            //TODO: Implement this...
        }

        
        public async UniTask TransferGoldAsync(string toAddress)
        {
            //TODO: Implement this...
        }

        
        public async UniTask TransferPrizeAsync(string toAddress, Prize prize)
        {
            //TODO: Implement this...
        }

        
        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllPrizesAsync()
        {
           //TODO: Implement this...
        }
        
        
        // Event Handlers ---------------------------------

    }
}


*/