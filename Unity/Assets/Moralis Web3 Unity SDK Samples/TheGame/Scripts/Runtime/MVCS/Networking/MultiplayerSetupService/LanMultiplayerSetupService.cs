using System;
using System.Threading.Tasks;
using RMC.Shared;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

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
		private  StringUnityEvent _onConnectionCompleted = new StringUnityEvent();
		private  StringUnityEvent _onStateNameChanged = new StringUnityEvent();
		
		//  Properties ------------------------------------
		public bool IsConnected { get; private set; }
		public StringUnityEvent OnConnectionCompleted { get { return _onConnectionCompleted; } }
		public StringUnityEvent OnStateNameChanged { get { return _onStateNameChanged; } }
		
		//  Fields ----------------------------------------
		private UnityTransport _unityTransport;
		private ulong _localClientId;
		private bool _isAutoStart = false;

		
		//  Initializer Methods ---------------------------------
		public LanMultiplayerSetupService(UnityTransport unityTransport, 
			bool isAutoStart, 
			UnityTransport.SimulatorParameters simulatorParameters)
		{
			_unityTransport = unityTransport;
			_unityTransport.DebugSimulator = simulatorParameters;
			_isAutoStart = isAutoStart;
			IsConnected = false;
			NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
		}


		//  Methods ---------------------------------------
		public void Connect()
		{
			// The instance playing in the Primary UNITY EDITOR will host
			// All others will NOT host
			if (_isAutoStart)
			{
				if (ClonesManagerWrapper.HasClonesManager)
				{
					if (ClonesManagerWrapper.IsClone)
					{
						StartClient();
					}
					else
					{
						// Primary UNITY EDITOR
						StartHost();
					}
				}
				else
				{
					StartClient();
				}
			}
		}

		
		public void OnGUI() 
		{
			//Regardless of _isAutoStart, this will appear properly as needed

			const int guiWidth = 300;
			const int guiMargin = 20;
			GUILayout.BeginArea(new Rect(Screen.width - guiWidth - guiMargin, guiMargin, guiWidth, guiWidth));
			if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) 
			{
				if (GUILayout.Button("Host"))
				{
					StartHost();
					OnStateNameChanged.Invoke("StartHost");
				}

				// if (GUILayout.Button("Server"))
				// {
				// 	NetworkManager.Singleton.StartServer();
				// 	OnStateNameChanged.Invoke("StartServer");
				// }

				if (GUILayout.Button("Client"))
				{
					StartClient();
					OnStateNameChanged.Invoke("StartClient");
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
			OnStateNameChanged.Invoke("StartHost");
			NetworkManager.Singleton.StartHost();
		}
		
		
		private void StartClient()
		{
			OnStateNameChanged.Invoke("StartClient");
			NetworkManager.Singleton.StartClient();
		}
		
		
		private void Shutdown(string label)
		{
			OnStateNameChanged.Invoke("Shutdown");
			NetworkManager.Singleton.Shutdown();
		}


		public Task DisconnectAsync()
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
			return Task.CompletedTask;
		}
		

		//  Event Handlers --------------------------------
		private void NetworkManager_OnClientConnected(ulong connectedClientId)
		{
			OnStateNameChanged.Invoke("Connected");
			
			// For each client, store the 'i am now connected'
			if (!NetworkManager.Singleton.IsClient) return;

			// For host, it is both a server and a client
			// so check that the connection is 'me'
			if (NetworkManager.Singleton.LocalClientId == connectedClientId)
			{
				IsConnected = true;
				_localClientId = connectedClientId;

				string debugMessage = $"Connected as LocalClientId = {_localClientId}";
				OnConnectionCompleted.Invoke(debugMessage);
				//Debug.Log($"{this.GetType().Name} NetworkManager_OnClientConnected() from l={_localClientId} nsl={NetworkManager.Singleton.LocalClientId} leaveID{connectedClientId}"); 
			}
		}
		
	}
}
