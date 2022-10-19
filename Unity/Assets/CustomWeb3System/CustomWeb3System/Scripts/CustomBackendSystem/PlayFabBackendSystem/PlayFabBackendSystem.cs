using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.SharedCustom.DesignPatterns.Creational.Singleton.CustomSingleton;
using MoralisUnity.Samples.SharedCustom.Exceptions;
using MoralisUnity.Samples.SharedCustom.Interfaces;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class PlayFabBackendSystem : CustomSingleton<PlayFabBackendSystem>, 
		IInitializable, ICustomSingletonParent, ICustomBackendSystem
	{
		// Properties -------------------------------------
		public bool IsInitialized { get; private set; }

		// Fields -----------------------------------------
		public bool IsAuthenticated { get; private set; }
		public bool HasAuthenticationError { get; private set; }
		

		// Initialization Methods -------------------------
		void ICustomSingletonParent.OnInstantiatedChild()
		{
			// Auto-Initialize
			Initialize();
		}

		public async void Initialize()
		{
			if (!IsInitialized)
			{
				PlayFabAuthService.OnLoginSuccess += PlayFabAuthService_OnLoginSuccess;
				PlayFabAuthService.OnPlayFabError += PlayFabAuthService_OnPlayFabError;
				PlayFabAuthService.Instance.RememberMe = true;
				IsInitialized = true;
			}
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
		
		public void RequireIsAuthenticated()
		{
			if (!IsAuthenticated)
			{
				throw new NotAuthenticatedException(this);
			}
		}
		
		// General Methods --------------------------------

		public UniTask ClearActiveSession()
		{
			PlayFabAuthService.Instance.ClearRememberMe();
			return UniTask.DelayFrame(0);
		}

		public UniTask AuthenticateAsync()
		{
			RequireIsInitialized();
			
			if (!IsAuthenticated)
			{
				PlayFabAuthService.Instance.Authenticate(Authtypes.Silent); //Play as guest. Adequate
			}

			return UniTask.WaitWhile(() => !IsAuthenticated && !HasAuthenticationError);
		}
		
		// Event Handlers ---------------------------------

		
		private void PlayFabAuthService_OnLoginSuccess(LoginResult loginResult)
		{
			Debug.Log($"PlayFabAuthService_OnLoginSuccess () loginResult = {loginResult}");
			RequireIsInitialized();
			IsAuthenticated = true;
			RequireIsAuthenticated();
		}
		
		private void PlayFabAuthService_OnPlayFabError(PlayFabError playFabError)
		{
			RequireIsInitialized();
			Debug.LogError($"PlayFabAuthService_OnPlayFabError () playFabError = {playFabError}");
			HasAuthenticationError = true;
			IsAuthenticated = false;
		}

	}
}

