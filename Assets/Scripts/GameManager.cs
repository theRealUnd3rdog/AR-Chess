using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TeamChecker
{
    public PieceTeam team;
    public bool check;
    public bool checkMate;
    public List<Piece> threatningPieces;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PieceTeam CurrentTeam;
    public TeamChecker[] TeamCheck;

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

        PieceManager.Instance.CheckTeam(PieceTeam.White);
        PieceManager.Instance.CheckTeam(PieceTeam.Black);
    }

    public static void CheckMateTeam(PieceTeam team)
    {
        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                Instance.TeamCheck[i].checkMate = true;
       
                break;
            }
        }
    }

    public static void CheckTeam(PieceTeam team, bool check)
    {
        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                Instance.TeamCheck[i].check = check;
       
                break;
            }
        }
    }

    public static List<Piece> GetThreatPieces(PieceTeam team)
    {
        List<Piece> pieces = new List<Piece>();

        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                pieces = Instance.TeamCheck[i].threatningPieces;

                break;
            }
        }

        return pieces;
    }

    public static void SetThreatenPieces(PieceTeam team, List<Piece> pieces)
    {
        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                Instance.TeamCheck[i].threatningPieces = pieces;
       
                break;
            }
        }
    }

    public static bool GetCheckedTeam(out PieceTeam team)
    {
        bool flag = false;
        team = PieceTeam.White;

        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (Instance.TeamCheck[i].check)
            {
                flag = true;
                team = Instance.TeamCheck[i].team;
            }
        }

        return flag;
    }
}
