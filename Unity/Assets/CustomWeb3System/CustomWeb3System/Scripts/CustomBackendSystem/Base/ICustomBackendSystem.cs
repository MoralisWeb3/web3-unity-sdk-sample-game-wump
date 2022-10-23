using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Interfaces;
using MoralisUnity.Samples.SharedCustom.Interfaces;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Implemented by <see cref="PlayFabBackendSystem"/>
    /// </summary>
    public interface ICustomBackendSystem : IInitializable
    {
        //  Properties ------------------------------------
        bool IsAuthenticated { get; }
        
        //  Methods ---------------------------------------
        UniTask ClearActiveSessionAsync();
        
        //  Async Methods ---------------------------------------
        UniTask AuthenticateAsync();


    }
}