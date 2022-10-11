
using System.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using UnityEngine.Events;

namespace MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService
{
    /// <summary>
    /// Replace with comments...
    /// </summary>
    public interface IMultiplayerSetupService
    {
        //  Properties ------------------------------------
        StringUnityEvent OnConnectionCompleted { get; }
        StringUnityEvent OnStateNameChanged { get; }
        bool IsConnected { get; }
        
        //  Methods ---------------------------------------
        void Connect();
        void OnGUI();
        Task DisconnectAsync();
    }
}