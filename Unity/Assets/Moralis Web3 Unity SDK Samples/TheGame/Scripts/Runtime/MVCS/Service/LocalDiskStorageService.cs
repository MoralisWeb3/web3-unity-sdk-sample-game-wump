using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Shared.Attributes;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;

namespace MoralisUnity.Samples.TheGame.MVCS.Service
{

	/// <summary>
	/// The object to be written/read to local disk
	/// </summary>
	[CustomFilePath(LocalDiskStorage.Title + "/TheGameLocalDiskStorage.txt", CustomFilePathLocation.StreamingAssetsPath)]
	[System.Serializable]
	public class LocalDiskStorageData
	{
		//  Properties ------------------------------------

		//  Fields ----------------------------------------
		public bool IsRegistered = false;
		public int Gold = 0;
		public List<Prize> TreasurePrizeDtos = new List<Prize>();
	}
	
	
	/// <summary>
	/// Depending on <see cref="TheGameServiceType"/> this is enabled.
	///		* Handles connection to external resource of Moralis Database
	/// </summary>
	public class LocalDiskStorageService : ITheGameService
	{
		// Properties -------------------------------------
		public PendingMessage PendingMessageActive { get { return _pendingMessageActive; }}
		public PendingMessage PendingMessagePassive { get { return _pendingMessagePassive; }}
		public PendingMessage PendingMessageExtraDelay { get { return _pendingMessageExtraDelay; }}
		public bool HasExtraDelay { get { return false; }}
		
		// Fields -----------------------------------------
		private readonly PendingMessage _pendingMessageActive = new PendingMessage("Loading ...", 500);
		private readonly PendingMessage _pendingMessagePassive = new PendingMessage("Loading ...", 500);
		private readonly PendingMessage _pendingMessageExtraDelay = new PendingMessage("Waiting ...", 0);

		// While LocalDiskStorage is FAST, add some delays to test the UI "Loading..." text, etc...
		private static readonly int DelaySimulatedPerMethod = 100;
		private static readonly int DelayExtraSimulatedAfterStateChange = 500;
		
		//
		private TransferLog _lastTransferLog;
		
		
		// Initialization Methods -------------------------
		public LocalDiskStorageService()
		{

		}
		
		// DELAY Methods -------------------------
		
		// Wait for contract values to sync so the client will see the changes
		public UniTask DoExtraDelayAsync()
		{
			return UniTask.Delay(DelayExtraSimulatedAfterStateChange);
		}
		
		//  GETTER - LocalDiskStorage Methods --------------------------------
		public UniTask<TransferLog> GetTransferLogHistoryAsync()
		{
			return new UniTask<TransferLog>(_lastTransferLog); 
		}

		public async UniTask<bool> GetIsRegisteredAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			return localDiskStorageData.IsRegistered;
		}
		
		public async UniTask<int> GetGoldAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);
	        
			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
	        
			return localDiskStorageData.Gold;
		}
		
		public async UniTask<List<Prize>> GetPrizesAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

			return localDiskStorageData.TreasurePrizeDtos;
		}

		//  SETTER - LocalDiskStorage Methods --------------------------------
		public static LocalDiskStorageData LoadLocalDiskStorageData()
		{
			bool hasTheGameLocalDiskStorage = LocalDiskStorage.Instance.Has<LocalDiskStorageData>();

			LocalDiskStorageData localDiskStorageData = null;
			if (hasTheGameLocalDiskStorage)
			{
				///////////////////////////////////////////
				// Execute: Load
				///////////////////////////////////////////
				localDiskStorageData = LocalDiskStorage.Instance.Load<LocalDiskStorageData>();
			}
			else
			{
				///////////////////////////////////////////
				// Execute: Create
				///////////////////////////////////////////
				localDiskStorageData = new LocalDiskStorageData();
				Custom.Debug.LogWarning("Creating LocalDiskStorageData");
			}
			return localDiskStorageData;
		}

		
		private static bool SaveLocalDiskStorageData(LocalDiskStorageData localDiskStorageData)
		{
			///////////////////////////////////////////
			// Execute: Save
			///////////////////////////////////////////
			bool isSuccess = LocalDiskStorage.Instance.Save<LocalDiskStorageData>(localDiskStorageData);
			return isSuccess;
		}

		
		private static bool ClearTheGameLocalDiskStorage()
		{
			///////////////////////////////////////////
			// Execute: Save
			///////////////////////////////////////////
			LocalDiskStorageData localDiskStorageData = new LocalDiskStorageData();
			bool isSuccess = LocalDiskStorage.Instance.Save<LocalDiskStorageData>(localDiskStorageData);
			return isSuccess;
		}

		//  General Methods --------------------------------
		
        public async UniTask RegisterAsync()
        {
			await UniTask.Delay(DelaySimulatedPerMethod);

			ClearTheGameLocalDiskStorage();
			
			await SetGoldAsync(100);
			
			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.IsRegistered = true;
			SaveLocalDiskStorageData(localDiskStorageData);
			
		}
        

        public async UniTask UnregisterAsync()
        {
			await UniTask.Delay(DelaySimulatedPerMethod);

			ClearTheGameLocalDiskStorage();
			
			await SetGoldAsync(0);
			
			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.IsRegistered = false;
			SaveLocalDiskStorageData(localDiskStorageData);

		}

        public UniTask TransferGoldAsync()
        {
	        throw new NotImplementedException();
        }

        public UniTask TransferPrizeAsync()
        {
	        throw new NotImplementedException();
        }


        public async UniTask SetGoldAsync(int targetBalance)
        {
	        await UniTask.Delay(DelaySimulatedPerMethod);
	        
	        LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
	        localDiskStorageData.Gold = targetBalance;
	        SaveLocalDiskStorageData(localDiskStorageData);
        }

        
        public async UniTask SetGoldByAsync(int deltaBalance)
        {
	        await UniTask.Delay(DelaySimulatedPerMethod);
	        
	        LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
	        localDiskStorageData.Gold = localDiskStorageData.Gold + deltaBalance;
	        SaveLocalDiskStorageData(localDiskStorageData);
        }


        public async UniTask AddTreasurePrizeAsync(Prize prizeToAdd)
        {
	        await UniTask.Delay(DelaySimulatedPerMethod);
	        
	        LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

	        int index = localDiskStorageData.TreasurePrizeDtos.FindIndex((next) =>
	        {
		        return next.Title == prizeToAdd.Title &&
		               next.Price == prizeToAdd.Price;
	        });

	        if (index == -1)
	        {
		        localDiskStorageData.TreasurePrizeDtos.Add(prizeToAdd);
		        SaveLocalDiskStorageData(localDiskStorageData);
	        }
	        else
	        {
		        Custom.Debug.LogError($"AddTreasurePrizeAsync() failed. Can't add already-existing prize");
	        }
        }

        
        public async UniTask SellTreasurePrizeAsync(Prize prizeToDelete)
        {
	        await UniTask.Delay(DelaySimulatedPerMethod);

	        LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

	        int countBefore = localDiskStorageData.TreasurePrizeDtos.Count;

	        int index = localDiskStorageData.TreasurePrizeDtos.FindIndex((next) =>
	        {
		        return next.Title == prizeToDelete.Title &&
		               next.Price == prizeToDelete.Price;
	        });

	        if (index != -1)
	        {
		        localDiskStorageData.TreasurePrizeDtos.RemoveAt(index);
		        SaveLocalDiskStorageData(localDiskStorageData);
		        
		        //Give gold
		        int gold = (int)prizeToDelete.Price;
		        Custom.Debug.Log($"Paying {gold} per Treasure sold");
		        await SetGoldByAsync(gold);
	        }
	        else
	        {
		        Custom.Debug.LogError($"SellTreasurePrizeAsync() failed. Can't sell non-existing prize");
	        }
        }

        
        public async UniTask DeleteAllTreasurePrizeAsync()
        {
	        await UniTask.Delay(DelaySimulatedPerMethod);

	        LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

	        localDiskStorageData.TreasurePrizeDtos.Clear();
	        SaveLocalDiskStorageData(localDiskStorageData);
        }

        
        /// <summary>
        /// Called from the "reset all data" button.
        /// Combine several operations into 1 to smooth the user experience
        /// </summary>
        public async UniTask SafeReregisterDeleteAllPrizesAsync()
        {
	        bool isRegistered = await GetIsRegisteredAsync();
	        if (isRegistered)
	        {
		        await UnregisterAsync();
	        }
	        await RegisterAsync();
	        await DeleteAllTreasurePrizeAsync();
        }

        
        public async UniTask StartGameAndGiveRewardsAsync(int goldAmount)
        {
	        if (goldAmount <= 0)
	        {
		        throw new Exception("goldAmount must be > 0 to start the game");
	        }
	        
	        if (await GetGoldAsync() < goldAmount)
	        {
		        throw new Exception("getGold() must be >= goldAmount to start the game");
	        }
	        if (await GetIsRegisteredAsync() == false)
	        {
		        throw new Exception("Must be registered to start the game.");
	        }

	        // Deduct gold
	        await SetGoldByAsync(-goldAmount);

	        // The higher the goldAmount paid, the higher the POTENTIAL Prize Price Value
	        uint random = (uint)UnityEngine.Random.Range(0, 100 + goldAmount);
	        uint price = random;
	        uint theType = 0;
	        string title = "";

	        if (random < 50)
	        {
		        // REWARD: Gold!
		        theType = TheGameHelper.GetRewardType(TheGameHelper.RewardGold);
		        title = TheGameHelper.CreateNewRewardTitle(TheGameHelper.RewardGold);
		        await SetGoldByAsync((int)price);
	        } 
	        else 
	        {
		        // REWARD: Prize!
		        theType = TheGameHelper.GetRewardType(TheGameHelper.RewardPrize);
		        title = TheGameHelper.CreateNewRewardTitle(TheGameHelper.RewardPrize);

		        //NOTE: Metadata structure must match in both: TheGameContract.sol and TreasurePrizeDto.cs
		        string moralisUserEthAddress = await MyMoralisWrapper.Instance.GetMoralisUserEthAddressAsync();
		        
		        // RELATES ONLY TO NFT
		        TreasurePrizeMetadata treasurePrizeMetadata = new TreasurePrizeMetadata
		        {
			        Title = title,
			        Price = price
		        };
		        string metadata = TheGameHelper.ConvertMetadataObjectToString(treasurePrizeMetadata);
		        Prize prize = new Prize(moralisUserEthAddress, metadata);
		        
		        await AddTreasurePrizeAsync(prize);
	        }
	        
	        // RELATES TO NFT OR GOLD
	        _lastTransferLog = new TransferLog
	        {
		        Title = title,
		        Type = theType,
		        Price = price
	        };
        }

        
        // Event Handlers ---------------------------------
    }
}
