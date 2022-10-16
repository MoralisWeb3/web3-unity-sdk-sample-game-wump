using System;
using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.View;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.Model
{
	//TODO: move this CustomPlayerInfo class. rename it?

	/// <summary>
	/// Stores data for the game
	///		* See <see cref="TheGameSingleton"/>
	/// </summary>
	public class TheGameModel 
	{
		[HideInInspector]
		public readonly TheGameModelUnityEvent OnTheGameModelChanged = new TheGameModelUnityEvent();
		
		// Properties -------------------------------------
		//
		
		public Observable<int> Gold { get { return _gold; } }
		public Observable<bool> IsRegistered { get { return _isRegistered; } }
		public Observable<bool> IsAuthenticated { get { return _isAuthenticated; } }
		public Observable<CustomPlayerInfo> CustomPlayerInfo { get { return _customPlayerInfo; } }
		public Observable<List<Prize>> Prizes { get { return _prizes; } }
		public Observable<PlayerView> SelectedPlayerView { get { return _selectedPlayerView; } }
		public bool HasSelectedPlayerView { get { return _selectedPlayerView.Value != null; } }
		public Observable<bool> IsTransferPending { get { return _isTransferPending; } }

		// Fields -----------------------------------------
		private Observable<bool> _isRegistered = new Observable<bool>();
		private Observable<bool> _isAuthenticated = new Observable<bool>();
		private Observable<int> _gold = new Observable<int>();
		private ObservablePrizes _prizes = new ObservablePrizes();
		private Observable<CustomPlayerInfo> _customPlayerInfo = new Observable<CustomPlayerInfo>();
		private Observable<PlayerView> _selectedPlayerView = new Observable<PlayerView>();
		private Observable<bool> _isTransferPending = new Observable<bool>();
		
		// Initialization Methods -------------------------
		public TheGameModel()
		{
			// Change in ANY subpart will dispatch 'everything changed'. Good.
			_isRegistered.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_isAuthenticated.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_gold.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_prizes.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_customPlayerInfo.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_selectedPlayerView.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			_isTransferPending.OnValueChanged.AddListener((i) => { OnValueChangedForAnything();});
			ResetAllData();
		}

		
		// General Methods --------------------------------
		public bool HasAnyData()
		{
			// TODO: Put real check here
			return false;
		}
		
		
		public void ResetAllData()
		{
			_gold.Value = 0;
			_prizes.Value = new List<Prize>();
			_customPlayerInfo.Value = new CustomPlayerInfo();
			_isRegistered.Value = false;
			_isAuthenticated.Value = false;
			_selectedPlayerView.Value = null;

		}
		
		// Event Handlers ---------------------------------
		private void OnValueChangedForAnything()
		{
			OnTheGameModelChanged.Invoke(this);
		}
	}
}
