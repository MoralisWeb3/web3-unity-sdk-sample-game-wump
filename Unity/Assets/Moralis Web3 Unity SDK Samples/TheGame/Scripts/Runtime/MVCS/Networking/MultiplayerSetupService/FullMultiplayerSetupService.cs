using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RMC.Shared.Exceptions;
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
#if UNITY_EDITOR
using UnityEditor;
#endif

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
		private UnityEvent _onConnectionStarted = new UnityEvent();
		private StringUnityEvent _onStateNameChanged = new StringUnityEvent();
		private  StringUnityEvent _onConnectionCompleted = new StringUnityEvent();

		//  Properties ------------------------------------
		public bool IsConnected { get; private set; }
		public UnityEvent OnConnectionStarted { get { return _onConnectionStarted; } }
		public StringUnityEvent OnConnectionCompleted { get { return _onConnectionCompleted; } }
		public StringUnityEvent OnStateNameChanged { get { return _onStateNameChanged; } }
		
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

		//  Initializer Methods ---------------------------------
		public FullMultiplayerSetupService(UnityTransport unityTransport)
		{
			_unityTransport = unityTransport;
			_observableFullMultiplayerState.OnValueChanged.AddListener(ObservableFullMultiplayerState_OnValueChanged);
			IsConnected = false;
			_observableFullMultiplayerState.Value = FullMultiplayerState.Null;
		}


		//  Methods ---------------------------------------
		public void Connect()
		{
			_onConnectionStarted.Invoke();
			_observableFullMultiplayerState.Value = FullMultiplayerState.Authenticating;
		}

		public void OnGUI()
		{
			//Do nothing
		}

		public async UniTask DisconnectAsync()
		{
			await LeaveLobbySafeAsync();
			await DisconnectAsync_Internal();
		}
		
		private async Task<Lobby> QuickJoinLobbyAsync()
		{
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

				NetworkManager.Singleton.StartClient();
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

				if (NetworkManager.Singleton != null)
				{
					NetworkManager.Singleton.StartHost();
				}
				
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
			try
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
					if (_lobby.HostId == _authenticatedPlayerId)
					{
						await Lobbies.Instance.DeleteLobbyAsync(_lobby.Id);
					}
					else
					{
						if (NetworkManager.Singleton != null)
						{
							foreach (var x in NetworkManager.Singleton.ConnectedClients)
							{
								Debug.Log("next : " + x.Key + " / " + x.Value);
							}
						}
	
						await Lobbies.Instance.RemovePlayerAsync(_lobby.Id, _authenticatedPlayerId);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"CreateLobbyAsync() failed. e = {e.Message}");
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
		private async void ObservableFullMultiplayerState_OnValueChanged(
			FullMultiplayerState oldValue, FullMultiplayerState newValue)
		{
			
			OnStateNameChanged.Invoke(newValue.ToString());
			
			switch (newValue)
			{
				case FullMultiplayerState.Null:
					// Do nothing
					break;
				case FullMultiplayerState.Authenticating:
					
					InitializationOptions initializationOptions = new InitializationOptions();

					////////////
					//Make each client unique for testing, so
					//The backend doesn't treat each instance as 'the same'
					string uniqueTestingProfile = "Client_" + Guid.NewGuid();
					
					// Solution #1: Set unique profile name
					initializationOptions.SetProfile(uniqueTestingProfile);
					
#if UNITY_EDITOR
					// Solution #2: Set unique product name
					PlayerSettings.productName = uniqueTestingProfile;
#endif 
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
					OnConnectionCompleted.Invoke(debugMessage);
					
					//////////////
					// Keep the connection alive with a heartbeat
					// Do not await this call
					if (NetworkManager.Singleton.IsHost)
					{
						_sendHeartbeatCancellationTokenSource = new CancellationTokenSource();
						Task.Run(async () => await SendHeartbeatPingRepeatingAsync(
							_sendHeartbeatCancellationTokenSource));
					}
					
					break;	
				default:
					SwitchDefaultException.Throw(newValue);
					break;
			}
		}
	}
}
