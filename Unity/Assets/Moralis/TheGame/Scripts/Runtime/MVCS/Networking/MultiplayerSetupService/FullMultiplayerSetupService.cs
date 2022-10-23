using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.Controller.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using SwitchDefaultException = RMC.Shared.Exceptions.SwitchDefaultException;

#pragma warning disable CS1998, CS4014
namespace MoralisUnity.Samples.TheGame.MVCS.Networking.MultiplayerSetupService
{
	//  Namespace Properties ------------------------------
	
	//  Class Attributes ----------------------------------


	/// <summary>
	/// Creates a shared space for 1+ players to join together
	/// * Uses Relay
	/// * Uses Lobby
	/// </summary>
	public class FullMultiplayerSetupService : IMultiplayerSetupService
	{
		//  Events ----------------------------------------
		public UnityEvent OnConnectStarted { get; } 
		public StringUnityEvent OnConnectCompleted { get; } 
		public UnityEvent OnDisconnectStarted { get; } 
		public UnityEvent OnDisconnectCompleted { get; } 
		public StringUnityEvent OnStateNameForDebuggingChanged { get; } 
		

		//  Properties ------------------------------------
		public bool IsInitialized { get; private set; }
		public bool IsConnected { get; private set; }
		public bool IsHost { get { return _isHost; } private set { _isHost = value;} }
		public bool IsClient { get { return _isClient; } private set { _isHost = value;} }
		public bool IsServer { get { return NetworkManager.Singleton.IsServer;} }
		
		//  Fields ----------------------------------------
		private UnityTransport _unityTransport;
		
		private ObservableFullMultiplayerState _observableFullMultiplayerState = 
			new ObservableFullMultiplayerState();
		
		private Lobby _lobby;
		private string _authenticatedPlayerId = "";
		private const string JoinCodeKey = "MyJoinCodeKey";
		private const int MaxConnections = 100;
		private CancellationTokenSource _sendHeartbeatCancellationTokenSource;
		private string _lastAllocatedRegion = "";
		private const int HeartBeatWaitTimeMilliseconds = 15000;
		private const int RateLimitWaitTimeMilliseconds = 11000;
		private bool _hasSetClientRelayData = false;
		private bool _hasSetHostRelayData = false;
		public bool _isHost = false;
		public bool _isClient = false;
		
		//  Initializer Methods ---------------------------------
		public FullMultiplayerSetupService(UnityTransport unityTransport)
		{
			OnConnectStarted = new UnityEvent();
			OnConnectCompleted = new StringUnityEvent();
			OnDisconnectStarted = new UnityEvent();
			OnDisconnectCompleted = new UnityEvent();
			OnStateNameForDebuggingChanged = new StringUnityEvent();
			NetworkManager.Singleton.OnTransportFailure += NetworkManager_OnTransportFailure;
			NetworkManager.Singleton.OnServerStarted += NetworkManager_OnServerStarted;
			
			//
			_unityTransport = unityTransport;
			_observableFullMultiplayerState.OnValueChanged.AddListener(ObservableFullMultiplayerState_OnValueChanged);
			IsConnected = false;
			_observableFullMultiplayerState.Value = FullMultiplayerState.Null;
		}



		public void Initialize()
		{
			if (!IsInitialized)
			{
				_observableFullMultiplayerState.Value = FullMultiplayerState.Initialized;
				//set IsInitialized = true, below in the state machine
			}
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
		
		private void RequireIsConnected()
		{
			if (!IsConnected)
			{
				throw new Exception("Must be connected");
			}
		}
		private void RequireIsNotConnected()
		{
			if (IsConnected)
			{
				throw new Exception("Must NOT be connected");
			}
		}

		//  Methods ---------------------------------------
		public async UniTask Connect()
		{
			RequireIsInitialized();
			OnConnectStarted.Invoke();
			_observableFullMultiplayerState.Value = FullMultiplayerState.Authenticating;
		}
		
		
		public bool CanStartAsHost()
		{
			return IsInitialized && _hasSetHostRelayData && !IsHost;
		}
		
		
		public bool CanJoinAsClient()
		{
			return IsInitialized &&_hasSetClientRelayData && !IsClient;
		}
		
		public bool CanShutdown()
		{
			return IsInitialized &&IsHost || IsClient;
		}

		public bool CanToggleStatsButton()
		{
			return IsInitialized && IsHost || IsClient;
		}

		public async UniTask StartAsHost()
		{
			RequireIsInitialized();
			RequireIsConnected();

			OnStateNameForDebuggingChanged.Invoke("StartingHost");
			IsHost = NetworkManager.Singleton.StartHost();
			if (IsHost)
			{
				OnStateNameForDebuggingChanged.Invoke("StartedHost");
			}
			
		}
		
		
		public async UniTask JoinAsClient()
		{
			RequireIsInitialized();
			RequireIsConnected();

			OnStateNameForDebuggingChanged.Invoke("StartingClient");
			IsClient = NetworkManager.Singleton.StartClient();
			if (IsClient)
			{
				OnStateNameForDebuggingChanged.Invoke("StartedClient");
			}
			
		}
		
		
		public async UniTask Shutdown()
		{
			RequireIsInitialized();
			RequireIsConnected();
			
			OnStateNameForDebuggingChanged.Invoke("ShuttingDown");
			NetworkManager.Singleton.Shutdown();
			OnStateNameForDebuggingChanged.Invoke("ShutDown");
		}

		public async UniTask DisconnectAsync()
		{
			RequireIsInitialized();
			RequireIsConnected();
			_observableFullMultiplayerState.Value = FullMultiplayerState.LobbyDisconnecting;

		}
		
		private async Task<Lobby> QuickJoinLobbyAsync()
		{
			Debug.LogWarning("QuickJoinLobbyAsync()");
			Lobby lobby = null;
			try
			{
				lobby = await Lobbies.Instance.QuickJoinLobbyAsync();
				if (lobby == null)
				{
					Debug.LogWarning($"QuickJoinLobbyAsync() found no lobby. That is ok.");
					return null;
				}
			}
			catch (Exception e)
			{
				if (IsAtRateLimit(e))
				{
					await WaitPerRateLimitAsync();
					return await QuickJoinLobbyAsync();
				}
				else if (e.Message.ToLower().Contains("failed to find"))
				{
					//No lobby yet, cool
					return null;
				}
				else
				{
					Debug.LogWarning($"QuickJoinLobbyAsync() failed. e = {e.Message}");
				}
			}

			string joinCode = "";
			try
			{
				joinCode = lobby.Data[JoinCodeKey].Value;

				if (string.IsNullOrEmpty(joinCode))
				{
					Debug.LogWarning($"QuickJoinLobbyAsync() found no joinCode. That is ok.");
					return null;
				}

				JoinAllocation joinAllocation = 
					await RelayService.Instance.JoinAllocationAsync(joinCode);
				
				_unityTransport.SetClientRelayData(
					joinAllocation.RelayServer.IpV4,
					(ushort)joinAllocation.RelayServer.Port,
					joinAllocation.AllocationIdBytes,
					joinAllocation.Key,
					joinAllocation.ConnectionData,
					joinAllocation.HostConnectionData);

				_hasSetClientRelayData = true;
				
				return lobby;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"QuickJoinLobbyAsync() failed. " +
				                 $"e = {e.Message} for joinCode = {joinCode}");

			}
	
			Debug.Log("QuickJoinLobbyAsync() - END");
			return null;
		}

		
		private async Task WaitPerRateLimitAsync()
		{
			Debug.LogWarning($"WaitPerRateLimit() {(RateLimitWaitTimeMilliseconds/1000)}s.");
			await Task.Delay(RateLimitWaitTimeMilliseconds);
		}

		
		private bool IsAtRateLimit(Exception exception)
		{
			return exception.Message.ToLower().Contains("rate limit");
		}

		
		private async Task<Lobby> CreateLobbyAsync()
		{
			Debug.LogWarning("CreateLobbyAsync()");
			
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);

			_lastAllocatedRegion = allocation.Region;
			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
			{
				Data = new Dictionary<string, DataObject>
				{
					{
						JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode)
					}
				},
				IsPrivate = false
			};

			Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("any lobby name",
				MaxConnections, createLobbyOptions);

			_unityTransport.SetHostRelayData(
				allocation.RelayServer.IpV4,
				(ushort)allocation.RelayServer.Port,
				allocation.AllocationIdBytes,
				allocation.Key,
				allocation.ConnectionData);

			_hasSetHostRelayData = true;
				
			return lobby;
	
		}

		
		private async Task SendHeartbeatPingRepeatingAsync(CancellationTokenSource cancellationTokenSource)
		{
			while (!cancellationTokenSource.IsCancellationRequested) 
			{
				// Do Stuff
				Debug.Log($"SendHeartbeatPingRepeatingAsync() id = {_lobby.Id});");
				await Lobbies.Instance.SendHeartbeatPingAsync(_lobby.Id);
				
				// Wait
				await Task.Delay(HeartBeatWaitTimeMilliseconds, cancellationTokenSource.Token);
			}
		}

		
		private async Task LeaveLobbySafeAsync()
		{
			if (_sendHeartbeatCancellationTokenSource != null)
			{
				_sendHeartbeatCancellationTokenSource.Cancel();
			}
			
			if (_lobby != null && 
			    Lobbies.Instance != null &&
			    !string.IsNullOrEmpty(_lobby.Id) &&
			    !string.IsNullOrEmpty(_authenticatedPlayerId))
			{
				// Reset the lobby data
				_hasSetClientRelayData = false;
				_hasSetHostRelayData = false;
				IsClient = false;
				IsHost = false;
				
				// The creator can remove the entire lobby
				if (_lobby.HostId == _authenticatedPlayerId)
				{
					try
					{
						await Lobbies.Instance.DeleteLobbyAsync(_lobby.Id);
					}
					catch (Exception e)
					{
						Debug.LogError($"DeleteLobbyAsync() failed. e = {e.Message}");
					}
					
				}
				// Otherwise just remove self
				else
				{
					try
					{
						await Lobbies.Instance.RemovePlayerAsync(_lobby.Id, _authenticatedPlayerId);
					}
					catch (Exception e)
					{
						Debug.LogError($"RemovePlayerAsync() failed. e = {e.Message}");
					}
				}
			}
		}


		private async Task DisconnectAsync_Internal()
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
							Debug.Log("Shutdown 3 ");
							NetworkManager.Singleton.Shutdown();
						}
					}
				}
			}
			catch (NullReferenceException)
			{
				//Do nothing. Sometimes "NetworkManager.Singleton" is null. That is ok.
			}
			
			IsConnected = false;

		}

		
		//  Event Handlers --------------------------------
		private void NetworkManager_OnTransportFailure()
		{
			Debug.Log("NetworkManager_OnTransportFailure");
		}
		
		private void NetworkManager_OnServerStarted()
		{
			Debug.Log("NetworkManager_OnServerStarted");
		}
		
		
		private async void ObservableFullMultiplayerState_OnValueChanged(
			FullMultiplayerState oldValue, FullMultiplayerState newValue)
		{
			
			OnStateNameForDebuggingChanged.Invoke(newValue.ToString());

			switch (newValue)
			{
				case FullMultiplayerState.Null:
					// Do nothing
					break;
				case FullMultiplayerState.Initialized:
					// Do nothing
					IsInitialized = true;
					break;
				case FullMultiplayerState.Authenticating:

					InitializationOptions initializationOptions = new InitializationOptions();

					////////////
					//Make each client unique for testing, so
					//The backend doesn't treat each instance as 'the same'
					string uniqueTestingProfile = TheGameConstants.GetNewUniqueTestingProfile();

					// Solution: Set unique profile name
					initializationOptions.SetProfile(uniqueTestingProfile);
					
					Debug.Log($"Authenticating with Profile={uniqueTestingProfile}.");
					
	
					////////////

					try
					{
						await UnityServices.InitializeAsync(initializationOptions);
					}
					catch (Exception e)
					{
						if (IsAtRateLimit(e))
						{
							await WaitPerRateLimitAsync();
							initializationOptions.SetProfile(uniqueTestingProfile);
						}
						else
						{
							Debug.LogWarning("Other Exception To Solve : " + e.Message);
						}
					}
					
					if (!AuthenticationService.Instance.IsSignedIn)
					{
						await AuthenticationService.Instance.SignInAnonymouslyAsync();
						_authenticatedPlayerId = AuthenticationService.Instance.PlayerId;
						_observableFullMultiplayerState.Value = FullMultiplayerState.Authenticated;
					}
					else
					{
						Debug.LogError($"SignInAnonymouslyAsync() failed. IsSignedIn must NOT be true already. ");
					}
					break;		
				case FullMultiplayerState.Authenticated:
					_observableFullMultiplayerState.Value = FullMultiplayerState.LobbyConnecting;
					break;	
				case FullMultiplayerState.LobbyConnecting:
					
					// Reset the lobby data
					_hasSetClientRelayData = false;
					_hasSetHostRelayData = false;
					IsHost = false;
					IsClient = false;
					
					_lobby = await QuickJoinLobbyAsync();

					if (_lobby == null)
					{
						_lobby = await CreateLobbyAsync();
					}
					
					if (_lobby == null)
					{
						Debug.LogError($"CreateLobbyAsync() failed. Must have _lobby.");
					}
					else
					{
						_observableFullMultiplayerState.Value = FullMultiplayerState.LobbyConnected;
					}
					
					break;	
				case FullMultiplayerState.LobbyConnected:
					IsConnected = true;
					string debugMessage = $"LastAllocatedRegion = {_lastAllocatedRegion}";
					OnConnectCompleted.Invoke(debugMessage);
					
					//////////////
					// Keep the connection alive with a heartbeat
					// Do not await this call
					if (IsHost)
					{
						_sendHeartbeatCancellationTokenSource = new CancellationTokenSource();
						Task.Run(async () => await SendHeartbeatPingRepeatingAsync(
							_sendHeartbeatCancellationTokenSource));
					}
					
					break;	
				case FullMultiplayerState.LobbyDisconnecting:
					
					await LeaveLobbySafeAsync();
					await DisconnectAsync_Internal();
					
					// Reset the lobby data
					_hasSetClientRelayData = false;
					_hasSetHostRelayData = false;
					IsHost = false;
					IsClient = false;
					
					_observableFullMultiplayerState.Value = FullMultiplayerState.LobbyDisconnected;
					break;
				case FullMultiplayerState.LobbyDisconnected:
	
					OnDisconnectCompleted.Invoke();
					break;
				default:
					SwitchDefaultException.Throw(newValue);
					break;
			}
		}


	}
}
