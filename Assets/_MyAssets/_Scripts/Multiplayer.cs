using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Multiplayer : MonoBehaviour
{
    public static Multiplayer Singleton { get; private set; }

    [SerializeField] private GameObject _networkPlayerPrefab;
    [SerializeField] private GameObject _networkHeadPrefab;
    [SerializeField] private GameObject _networkLeftHandPrefab;
    [SerializeField] private GameObject _networkRightHandPrefab;

    private Lobby _joinedLobby;
    private float _sendHeartBeatTimer = 0;

    private readonly float SEND_HEART_BEAT_TIME = 15;
    public readonly string RELAY_JOIN_CODE = "RelayJoinCode";


    void Awake()
    {
        Singleton = this;
    }

    async void Start()
    {
        NetworkManager.Singleton.OnServerStarted += NetworkManager_OnServerStarted;

        try
        {
            await SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    void Update()
    {
        SendHeartBeat();
    }

    public async Task SignInAnonymouslyAsync()
    {
        InitializationOptions options = new InitializationOptions()
            .SetProfile("Profile" + UnityEngine.Random.Range(0, 100));

        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("SignInAnonymouslyAsync");
    }


    #region Lobby & Relay
    public async Task<bool> CreateLobbyAsync(string lobbyName, bool isPrivate = false, bool isLocked = false)
    {
        try
        {
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(10);
            var relayJoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsLocked = isLocked,
                IsPrivate = isPrivate,
                Data = new System.Collections.Generic.Dictionary<string, DataObject> {
                    {
                        RELAY_JOIN_CODE,
                        new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode)
                    }
                }
            };

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);
            _joinedLobby = lobby;

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public async Task<bool> QuickJoinLobbyAsync()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            _joinedLobby = lobby;

            var relayJoinCode = lobby.Data[RELAY_JOIN_CODE].Value;
            var allocation = await Relay.Instance.JoinAllocationAsync(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public void SendHeartBeat()
    {
        bool isLobbyOwner = _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        if (!isLobbyOwner)
        {
            return;
        }

        this._sendHeartBeatTimer -= Time.deltaTime;
        if (this._sendHeartBeatTimer <= 0)
        {
            this._sendHeartBeatTimer = SEND_HEART_BEAT_TIME;
            LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
        }
    }
    #endregion


    #region Network Event
    private void NetworkManager_OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("New Client " + clientId);
        ServerSpawnNetworkPlayer(clientId);
    }

    private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
    {
        Debug.Log("Client Disconnected " + clientId);
    }
    #endregion



    private void ServerSpawnNetworkPlayer(ulong clientId)
    {
        var netPlayer = Instantiate(_networkPlayerPrefab);
        var netHead = Instantiate(_networkHeadPrefab);
        var netLeftHand = Instantiate(_networkLeftHandPrefab);
        var netRightHand = Instantiate(_networkRightHandPrefab);

        var netPlayerObj = netPlayer.GetComponent<NetworkObject>();
        var netHeadObj = netHead.GetComponent<NetworkObject>();
        var netLeftHandObj = netLeftHand.GetComponent<NetworkObject>();
        var netRightHandObj = netRightHand.GetComponent<NetworkObject>();

        netPlayerObj.SpawnWithOwnership(clientId);
        netHeadObj.SpawnWithOwnership(clientId);
        netLeftHandObj.SpawnWithOwnership(clientId);
        netRightHandObj.SpawnWithOwnership(clientId);

        // netPlayerObj.TrySetParent(netPlayer);
        netHeadObj.TrySetParent(netPlayer);
        netLeftHandObj.TrySetParent(netPlayer);
        netRightHandObj.TrySetParent(netPlayer);
    }
}