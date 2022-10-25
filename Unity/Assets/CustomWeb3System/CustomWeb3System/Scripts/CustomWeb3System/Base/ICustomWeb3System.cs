using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;
using PlayFab.CloudScriptModels;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="MoralisTwoWeb3System"/>
    /// </summary>
    public interface ICustomWeb3System : IWeb3Calls
    {
        //  Properties ------------------------------------
        ICustomWeb3WalletSystem CustomWeb3WalletSystem { set; get; }
        ICustomBackendSystem CustomBackendSystem { set; get; }
        int ChainId { get; }

        //  Methods ---------------------------------------
        string ConvertWeb3AddressToShortFormat(string address);

        //  Async Methods ---------------------------------------
        UniTask AuthenticateAsync();
        UniTask<bool> IsAuthenticatedAsync();
        UniTask<string> GetWeb3UserAddressAsync();
        UniTask<bool> HasWeb3UserAddressAsync();
        UniTask<string> EthPersonalSignAsync(string web3UserAddress, string message);
        UniTask KLUGE_CloseOpenWalletConnection();
        
        //  Async PlayFab Methods ---------------------------------------
        UniTask<ExecuteFunctionResult> ChallengeRequestAsync(string web3UserAddress, int chainId);
        UniTask<ExecuteFunctionResult> ChallengeVerifyAsync(string message, string signature);
        
    }
}