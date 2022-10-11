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
	[CustomFilePath(LocalDiskStorage.Title + "/TheGameLocalDiskStorage.txt",
		CustomFilePathLocation.StreamingAssetsPath)]
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
		public PendingMessage PendingMessageActive
		{
			get { return _pendingMessageActive; }
		}

		public PendingMessage PendingMessagePassive
		{
			get { return _pendingMessagePassive; }
		}

		public PendingMessage PendingMessageExtraDelay
		{
			get { return _pendingMessageExtraDelay; }
		}

		public bool HasExtraDelay
		{
			get { return false; }
		}

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

		public UniTask TransferPrizeAsync(Prize prize)
		{
			throw new NotImplementedException();
		}

		private async UniTask SetGoldAsync(int targetBalance)
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.Gold = targetBalance;
			SaveLocalDiskStorageData(localDiskStorageData);
		}


		private async UniTask SetGoldByAsync(int deltaBalance)
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.Gold = localDiskStorageData.Gold + deltaBalance;
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

		public async UniTask DeleteAllTreasurePrizeAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

			localDiskStorageData.TreasurePrizeDtos.Clear();
			SaveLocalDiskStorageData(localDiskStorageData);
		}

		// Event Handlers ---------------------------------
	}
}