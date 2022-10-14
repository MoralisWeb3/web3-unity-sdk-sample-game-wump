
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using UnityEngine.Events;

namespace MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService
{
    /// <summary>
    /// Replace with comments...
    /// </summary>
    public interface IMultiplayerSetupService : IInitializable
    {
        //  Properties ------------------------------------
        UnityEvent OnConnectStarted { get; }
        StringUnityEvent OnConnectCompleted { get; }
        UnityEvent OnDisconnectStarted { get; }
        UnityEvent OnDisconnectCompleted { get; }
        StringUnityEvent OnStateNameForDebuggingChanged { get; }
        bool IsConnected { get; }
        
        //  Methods ---------------------------------------
        void Connect();
        void OnGUI();
        UniTask DisconnectAsync();
    }
}