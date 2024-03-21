using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

[System.Serializable]
public struct TeamChecker
{
    public PieceTeam team;
    public bool check;
    public bool checkMate;
    public bool staleMate;
    public List<Piece> threatningPieces;
    public List<Tile> pawnTiles;
}

public enum GameState
{
    Waiting,
    Started,
}


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SyncVar]
    public GameState state;

    [SyncVar]
    public PieceTeam CurrentTeam;
    public PieceTeam PlayerTeam;

    public TeamChecker[] TeamCheck;

    // Define an event to signal when a move has been made
    public static event Action<PieceTeam> MoveMade;

    private void Awake()
    {
        Instance = this;
        CurrentTeam = PieceTeam.White; // Start with White team
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ServerSetGameState(GameState.Waiting);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerSetCurrentTeam(PieceTeam team)
    {
        CurrentTeam = team;
    }

    public static void SetGameState(GameState state)
    {
        Instance.ServerSetGameState(state);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerSetGameState(GameState state)
    {
        Instance.state = state;
    }

    public static void StorePawnTiles(PieceTeam team, List<Tile> tiles)
    {
        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                Instance.TeamCheck[i].pawnTiles = tiles;

                break;
            }
        }
    }

    public static bool IsPawnOnPromotionTile(Piece piece)
    {
        PieceTeam oppositeTeam = piece.Team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White;

        List<Tile> pawnTiles = new List<Tile>();
        Tile pieceTile = piece.currentTile;

        foreach (TeamChecker teamCheck in Instance.TeamCheck)
        {
            if (teamCheck.team == oppositeTeam)
                pawnTiles = teamCheck.pawnTiles;
        }

        if (pawnTiles.Any(tile => tile == pieceTile))
        {
            return true;
        }

        return false;
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

    public static void StateMateTeam(PieceTeam team)
    {
        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (team == Instance.TeamCheck[i].team)
            {
                Instance.TeamCheck[i].staleMate = true;

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

    public static bool IsTeamChecked(PieceTeam team)
    {
        bool flag = false;

        for (int i = 0; i < Instance.TeamCheck.Length; i++)
        {
            if (Instance.TeamCheck[i].check)
            {
                flag = Instance.TeamCheck[i].check;
                break;
            }
        }

        return flag;
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
