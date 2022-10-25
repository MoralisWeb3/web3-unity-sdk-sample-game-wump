using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.SharedCustom.Interfaces;
using PlayFab.CloudScriptModels;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="PlayFabBackendSystem"/>
    /// </summary>
    public interface ICustomBackendSystem : IInitializable, IWeb3Calls
    {
        //  Properties ------------------------------------
        bool IsAuthenticated { get; } //TODO: be async?
        
        //  Methods ---------------------------------------
        
        //  Async Methods ---------------------------------------
        UniTask AuthenticateAsync();
        
        //  Async PlayFab Methods ---------------------------------------
        UniTask<ExecuteFunctionResult> ChallengeRequestAsync(string web3UserAddress, int chainId);
        UniTask<ExecuteFunctionResult> ChallengeVerifyAsync(string message, string signature);


    }
}