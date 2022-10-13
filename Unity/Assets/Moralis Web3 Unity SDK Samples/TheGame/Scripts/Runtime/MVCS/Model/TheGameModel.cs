using System;
using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.View;

namespace MoralisUnity.Samples.TheGame.MVCS.Model
{
	//TODO: move this CustomPlayerInfo class. rename it?

	/// <summary>
	/// Stores data for the game
	///		* See <see cref="TheGameSingleton"/>
	/// </summary>
	public class TheGameModel 
	{
		//TODO: make any onchange automatically invoke the custom change, then never call 'refresh'? Optional, but better for design
		
		
		// Properties -------------------------------------
		public TheGameConfiguration TheGameConfiguration { get { return TheGameConfiguration.Instance; }  }
		public Observable<int> Gold { get { return _gold; } }
		public Observable<bool> IsRegistered { get { return _isRegistered; } }
		public Observable<CustomPlayerInfo> CustomPlayerInfo { get { return _customPlayerInfo; } }
		public Observable<List<Prize>> Prizes { get { return _prizes; } }
		public Observable<PlayerView> SelectedPlayerView { get { return _selectedPlayerView; } }
		public bool HasSelectedPlayerView { get { return _selectedPlayerView.Value != null; } }

		// Fields -----------------------------------------
		private Observable<int> _gold = new Observable<int>();
		private ObservablePrizes _prizes = new ObservablePrizes();
		private Observable<CustomPlayerInfo> _customPlayerInfo = new Observable<CustomPlayerInfo>();
		private Observable<bool> _isRegistered = new Observable<bool>();
		private Observable<PlayerView> _selectedPlayerView = new Observable<PlayerView>();
		
		// Initialization Methods -------------------------
		public TheGameModel()
		{
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
			_customPlayerInfo.Value = new CustomPlayerInfo();
			_prizes.Value = new List<Prize>();
			_isRegistered.Value = false;
			_selectedPlayerView.Value = null;
		}
		
		// Event Handlers ---------------------------------
	}
}
