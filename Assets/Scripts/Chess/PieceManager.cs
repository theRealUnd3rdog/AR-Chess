using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
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

        InstantiatePieces(_whitePieces);
        InstantiatePieces(_blackPieces);
    }

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
                    piece.currentTile = tile;
                    SetTeam(piece);
                }
                // Spawn the pawns after
                else
                {
                    // Get the tile
                    pieceToSpawn = pieces[pieces.Length - 1];

                    Piece piece = Instantiate(pieceToSpawn, tile.transform.position, Quaternion.identity);

                    piece.transform.SetParent(tile.transform);
                    piece.currentTile = tile;
                    SetTeam(piece);
                }
            }
        }
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
}
