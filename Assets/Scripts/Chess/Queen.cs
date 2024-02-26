using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override List<Tile> GetValidMoves()
    {
        List<Tile> validMoves = new List<Tile>();

        List<Tile> uniTiles = GetUniDirectionalTiles();
        List<Tile> diagonalTiles = GetDiagonalTiles();

        validMoves.AddRange(uniTiles);
        validMoves.AddRange(diagonalTiles);

        foreach (Tile tile in validMoves)
        {
            Debug.Log("Valid Moves: " + tile.name);
        }

        return validMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        List<Tile> invalidMoves = new List<Tile>();

        List<Tile> uniTiles = GetUniDirectionalTiles(false);
        List<Tile> diagonalTiles = GetDiagonalTiles(false);

        invalidMoves.AddRange(uniTiles);
        invalidMoves.AddRange(diagonalTiles);

        foreach (Tile tile in invalidMoves)
        {
            Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
