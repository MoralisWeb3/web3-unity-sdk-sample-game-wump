using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.UnityWeb3Tools.Models;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="MoralisTwoWeb3System"/>
    /// </summary>
    public interface IWeb3Calls 
    {
        //  Properties ------------------------------------

        //  Methods ---------------------------------------
        
        //  Async Methods ---------------------------------------
        UniTask<String> ExecuteContractFunctionAsync(string contractAddress, string abi, string functionName, object[] args, bool isLogging = false);
        UniTask<object> RunContractFunctionAsync(string contractAddress, string functionName, string abi, object args, bool isLogging = false);
        UniTask<List<NftOwner>> GetNFTsForContractAsync(string contractAddress, bool isLogging = false);
    
    }
}