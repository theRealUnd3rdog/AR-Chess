using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Transporting;


public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    /// <summary>
    /// Called when the player changes any information
    /// </summary>
    public static event Action<NetworkConnection, PlayerInfo> OnInfoChange;

    /// <summary>
    /// Collection of players
    /// </summary>
    [SyncObject]
    public readonly SyncDictionary<NetworkConnection, PlayerInfo> Players = new SyncDictionary<NetworkConnection, PlayerInfo>();

    private void Awake()
    {
        Instance = this;

        Players.OnChange += PlayerInfo_OnChange;
    }

    private void OnDestroy()
    {
        Players.OnChange -= PlayerInfo_OnChange;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && IsServer)
        { 
            foreach (var entity in Players)
            {
                NetworkConnection connection = entity.Key;
                PieceTeam team = entity.Value.team;
                string name = entity.Value.name;

                Debug.Log($"Connection: {connection}  Team: {team}  Name: {name}");
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Subscribe to server manager to know when a client disconnects, cleaning the dictionary for any clients that aren't in the game
        base.NetworkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // Remove any from the dictonary if it isn't started or if any client hasn't joined yet
        base.NetworkManager.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
    }

    /// <summary>
    /// Called when remote client connection state changes
    /// </summary>
    private void ServerManager_OnRemoteConnectionState(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {
        // If the connection isn't present anymore, remove the player
        if (arg2.ConnectionState != RemoteConnectionState.Started)
            Players.Remove(arg1);
    }

    /// <summary>
    /// Callback when the collection changes
    /// </summary>
    private void PlayerInfo_OnChange(SyncDictionaryOperation op, NetworkConnection key, PlayerInfo value, bool asServer)
    {
        if (asServer)
            return;

        if (op == SyncDictionaryOperation.Add || op == SyncDictionaryOperation.Set)
            OnInfoChange?.Invoke(key, value);
    }

    /// <summary>
    /// Returns the entity containing player info
    /// </summary>
    /// <param name="conn">The client's network connection</param>
    public static PlayerInfo GetPlayerInfo(NetworkConnection conn)
    {
        if (Instance.Players.TryGetValue(conn, out PlayerInfo entity))
            return entity;
        else
            return default(PlayerInfo);
    }

    /// <summary>
    /// Set an entity to player info collection on the server
    /// </summary>
    public static void SetInfo(PlayerInfo entity, NetworkConnection sender = null)
    {
        Instance.ServerSetInfo(entity, sender);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSetInfo(PlayerInfo entity, NetworkConnection sender = null)
    {
        Players[sender] = entity;
    }
}
