using System;
using System.Linq;
using RMC.Shared.Data.Types;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{

    /// <summary>
    /// Observable wrapper invoking events for <see cref="FullMultiplayerState"/>
    /// </summary>
    [Serializable]
    public class ObservableFullMultiplayerState : Observable<FullMultiplayerState>
    {
        //  Properties ------------------------------------

		
        //  Fields ----------------------------------------

		
        //  General Methods -------------------------------
        /// <summary>
        /// Overly simplified, but doable, control system for the states.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected override FullMultiplayerState OnValueChanging(
	        FullMultiplayerState oldValue, FullMultiplayerState newValue)
        {
	        bool isAllowedTransition = true;
	        try
	        {
		        AllowStateTransition(oldValue, newValue,
			        FullMultiplayerState.Null,
			        new FullMultiplayerState[] {FullMultiplayerState.Initialized});
		        
		        AllowStateTransition(oldValue, newValue,
			        FullMultiplayerState.Initialized,
			        new FullMultiplayerState[] {FullMultiplayerState.Authenticating});

		        
		        AllowStateTransition(oldValue, newValue,
			        FullMultiplayerState.Authenticating,
			        new FullMultiplayerState[] {FullMultiplayerState.Authenticated});
		        
		        AllowStateTransition(oldValue, newValue,
			        FullMultiplayerState.Authenticated,
			        new FullMultiplayerState[] {FullMultiplayerState.LobbyConnecting});
		        
		        AllowStateTransition(oldValue, newValue,
			        FullMultiplayerState.LobbyConnecting,
			        new FullMultiplayerState[] {FullMultiplayerState.LobbyConnected});
	        }
	        catch (Exception e)
	        {
		        Debug.LogWarning(e.Message);
		        isAllowedTransition = false;
	        }

	        if (isAllowedTransition)
	        {
		        return base.OnValueChanging(oldValue, newValue);
	        }
	        else
	        {
		        return base.OnValueChanging(oldValue, oldValue);
	        }
        }

        private void AllowStateTransition(FullMultiplayerState oldValue, FullMultiplayerState newValue,
	        FullMultiplayerState ifOldState, FullMultiplayerState[] thenAllowNewStates)
        {
	        if (oldValue == ifOldState && 
	            !thenAllowNewStates.Contains(newValue))
	        {
		        throw new Exception($"OnValueChanging() failed for {oldValue} -> {newValue}");
	        }
        }


        //  Event Handlers --------------------------------
    }
	
}