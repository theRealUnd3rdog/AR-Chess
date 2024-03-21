using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using FishNet.Connection;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _teamText;
    [SerializeField] private TextMeshProUGUI _timeText;

    private PlayerInfo _playerInfo;

    private void Update()
    {
        _nameText.text = _playerInfo.name;
        _teamText.text = $"{_playerInfo.team}";
    }

    public void UpdateScore(NetworkConnection key = null)
    {
        if (PlayerManager.Instance.Players.ContainsKey(key))
            _playerInfo = PlayerManager.GetPlayerInfo(key);
    }
}
