using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Shared.Attributes;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using UnityEngine;

#pragma warning disable CS0162
namespace MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService
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
		public bool IsRegistered
		{
			set
			{
				_isRegistered = value;
			}
			get
			{
				return _isRegistered;
			}
		}

		//  Fields ----------------------------------------
		[SerializeField]
		private bool _isRegistered = false;
		public int Gold = 0;
		public List<Prize> Prizes = new List<Prize>();
		public TransferLog TransferLog;
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
		public async UniTask<TransferLog> GetTransferLogHistoryAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);
			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			return localDiskStorageData.TransferLog;
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
			return localDiskStorageData.Prizes;
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

			await SetGoldAsync((int)TheGameConstants.GoldOnRegister);

			List<Prize> prizes = new List<Prize>();
			for (int i = 0; i < TheGameConstants.PrizesOnRegister; i++)
			{
				prizes.Add(new Prize());
			}
			await SetPrizesAsync(prizes);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.IsRegistered = true;
			SaveLocalDiskStorageData(localDiskStorageData);

		}


		public async UniTask UnregisterAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			ClearTheGameLocalDiskStorage();
			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.IsRegistered = false;
			SaveLocalDiskStorageData(localDiskStorageData);

		}

		public async UniTask TransferGoldAsync(string address)
		{
			int gold = await GetGoldAsync();
			int newGold = gold - (int)TheGameConstants.GoldOnTransfer;
			if (newGold < 0)
			{
				Debug.LogWarning("TransferGoldAsync() failed. not enough balance");
				return;
			}
			Debug.LogWarning("TransferGoldAsync() PARTIAL functionality. This REMOVES, but doesn't transfer. That is ok for testing.");
			await SetGoldAsync(newGold);
		}

		public async UniTask TransferPrizeAsync(string address, Prize prize)
		{
			List<Prize> prizes = await GetPrizesAsync();

			if (prizes.Count < TheGameConstants.PrizesOnTransfer)
			{
				Debug.LogWarning("TransferPrizeAsync() failed. not enough balance");
				return;
			}

			// Do a quick and dirty 'remove ONE' 
			if (TheGameConstants.PrizesOnTransfer != 1)
			{
				Debug.LogError("Change implementation, per new value");
				return;
			}

			prizes.RemoveAt(0);
			Debug.LogWarning("TransferPrizeAsync() PARTIAL functionality. This REMOVES, but doesn't transfer. That is ok for testing.");
			await SetPrizesAsync(prizes);
		}

		private async UniTask SetGoldAsync(int targetBalance)
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.Gold = targetBalance;
			SaveLocalDiskStorageData(localDiskStorageData);
		}

		private async UniTask SetPrizesAsync(List<Prize> prizes)
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();
			localDiskStorageData.Prizes = prizes;
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
			await DeleteAllTreasurePrizeAsync();
			await RegisterAsync();
			
		}

		private async UniTask DeleteAllTreasurePrizeAsync()
		{
			await UniTask.Delay(DelaySimulatedPerMethod);

			LocalDiskStorageData localDiskStorageData = LoadLocalDiskStorageData();

			localDiskStorageData.Prizes.Clear();
			SaveLocalDiskStorageData(localDiskStorageData);
		}

		// Event Handlers ---------------------------------
	}
}