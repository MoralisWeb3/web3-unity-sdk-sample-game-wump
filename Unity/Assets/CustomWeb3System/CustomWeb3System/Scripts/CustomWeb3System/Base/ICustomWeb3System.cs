using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="MoralisTwoWeb3System"/>
    /// </summary>
    public interface ICustomWeb3System 
    {
        //  Properties ------------------------------------
        ICustomWeb3WalletSystem CustomWeb3WalletSystem { set; get; }
        ICustomBackendSystem CustomBackendSystem { set; get; }
        bool HasActiveSession { get; }
        int ChainId { get; }

        //  Methods ---------------------------------------
        string ConvertWeb3AddressToShortFormat(string address);

        
        //  Async Methods ---------------------------------------
        UniTask AuthenticateAsync();
        UniTask<bool> IsAuthenticatedAsync();
        Task<string> GetWeb3UserAddressAsync();
        UniTask<String> ExecuteContractFunctionAsync(string contractAddress, string abi, string functionName, object[] args, bool isLogging = false);
        UniTask<object> RunContractFunctionAsync(string contractAddress, string functionName, string abi, object args, bool isLogging = false);
        UniTask<List<NftOwner>> GetNFTsForContractAsync(string contractAddress, bool isLogging = false);
        UniTask ClearActiveSessionAsync();
        UniTask CloseActiveSessionAsync();
    
    }
}