using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

[System.Serializable]
public struct PlayerInfo
{
    public PieceTeam team;
    public string name;
}

public class PlayerClient : NetworkBehaviour
{
    /// <summary>
    /// Invoked when the client spawns and returns the player client class
    /// </summary>
    public static event Action<PlayerClient> OnClientSpawned;

    /// <summary>
    /// Invoked when the client despawns and returns the player client class
    /// </summary>
    public static event Action<PlayerClient> OnClientDespawned;

    /// <summary>
    /// Hold temporary player information on the local client and does not sync
    /// </summary>
    private PlayerInfo _info;
    
    /// <summary>
    /// Holds a reference of the score on the client
    /// </summary>
    private PlayerScore _scoreInfo;

    public override void OnStartClient()
    {
        base.OnStartClient();

        OnClientSpawned?.Invoke(this);

        PlayerManager.OnInfoChange += PlayerInfo_OnChange;
        this.transform.SetParent(PlayerManager.Instance.transform);

        // Set info upon spawning
        SetPlayerInfo();

        if (!base.IsOwner)
            PieceManager.Instance.InitializeTeam(PlayerManager.GetPlayerInfo(base.Owner).team);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        OnClientDespawned?.Invoke(this);

        PlayerManager.OnInfoChange -= PlayerInfo_OnChange;
    }

    /// <summary>
    /// Resets the player information everytime the information collection changes
    /// </summary>
    private void PlayerInfo_OnChange(NetworkConnection arg1, PlayerInfo arg2)
    {
        SetPlayerInfo();
    }

    /// <summary>
    /// Sets the player information so it updates on all clients
    /// </summary>
    private void SetPlayerInfo()
    {
        _info = PlayerManager.GetPlayerInfo(base.Owner);
        this.gameObject.name = _info.name;
    
        SetPlayerScore();
    }

    private void SetPlayerScore()
    {
        if (_scoreInfo != null)
            _scoreInfo.UpdateScore(base.Owner);
    }

    public void SetPlayerScore_Client(PlayerScore score) => _scoreInfo = score;
    public PlayerScore GetPlayerScore_Client()
    {
        return _scoreInfo;
    }
}
