using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Interfaces;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class MyMoralisWrapper : CustomSingleton<MyMoralisWrapper>, IInitializable, ICustomSingletonParent
	{
		// Properties -------------------------------------
		
		
		// Fields -----------------------------------------
		public bool IsInitialized { get; private set; }
		
		// Initialization Methods -------------------------
		void ICustomSingletonParent.OnInstantiatedChild()
		{
			//TODO: Move screen sizing to some OTHER centralized code placement
			//Set screen size for Standalone
#if UNITY_STANDALONE
			Screen.SetResolution(675, 1000, false);
			Screen.fullScreen = false;
#endif

			// Auto-Initialize
			Initialize();

		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				return;
			}
			
			// HasMoralisUserAsync may be called early. So ensure this is called
			Moralis.Start();
			IsInitialized = true;
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
		
		
		// General Methods --------------------------------
		public async UniTask<bool> HasMoralisUserAsync()
		{
			RequireIsInitialized();
			
			// Determines if Moralis is logged in with an active user.
			MoralisUser moralisUser = await GetMoralisUserAsync();
			return moralisUser != null;
		}
		
		
		public async UniTask<MoralisUser> GetMoralisUserAsync()
		{
			RequireIsInitialized();
			return await Moralis.GetUserAsync();
		}
		
		// Event Handlers ---------------------------------
	
	}
}