using System;
using System.Linq;
using RMC.Shared.Data.Types;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{
    /// <summary>
    /// Determines the user-input state of the game
    /// </summary>
    public enum FullMultiplayerState
    {
        Null,
        Authenticating,
        Authenticated,
        LobbyConnecting,
        LobbyConnected,
    }
}