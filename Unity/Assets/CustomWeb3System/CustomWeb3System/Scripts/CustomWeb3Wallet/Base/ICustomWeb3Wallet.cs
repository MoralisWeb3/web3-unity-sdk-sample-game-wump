
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.SharedCustom.Interfaces;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="WalletConnectWeb3WalletSystem"/>
    /// </summary>
    public interface ICustomWeb3WalletSystem : IInitializable
    {
        //  Properties ------------------------------------
        bool HasActiveSession { get; }
        bool IsConnected { get; }
        int ChainId { get; }

        //  Methods ---------------------------------------
        
        //  Async Methods ---------------------------------------
        UniTask<string> GetWeb3UserAddressAsync();
        UniTask<bool> HasWeb3UserAddressAsync();
        UniTask ConnectAsync();
        UniTask KLUGE_CloseOpenWalletConnection();
        UniTask<string> EthPersonalSignAsync(string web3UserAddress, string message);
    }
}