using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.TheGame.MVCS.View;
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
			TheGameSingleton.Instance.TheGameController.OnPlayerAction.AddListener(OnPlayerAction);
			TheGameSingleton.Instance.TheGameController.OnSharedStatusChanged.AddListener(OnSharedStatusChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChanged.AddListener(OnTheGameModelChanged);
			TheGameSingleton.Instance.TheGameController.OnTheGameModelChangedRefresh();
	
			Initialize();
			
			RefreshUIAsync();
			bool isAuthenticated = await TheGameSingleton.Instance.TheGameController.GetIsAuthenticatedAsync();
			bool isRegistered = false;
			if (isAuthenticated)
			{ 
				// Populate the top UI
				isRegistered = await TheGameSingleton.Instance.TheGameController.GetIsRegisteredAndUpdateModelAsync();
				RefreshUIAsync();
			}

			if (isAuthenticated && isRegistered)
			{
				if (TheGameConfiguration.Instance.MultiplayerIsAutoStart && 
				    !TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceIsConnected())
				{
					TheGameSingleton.Instance.TheGameController.MultiplayerSetupServiceConnect();
				}
			}
			else
			{
				await TheGameSingleton.Instance.TheGameController.ShowMessageWithDelayAsync(
					TheGameConstants.MustBeRegistered, 5000);
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
		
		private void OnTheGameModelChanged(TheGameModel theGameModel)
		{
		
			
		}
		
		private void OnPlayerAction(PlayerView playerView)
		{
		}
		
		private void OnSharedStatusChanged(PlayerView playerView)
		{
			if (!string.IsNullOrEmpty(playerView.SharedStatus))
			{
				_ui.TopUI.QueueSharedStatusText(playerView.SharedStatus, 3000);
			}
		}
		
		//  Event Handlers --------------------------------
		private void BackButton_OnClicked()
		{
			TheGameSingleton.Instance.TheGameController.LoadIntroSceneAsync();
		}

	}
}
