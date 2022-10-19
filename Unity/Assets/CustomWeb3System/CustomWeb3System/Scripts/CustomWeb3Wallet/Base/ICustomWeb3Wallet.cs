
using Cysharp.Threading.Tasks;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="WalletConnectWeb3WalletSystem"/>
    /// </summary>
    public interface ICustomWeb3WalletSystem 
    {
        //  Properties ------------------------------------
        bool HasActiveSession { get; }
        bool IsConnected { get; }
        int ChainId { get; }

        //  Methods ---------------------------------------
        UniTask ClearActiveSession();
        
        //  Async Methods ---------------------------------------
        UniTask<string> GetWeb3UserAddressAsync();
        UniTask<bool> HasWeb3UserAddressAsync();
        UniTask ConnectAsync();
    }
}