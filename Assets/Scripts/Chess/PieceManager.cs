using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using UnityEngine;

public class PieceManager : NetworkBehaviour
{
    public static PieceManager Instance;

    public List<Piece> whiteTeam;
    public List<Piece> blackTeam;

    [SerializeField] private PieceSO _whitePieces;
    [SerializeField] private PieceSO _blackPieces;

    private string[] _pieceOrder = {"Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook"};
    private TileManager _tileManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _tileManager = TileManager.Instance;

        GameManager.MoveMade += PerformCheckMate;
    }
    
    private void OnDestroy()
    {
        GameManager.MoveMade -= PerformCheckMate;
    }

    public void InitializeTeam(PieceTeam team)
    {
        switch (team)
        {
            case PieceTeam.White:
                InstantiatePieces(_whitePieces);
                GameManager.StorePawnTiles(PieceTeam.White, GetPawnTiles(PieceTeam.White));

                break;

            case PieceTeam.Black:
                InstantiatePieces(_blackPieces);
                GameManager.StorePawnTiles(PieceTeam.Black, GetPawnTiles(PieceTeam.Black));
                
                break;
        }
    }


    public List<Tile> GetPawnTiles(PieceTeam team)
    {
        List<Tile> pawnTiles = new List<Tile>();
        string[] coordinates;

        switch (team)
        {
            case PieceTeam.White:
                coordinates = _whitePieces.columnCoordinates[0].coordinates;
                pawnTiles = TileManager.GetTilesByNames(coordinates);

                break;

            case PieceTeam.Black:
                coordinates = _blackPieces.columnCoordinates[0].coordinates;
                pawnTiles = TileManager.GetTilesByNames(coordinates);

                break;
        }

        return pawnTiles;
    }


    [ServerRpc(RequireOwnership = false)]
    private void InstantiatePieces(PieceSO pieceSO)
    {
        // Grab pieces
        Piece[] pieces = pieceSO.pieces;
        Coordinates[] coordinates = pieceSO.columnCoordinates;

        // Places the pieces in order depending on the structure
        for (int i = 0; i < coordinates.Length; i++)
        {
            for (int j = 0; j < coordinates[i].coordinates.Length; j++)
            {
                Tile tile = _tileManager.tiles[0];
                Piece pieceToSpawn = pieces[0];

                // Get the tile needed
                for (int k = 0; k < _tileManager.tiles.Length; k++)
                {
                    if (coordinates[i].coordinates[j] == _tileManager.tiles[k].name)
                    {
                        tile = _tileManager.tiles[k];
                        continue;
                    }
                }

                // Spawn the unique pieces first
                if (i == 0)
                {
                    // Get the piece necessary
                    for (int k = 0; k < pieces.Length; k++)
                    {
                        if (Enum.GetName(typeof(PieceType),pieces[k].Type) == _pieceOrder[j])
                        {
                            pieceToSpawn = pieces[k];
                        }
                    }
                    
                    Piece piece = Instantiate(pieceToSpawn, tile.transform.position, Quaternion.identity);
                    piece.transform.SetParent(tile.transform);

                    ServerManager.Spawn(piece.gameObject);
                    InitializePiece(piece, tile);
                }
                // Spawn the pawns after
                else
                {
                    // Get the tile
                    pieceToSpawn = pieces[pieces.Length - 1];

                    Piece piece = Instantiate(pieceToSpawn, tile.transform.position, Quaternion.identity);
                    piece.transform.SetParent(tile.transform);
                    
                    ServerManager.Spawn(piece.gameObject);

                    InitializePiece(piece, tile);
                }
            }
        }
    }

    [ObserversRpc]
    private void InitializePiece(Piece piece, Tile tile)
    {
        // Handle rook side for castling later
        Rook rook = piece.GetComponent<Rook>();

        if (rook != null)
        {
            switch (tile.name)
            {
                case "A1":
                    rook.Side = RookSide.Left;
                    break;

                case "H1":
                    rook.Side = RookSide.Right;
                    break;

                case "H8":
                    rook.Side = RookSide.Left;
                    break;

                case "A8":
                    rook.Side = RookSide.Right;
                    break;
            }
        }

        piece.currentTile = tile;
        SetTeam(piece);
    }

    public static void InstantiatePiece<T>(Tile tile, PieceTeam team)
    {
        Piece[] pieces = null;

        switch (team)
        {
            case PieceTeam.White:
                pieces = Instance._whitePieces.pieces;
                break;

            case PieceTeam.Black:
                pieces = Instance._blackPieces.pieces;
                break;
        }

        Piece piece = null;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] is T)
            {
                piece = pieces[i];
                break;
            }
        }

        if (piece != null)
        {
            Instance.ServerInitializeSinglePiece(piece, tile);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerInitializeSinglePiece(Piece piece, Tile tile)
    {
        Piece instantiatedPiece = Instantiate(piece, tile.transform.position, Quaternion.identity);
        ServerManager.Spawn(instantiatedPiece.gameObject);

        instantiatedPiece.MoveToTileNonNotify(tile);

        InitializeSinglePiece(piece);
    }

    [ObserversRpc]
    private void InitializeSinglePiece(Piece piece)
    {
        Instance.SetTeam(piece);
    }

    private void SetTeam(Piece piece)
    {
        PieceTeam team = piece.Team;

        switch (team)
        {
            case PieceTeam.White:
                whiteTeam.Add(piece);
                break;

            case PieceTeam.Black:
                blackTeam.Add(piece);
                break;
        }
    }

    public void CheckTeam(PieceTeam team)
    {
        PieceTeam oppositeTeam = PieceTeam.White;

        if (team is PieceTeam.White)
            oppositeTeam = PieceTeam.Black;

        List<Piece> pieces = GetPiecesThatAreCheckingKing(oppositeTeam);

        // If the list is empty
        if (pieces.Count <= 0)
        {
            GameManager.CheckTeam(team, false);
            return;
        }

        GameManager.CheckTeam(team, true);
    }

    public void PerformCheckMate(PieceTeam team)
    {
        //bool check = GameManager.GetCheckedTeam(out PieceTeam team);

        PieceTeam oppositeTeam = team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White;

        List<Tile> tiles = GetAllPseudoValidMovesTeam(oppositeTeam);

        Debug.LogWarning("Number of valid tiles: " + tiles.Count);

        if (tiles.Count <= 0)
        {
            GameManager.CheckMateTeam(oppositeTeam);
            
            Debug.Log($"{team} has won");
        }
    }

    public Rook GetRook(PieceTeam team, RookSide side)
    {
        Rook rook = null;

        switch (team)
        {
            case PieceTeam.White:
                foreach (Piece piece in whiteTeam)
                {
                    if (piece is Rook)
                    {
                        Rook rookComp = piece.GetComponent<Rook>();

                        if (rookComp.Side == side)
                        {
                            rook = rookComp;
                            break;
                        }
                            
                    }
                }

                break;

            case PieceTeam.Black:
                foreach (Piece piece in blackTeam)
                {
                    if (piece is Rook)
                    {
                        Rook rookComp = piece.GetComponent<Rook>();

                        if (rookComp.Side == side)
                        {
                            rook = rookComp;
                            break;
                        }
                    }
                        
                }

                break;
        }

        return rook;
    }

    public Tile GetPieceCurrentTile<T>(PieceTeam team)
    {
        Tile tile = default;

        switch (team)
        {
            case PieceTeam.White:
                foreach (Piece piece in whiteTeam)
                {
                    if (piece is T)
                        tile = piece.currentTile;
                }

                break;

            case PieceTeam.Black:
                foreach (Piece piece in blackTeam)
                {
                    if (piece is T)
                        tile = piece.currentTile;
                }

                break;
        }

        return tile;
    }

    public List<Piece> GetPiecesThreatningKing(PieceTeam team)
    {
        List<Piece> threatPieces = new List<Piece>();

        switch (team)
        {
            case PieceTeam.White:
                threatPieces.AddRange(GetPiecesThatAreThreatningKing(whiteTeam));
                break;

            case PieceTeam.Black:
                threatPieces.AddRange(GetPiecesThatAreThreatningKing(blackTeam));
                break;
        }

        /* foreach (Piece threats in threatPieces)
        {
            Debug.Log($"Threat pieces are {threats.name}");
        }
        */
        return threatPieces;
    }

    private List<Piece> GetPiecesThatAreThreatningKing(List<Piece> pieces)
    {
        List<Piece> threatPieces = new List<Piece>();

        foreach (Piece piece in pieces)
        {
            foreach (Tile tile in piece.GetValidMoves())
            {
                if (tile.tilePiece is King)
                {
                    threatPieces.Add(piece);
                    continue;
                }
            }
        }

        return threatPieces;
    }

    public List<Piece> GetPiecesThatAreCheckingKing(PieceTeam team)
    {
        List<Piece> pieces = new List<Piece>();

        switch (team)
        {
            case PieceTeam.White:
                foreach (Piece piece in whiteTeam)
                {
                    if (piece.isCheckingKing)
                        pieces.Add(piece);
                }

                break;

            case PieceTeam.Black:
                foreach (Piece piece in blackTeam)
                {
                    if (piece.isCheckingKing)
                        pieces.Add(piece);
                }
                break;
        }

        return pieces;
    }

    /* public List<Tile> GetPathThatChecksKing(PieceTeam team)
    {
        List<Tile> paths = new List<Tile>();


    } */

    public List<Tile> GetAllPseudoValidMovesTeam(PieceTeam team)
    {
        List<Tile> moves = new List<Tile>();

        switch (team)
        {
            case PieceTeam.White:
                moves.AddRange(GetAllPiecesPseudoValidMoves(whiteTeam));
                break;

            case PieceTeam.Black:
                moves.AddRange(GetAllPiecesPseudoValidMoves(blackTeam));
                break;
        }

        return moves;
    }

    private List<Tile> GetAllPiecesPseudoValidMoves(List<Piece> pieces)
    {
        List<Tile> allMoves = new List<Tile>();

        foreach (Piece piece in pieces)
        {
            piece.currentTile.Skippable = true;

            piece.CheckKingOnSelection();
            allMoves.AddRange(piece.GetPseudoValidMoves());

            piece.currentTile.Skippable = false;
        }

        return allMoves;
    }



    public List<Tile> GetAllValidMovesTeam(PieceTeam team)
    {
        List<Tile> moves = new List<Tile>();

        switch (team)
        {
            case PieceTeam.White:
                moves.AddRange(GetAllPiecesValidMoves(whiteTeam));
                break;

            case PieceTeam.Black:
                moves.AddRange(GetAllPiecesValidMoves(blackTeam));
                break;
        }

        return moves;
    }

    private List<Tile> GetAllPiecesValidMoves(List<Piece> pieces)
    {
        List<Tile> allMoves = new List<Tile>();

        foreach (Piece piece in pieces)
        {
            if (piece is not King)
                allMoves.AddRange(piece.GetValidMoves());
        }

        return allMoves;
    }

    public static void KillPiece(Piece piece)
    {
        Instance.ServerKillPiece(piece);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerKillPiece(Piece piece)
    {
        RemovePiece(piece);
        ServerManager.Despawn(piece.gameObject);

        Debug.Log($"Killed {piece.name}");
    }

    [ObserversRpc]
    private void RemovePiece(Piece piece)
    {
        if (Instance.whiteTeam.Contains(piece))
            Instance.whiteTeam.Remove(piece);

        else if (Instance.blackTeam.Contains(piece))
            Instance.blackTeam.Remove(piece);
    }
}
