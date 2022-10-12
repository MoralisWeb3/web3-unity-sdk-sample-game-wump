using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using RMC.Shared.Managers;
using MoralisUnity.Samples.TheGame.MVCS.View.Scenes;
using UnityEngine;

#pragma warning disable 0472, CS4014, CS1998
namespace MoralisUnity.Samples.TheGame
{
	/// <summary>
	/// Core Scene Behavior - Using <see cref="Scene05_GameUI"/>
	/// </summary>
	public class Scene05_Game : MonoBehaviour
	{
		//  Events ----------------------------------------
		
		//  Properties ------------------------------------


		//  Fields ----------------------------------------
		[Header("References (Scene)")]
		[SerializeField]
		private Scene05_GameUI _ui;
	
		private static Scene05_Game _Instance;
		private bool _isInitialized = false;


		//  Unity Methods ---------------------------------
		protected void Awake()
		{
			_Instance = this;
		}

		
		protected async void Start()
		{
			_ui.BackButton.Button.onClick.AddListener(BackButton_OnClicked);
	
			Initialize();
			
			RefreshUIAsync();
			bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
			if (isAuthenticated)
			{ 
				// Populate the top UI
				bool isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
				RefreshUIAsync();
			}

			if (TheGameConfiguration.Instance.MultiplayerIsAutoStart && 
			    !TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
			{
				TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnect();
			}
		}

		protected async void OnDestroy()
		{
			if (!TheGameSingleton.IsShuttingDown)
			{
				if (TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
				{
					TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceDisconnectAsync();
				}
			}
		}


		private void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}
			_isInitialized = true;
			
			if (!_isInitialized)
			{
				Debug.LogWarning($"{GetType().Namespace}.Initialize() failed. ");
			}
		}


		//  Methods ---------------------------------------

		private async UniTask RefreshUIAsync()
		{
			_ui.BackButton.IsInteractable = true;
		}

		
		//  Event Handlers --------------------------------
		private void BackButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}

	}
}
