using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace MoralisUnity.Samples.TheGame.MVCS.Networking.MultiplayerSetupService
{
	//  Namespace Properties ------------------------------


	//  Class Attributes ----------------------------------


	/// <summary>
	/// Creates a shared space for 1+ players to join together
	/// * Uses few unity services. Quick and dirty for local play (e.g. on one machine)
	/// </summary>
	public class LanMultiplayerSetupService : IMultiplayerSetupService
	{
		
		//  Events ----------------------------------------
		private UnityEvent _onConnectStarted = new UnityEvent();
		private  StringUnityEvent _onConnectCompleted = new StringUnityEvent();
		private UnityEvent _onDisconnectStarted = new UnityEvent();
		private  UnityEvent _onDisconnectCompleted = new UnityEvent();
		private  StringUnityEvent _onStateNameForDebuggingChanged = new StringUnityEvent();
		
		//  Properties ------------------------------------
		public bool IsConnected { get; private set; }
		public bool IsHost { get { return NetworkManager.Singleton.IsHost;} }
		public bool IsClient { get { return NetworkManager.Singleton.IsClient;} }
		public bool IsServer { get { return NetworkManager.Singleton.IsServer;} }
		
		public UnityEvent OnConnectStarted { get { return _onConnectStarted; } }
		public StringUnityEvent OnConnectCompleted { get { return _onConnectCompleted; } }
		public UnityEvent OnDisconnectStarted { get { return _onDisconnectStarted; } }
		public UnityEvent OnDisconnectCompleted { get { return _onDisconnectCompleted; } }
		public StringUnityEvent OnStateNameForDebuggingChanged { get { return _onStateNameForDebuggingChanged; } }
		public bool IsInitialized { get; private set; }

		//  Fields ----------------------------------------
		private UnityTransport _unityTransport;
		private ulong _localClientId;

		
		//  Initializer Methods ---------------------------------
		public LanMultiplayerSetupService(UnityTransport unityTransport, 
			UnityTransport.SimulatorParameters simulatorParameters)
		{
			_unityTransport = unityTransport;
			_unityTransport.DebugSimulator = simulatorParameters;
			IsConnected = false;
			NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
		}

		public void Initialize()
		{
			if (!IsInitialized)
			{
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
		//  Methods ---------------------------------------
		public async UniTask Connect()
		{
			RequireIsInitialized();
			if (!IsConnected)
			{
				//NOTE: In this LAN implementation, not much happens in this method. That is ok.
				IsConnected = true;
				
				OnStateNameForDebuggingChanged.Invoke("Connected");
				await UniTask.CompletedTask;
			}
		}

		
		public bool CanStartAsHost()
		{
			return IsInitialized && !IsClient && !IsServer;
		}
		
		
		public bool CanJoinAsClient()
		{
			return IsInitialized && !IsClient && !IsServer;;
		}
		
		
		public bool CanShutdown()
		{
			return IsInitialized && IsClient || IsServer;
		}
		
		
		public bool CanToggleStatsButton()
		{
			return IsInitialized && IsClient || IsServer;
		}
		
		
		public async UniTask StartAsHost()
		{	
			RequireIsInitialized();
			if (!IsConnected)
			{
				Debug.LogWarning("StartAsHost () failed. Must be connected");
			}
			
			//Debug.LogError("Start as host!");
			OnStateNameForDebuggingChanged.Invoke("StartHostStarting");
			NetworkManager.Singleton.StartHost();
			
			await UniTask.Delay(1000);
			OnStateNameForDebuggingChanged.Invoke("StartHostCompleted");
			
		}

		
		public async UniTask JoinAsClient()
		{
			RequireIsInitialized();
			if (!IsConnected)
			{
				Debug.LogWarning("JoinAsClient () failed. Must be connected");
			}
			
			OnStateNameForDebuggingChanged.Invoke("StartClientStarting");
			NetworkManager.Singleton.StartClient();

			await UniTask.Delay(1000);
			OnStateNameForDebuggingChanged.Invoke("StartClientCompleted");
		}
		
		
		public async UniTask Shutdown()
		{
			RequireIsInitialized();
			
			NetworkManager.Singleton.Shutdown(true);
			OnStateNameForDebuggingChanged.Invoke("ShutdownStarting");
			await UniTask.Delay(1000);
			OnStateNameForDebuggingChanged.Invoke("ShutdownCompleted");
			IsConnected = false;
		}


		public async UniTask DisconnectAsync()
		{
			//When the scene is closing its 'too late' to do these things. That is ok.
			try
			{
				if (!NetworkManager.Singleton.ShutdownInProgress)
				{
					if (IsServer)
					{
						//UNITY MESSAGE: Only server can disconnect remote clients. Please use `Shutdown()` instead.
						NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
					}
					await Shutdown();
				}
			}
			catch (NullReferenceException e)
			{
				//Do nothing. Sometimes "NetworkManager.Singleton" is null. That is ok.
				Debug.LogWarning(e.Message);
			}
			catch (Exception e)
			{
				//Do nothing. Sometimes "NetworkManager.Singleton" is null. That is ok.
				Debug.LogWarning("2: " + e.Message);
			}

			IsConnected = false;
		}
		

		//  Event Handlers --------------------------------
		private void NetworkManager_OnClientConnected(ulong connectedClientId)
		{
			OnStateNameForDebuggingChanged.Invoke("Connected");
			
			// For each client, store the 'i am now connected'
			if (!IsClient) return;

			// For host, it is both a server and a client
			// so check that the connection is 'me'
			if (NetworkManager.Singleton.LocalClientId == connectedClientId)
			{
				IsConnected = true;
				_localClientId = connectedClientId;

				string debugMessage = $"Connected as LocalClientId = {_localClientId}";
				OnConnectCompleted.Invoke(debugMessage);
				//Debug.Log($"{this.GetType().Name} NetworkManager_OnClientConnected() from l={_localClientId} nsl={NetworkManager.Singleton.LocalClientId} leaveID{connectedClientId}"); 
			}
		}
	}
}
