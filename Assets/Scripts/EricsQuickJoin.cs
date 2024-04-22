using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Matchmaker;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using System.Threading;
using UnityEngine.SceneManagement;

/// <summary>
/// CornhacksEloJoin is a Unity Quick Join script modified for potential ELO rating matchmaking. It utilizes Lobby and Relay to find a similarly skilled player and connect to them.
/// </summary>
public class EricsQuickJoin : NetworkBehaviour
{
	public string newLobbyName = "VRRTSn" + Guid.NewGuid();
	private int maxPlayers = 2;
	private int currentPlayers = 1;
	public bool isPrivate = false;

	private static Lobby currentLobby;
	private static CancellationTokenSource heartbeatSource;

	public NetworkManager netMan;

	// these two probably aren't important for later
	public TMP_Text JoinCodeText;
	public TMP_InputField JoinCodeInputField;

	private static UnityTransport _transport;
	private static UnityTransport transport
	{
		get => _transport != null ? _transport : _transport = UnityEngine.Object.FindObjectOfType<UnityTransport>();
		set => _transport = value;
	}

	private ILobbyEvents currentLobbyEvents;

	public NetworkObject networkGameManager;

	private async void Awake()
	{
		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync(); // change when stretch goal
	}

	public async void TryFindMatch()
	{
		DontDestroyOnLoad(this);
		try
		{
			await FindMatch();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	async Task FindMatch()
	{
		await UnityServices.InitializeAsync();

		Player loggedInPlayer = await GetPlayerLogin();
		loggedInPlayer.Data.Add("Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "No"));

		List<QueryFilter> queryFilters = new List<QueryFilter>
		{
			new QueryFilter(
				field:QueryFilter.FieldOptions.AvailableSlots,
				op: QueryFilter.OpOptions.LT,
				value: "2"),
			//new QueryFilter(
			//	field:QueryFilter.FieldOptions.S1,
			//	op: QueryFilter.OpOptions.LT,
			//	value: "100"), //ELO
			//new QueryFilter(
			//	field:QueryFilter.FieldOptions.S1,
			//	op: QueryFilter.OpOptions.GT,
			//	value: "50"), //ELO
			new QueryFilter(
				field:QueryFilter.FieldOptions.IsLocked,
				op: QueryFilter.OpOptions.EQ,
				value:"0")

		};

		List<QueryOrder> queryOrdering = new List<QueryOrder>
		{
			new QueryOrder(true, QueryOrder.FieldOptions.AvailableSlots),
			new QueryOrder(false, QueryOrder.FieldOptions.Created),
			new QueryOrder(false, QueryOrder.FieldOptions.Name),
		};

		QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions()
		{
			Count = 20,
			Filters = queryFilters,
			Order = queryOrdering
		});

		List<Lobby> foundLobbies = response.Results;
		Debug.Log(foundLobbies.Count > 0);

		try
		{
			Debug.Log("Looking for a match to join");
			if (foundLobbies.Count > 0)
			{
				try
				{
					currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(foundLobbies[0].Id);
				}
				catch (LobbyServiceException ex)
				{
					Debug.Log(ex);
				}
			}
			else
			{
				currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions
				{
					Player = loggedInPlayer,
					Filter = queryFilters
				});
			}

			Debug.Log($"Found Lobby:{currentLobby.Name}");
			var allocation = await RelayService.Instance.JoinAllocationAsync(currentLobby.Data["JoinKey"].Value);
			transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

			//do something different here
			netMan.StartClient();
		}
		catch (LobbyServiceException ex) //I guess I should do this again with wider filters afterwards, maybe later
		{
			if (ex.Reason == LobbyExceptionReason.NoOpenLobbies)
			{
				Debug.LogWarning("There are no lobbies to join currently.");
			}
			else
			{
				Debug.LogException(ex);
				return;
			}

			//creating a relay allocation for the lobby to store
			var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
			var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			var lobbyData = new Dictionary<string, DataObject>
			{
				["HostELO"] = new DataObject(DataObject.VisibilityOptions.Public, "75", DataObject.IndexOptions.S1), //change when ELO is implemented
				["JoinKey"] = new DataObject(DataObject.VisibilityOptions.Member, joinCode, DataObject.IndexOptions.S2),
			};

			var lobbyOptions = new CreateLobbyOptions
			{
				Data = lobbyData,
				IsPrivate = isPrivate,
				Player = loggedInPlayer
			};

			currentLobby = await LobbyService.Instance.CreateLobbyAsync(
				lobbyName: newLobbyName,
				maxPlayers: maxPlayers,
				options: lobbyOptions
				);

			transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

			//Heartbeat and refresh, look into once this is set

			var callbacks = new LobbyEventCallbacks();
			callbacks.PlayerJoined += CallbacksPlayerJoined;
			try
			{
				currentLobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callbacks);
			}
			catch (LobbyServiceException exc)
			{
				Debug.LogException(exc);
			}
			//more callbacks stuff, if this becomes important come back to this point

			Debug.Log($"Created new lobby {currentLobby.Name} ({currentLobby.Id})");

			Heartbeat();
			netMan.StartHost(); //change this
		}

		if (currentLobby == null)
		{
			Debug.Log("Couldn't find or create a new lobby.");
			return;
		}
		Debug.Log($"Joined lobby {currentLobby.Name} ({currentLobby.Id})");
	}

	private async void CallbacksPlayerJoined(List<LobbyPlayerJoined> obj)
	{
		Debug.Log(obj[currentPlayers].Player.Id);
		if (currentPlayers == 1)
        {
			NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
		if (currentPlayers < maxPlayers)
		{
			currentPlayers++;
			if (currentPlayers == maxPlayers)
            {
				await LockLobby();
				FindObjectOfType<GameManager>().gameObject.SetActive(false);
				Instantiate(networkGameManager);
			}
		}
	}

	static async Task<Player> GetPlayerLogin()
	{
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			Debug.Log("Signing in Anonymously");

			await AuthenticationService.Instance.SignInAnonymouslyAsync();

			if (!AuthenticationService.Instance.IsSignedIn)
			{
				throw new InvalidOperationException("Failed to sign in a player, and thus cannot continue.");
			}
		}

		Debug.Log("Player signed in as " + AuthenticationService.Instance.PlayerId);

		return new Player(AuthenticationService.Instance.PlayerId, null, data: new Dictionary<string, PlayerDataObject>());
	}

	private static async void Heartbeat()
	{
		heartbeatSource = new CancellationTokenSource();
		while (!heartbeatSource.IsCancellationRequested && currentLobby != null)
		{
			await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);
			await Task.Delay(15000); // change this
		}
	}

	public static async Task LockLobby()
	{
		try
		{
			await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}
}
