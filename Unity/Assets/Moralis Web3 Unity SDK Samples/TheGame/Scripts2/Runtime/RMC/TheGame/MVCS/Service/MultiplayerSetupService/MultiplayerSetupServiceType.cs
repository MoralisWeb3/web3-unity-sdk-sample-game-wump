
using System.Threading.Tasks;
using TheGame.MVCS.Controller.Events;
using UnityEngine.Events;

namespace RMC.TheGame.MVCS.Service.MultiplayerSetupService
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