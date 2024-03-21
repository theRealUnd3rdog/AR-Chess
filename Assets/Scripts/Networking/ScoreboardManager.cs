using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager Instance;
    private bool _isScore;

    [Header("Scoreboard")]
    [SerializeField] private GameObject _scoreboardPanel;

    /// <summary>
    /// The group containing the vertical layout UI to add info
    /// </summary>
    [SerializeField] private GameObject _scoreGroup;

    /// <summary>
    /// The prefab containing the info
    /// </summary>
    [SerializeField] private GameObject _scoreInfoPrefab;

    private void Awake()
    {
        Instance = this;
        _isScore = false;

        PlayerClient.OnClientSpawned += PlayerClient_OnClientSpawned;
        PlayerClient.OnClientDespawned += PlayerClient_OnClientDespawned;
    }

    private void OnDestroy()
    {
        PlayerClient.OnClientSpawned -= PlayerClient_OnClientSpawned;
        PlayerClient.OnClientDespawned -=PlayerClient_OnClientDespawned;
    }

    private void PlayerClient_OnClientSpawned(PlayerClient client)
    {
        // Instantiate score
        InstantiateScoreInfo(out GameObject spawnedInfo);
        PlayerScore score = spawnedInfo.GetComponent<PlayerScore>();

        client.SetPlayerScore_Client(score);
    }

    private void PlayerClient_OnClientDespawned(PlayerClient client)
    {
        Destroy(client.GetPlayerScore_Client().gameObject);
    }

    public void InstantiateScoreInfo(out GameObject spawnedInfo)
    {
        spawnedInfo = null;
        spawnedInfo = Instantiate(_scoreInfoPrefab, _scoreGroup.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            _isScore = true;
        else if (Input.GetKeyUp(KeyCode.Tab))
            _isScore = false;

        ToggleScoreboard(_isScore);
    }

    private void ToggleScoreboard(bool flag) => _scoreboardPanel.SetActive(flag);
}
