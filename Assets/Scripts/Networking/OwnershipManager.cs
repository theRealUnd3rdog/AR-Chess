using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing;

public class OwnershipManager : NetworkBehaviour
{
    public static OwnershipManager Instance;

    /// <summary>
    /// Called when the player spawns
    /// </summary>
    public static event Action<NetworkObject> OnSpawned;

    private NetworkManager _networkManager;

    [Header("Networking")]
    [SerializeField] private NetworkObject _playerPrefab;

    [Tooltip("True to add player to the active scene when no global scenes are specified through the SceneManager.")]
    [SerializeField]
    private bool _addToDefaultScene = true;
    
    [Header("UI")]
    [SerializeField] private Canvas _teamSelector;
    [SerializeField] private Canvas _waitScreen;

    private void Awake()
    {
        Instance = this;

        _networkManager = InstanceFinder.NetworkManager;

        if (_networkManager != null)
            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;

        PlayerManager.OnInfoChange += OnInfoChange_EnterGameScreen;
    }

    private void OnDestroy()
    {
        if (_networkManager != null)
            _networkManager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
        
        PlayerManager.OnInfoChange -= OnInfoChange_EnterGameScreen;
    }

    /// <summary>
    /// Called as soon as the client loads into a scene
    /// </summary>
    private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (asServer)
            Debug.Log($"Client with {conn} has connected");

        if (!asServer)
        {        
            _teamSelector.gameObject.SetActive(true);
        }
    }

    public void SelectTeam_White()
    {
        SelectTeam(PieceTeam.White);
    }

    public void SelectTeam_Black()
    {
        SelectTeam(PieceTeam.Black);
    }

    private void SelectTeam(PieceTeam team)
    {
        if (IsTeamTaken(team))
        {
            Debug.LogWarning($"{team} is already taken, please select another team");
            return;
        }

        PlayerInfo info;
        info.team = team;
        info.name = $"{team} boi";

        // Set the local team per player
        GameManager.Instance.PlayerTeam = team;

        // Set info
        PlayerManager.SetInfo(info);

        _teamSelector.gameObject.SetActive(false);
        _waitScreen.gameObject.SetActive(true);
    }

    private void OnInfoChange_EnterGameScreen(NetworkConnection arg1, PlayerInfo arg2)
    {
        if (IsAllTeamsTaken())
        {
            _teamSelector.gameObject.SetActive(false);
            _waitScreen.gameObject.SetActive(false);

            GameManager.SetGameState(GameState.Started);
            SpawnPlayer_Server(arg1);
        }
    }

    public static void SpawnAllPlayers()
    {
        // spawn all players
        foreach (var info in PlayerManager.Instance.Players)
        {
            Debug.Log(info.Key);
            Instance.SpawnPlayer_Server(info.Key);
        }
    }

    private bool IsTeamTaken(PieceTeam team)
    {
        foreach (var info in PlayerManager.Instance.Players)
        {
            if (team == info.Value.team)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAllTeamsTaken()
    {
        bool whiteTaken = false;
        bool blackTaken = false;

        foreach (var info in PlayerManager.Instance.Players)
        {
            if (!whiteTaken && info.Value.team == PieceTeam.White)
            {
                whiteTaken = true;
            }

            if (!blackTaken && info.Value.team == PieceTeam.Black)
            {
                blackTaken = true;
            }
        }

        if (whiteTaken && blackTaken)
            return true;

        return false;
    }

    /// <summary>
    /// Spawns player and communicates client to server
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer_Server(NetworkConnection conn = null)
    {
        NetworkObject nob = _networkManager.GetPooledInstantiated(_playerPrefab, true);
        
        _networkManager.ServerManager.Spawn(nob, conn);

        if (_addToDefaultScene)
            _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

        OnSpawned?.Invoke(nob);
    }
}
