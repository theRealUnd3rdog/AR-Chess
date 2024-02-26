using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PieceTeam CurrentTeam;

    // Define an event to signal when a move has been made
    public static event Action<PieceTeam> MoveMade;

    private void Awake()
    {
        Instance = this;
        CurrentTeam = PieceTeam.White; // Start with White team
    }

    // Method to notify subscribers that a move has been made
    public static void NotifyMoveMade(PieceTeam team)
    {
        MoveMade?.Invoke(team);
    }
}
