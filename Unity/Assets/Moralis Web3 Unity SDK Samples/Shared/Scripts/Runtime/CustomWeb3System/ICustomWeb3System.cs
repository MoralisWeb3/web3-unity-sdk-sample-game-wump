using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace MoralisUnity.Samples.Shared
{
    public interface ICustomWeb3System 
    {
        bool HasWalletConnectStaticInstance { get; }
        void EnsureDestroyedWalletConnectInstance();
        void EnsureInstantiatedWalletConnectInstance();
        UniTask<bool> IsAuthenticatedAsync();
        Task<string> GetWeb3UserAddressAsync();
        string ConvertWeb3AddressToShortFormat(string address);
        UniTask<String> ExecuteContractFunction(string _address, string _abi,
            string functionName, object[] args, bool isLogging);

        UniTask<T> RunContractFunction<T>(string address, string functionName,
            object[] abiObject, Dictionary<string, object> args, bool isLogging);

        UniTask<CustomNftOwnerCollection>
            GetNFTsForContract(string ethAddress, string contractAddress);
    }
}