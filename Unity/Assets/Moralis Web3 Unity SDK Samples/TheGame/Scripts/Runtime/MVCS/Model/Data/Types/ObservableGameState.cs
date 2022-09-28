
using System;
using MoralisUnity.Samples.Shared.Data.Types;
using UnityEngine;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types
{
	/// <summary>
	/// Determines the user-input state of the game
	/// </summary>
	public enum GameState
	{
		Null,
		TreasureChestEntering,
		TreasureChestEntered,
		TreasureChestIdle,
		WaitForUser,
		TreasureChestPreOpening,
		TreasureChestOpening,
		TreasureChestOpened,
		CardsEntering,
		CardsEntered,
		CardsIdle
		
	}
	
	/// <summary>
	/// Observable wrapper invoking events for <see cref="GameState"/>
	/// </summary>
	[Serializable]
	public class ObservableGameState : Observable<GameState>
	{
		//  Properties ------------------------------------

		
		//  Fields ----------------------------------------

		
		//  General Methods -------------------------------

		
		//  Event Handlers --------------------------------
	}
	
}
