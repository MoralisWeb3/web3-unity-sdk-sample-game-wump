using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Exceptions;
using RMC.Shared;
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
		public void Connect()
		{
			RequireIsInitialized();
			if (IsConnected)
			{
				return;
			}
			
			// The instance playing in the Primary UNITY EDITOR will host
			// All others will NOT host
			if (ClonesManagerWrapper.HasClonesManager)
			{
				if (ClonesManagerWrapper.IsClone)
				{
					_onConnectStarted.Invoke();
					StartClient();
				}
				else
				{
					// Primary UNITY EDITOR
					_onConnectStarted.Invoke();
					StartHost();
				}
			}
			else
			{
				_onConnectStarted.Invoke();
				StartClient();
			}
		}

		
		public void OnGUI() 
		{
			if (!IsInitialized) return;
			//Regardless of _isAutoStart, this will appear properly as needed

			float guiWidth = Screen.width * 0.2f;
			float guiHeight = Screen.height * 0.2f;
			float guiMarginWidth = 10;
			float guiMarginHeight = 320;
			GUILayout.BeginArea(new Rect(
				Screen.width - guiWidth - guiMarginWidth, 
				Screen.height - guiHeight - guiMarginHeight, 
				guiWidth, 
				guiHeight));
			if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) 
			{
				if (GUILayout.Button("Host"))
				{
					StartHost();
					OnStateNameForDebuggingChanged.Invoke("StartHost");
				}

				// if (GUILayout.Button("Server"))
				// {
				// 	NetworkManager.Singleton.StartServer();
				// 	OnStateNameChanged.Invoke("StartServer");
				// }

				if (GUILayout.Button("Client"))
				{
					StartClient();
					OnStateNameForDebuggingChanged.Invoke("StartClient");
				}
			}
			else
			{
				string label = "Disconnect ";
				if (NetworkManager.Singleton.IsHost)
				{
					label += "Host";
				}
				else
				{
					label += "Client";
				}

				if (GUILayout.Button(label))
				{
					Shutdown(label);
				}
			}
			GUILayout.EndArea();
		}

		
		private void StartHost()
		{
			OnStateNameForDebuggingChanged.Invoke("StartHost");
			NetworkManager.Singleton.StartHost();
		}
		
		
		private void StartClient()
		{
			OnStateNameForDebuggingChanged.Invoke("StartClient");
			NetworkManager.Singleton.StartClient();
		}
		
		
		private void Shutdown(string label)
		{
			OnStateNameForDebuggingChanged.Invoke("Shutdown");
			NetworkManager.Singleton.Shutdown();
		}


		public UniTask DisconnectAsync()
		{
			//When the scene is closing its 'too late' to do these things. That is ok.
			try
			{
				if (NetworkManager.Singleton.IsServer)
				{
					if (!NetworkManager.Singleton.ShutdownInProgress)
					{
						NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
						Debug.Log("DisconnectClient 2 ");
				
						if (NetworkManager.Singleton.IsServer)
						{
							Shutdown("Shutdown");
						}
					}
				}
			}
			catch (NullReferenceException)
			{
				//Do nothing. Sometimes "NetworkManager.Singleton" is null. That is ok.
			}

			IsConnected = false;

			//Return dummy value
			return UniTask.CompletedTask;
		}
		

		//  Event Handlers --------------------------------
		private void NetworkManager_OnClientConnected(ulong connectedClientId)
		{
			OnStateNameForDebuggingChanged.Invoke("Connected");
			
			// For each client, store the 'i am now connected'
			if (!NetworkManager.Singleton.IsClient) return;

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
