using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
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
        List<Tile> validMoves = GetUniDirectionalTiles();
        
        foreach (Tile tile in validMoves)
        {
            //Debug.Log("Valid Moves: " + tile.name);

            List<Tile> uniTiles = GetUniDirectionalTiles(true, 1, tile);
            List<Tile> diagonalTiles = GetDiagonalTiles(true, 1, tile);

            string uniName = "";
            string diagName = "";

            foreach (Tile uni in uniTiles)
            {
                if (uni.tilePiece != null)
                {
                    if (uni.tilePiece is King)
                    {
                        Debug.Log($"King at {uni.name}, cannot move to {tile.name}");
                    }
                }

                uniName += " " + uni.name;
            }

            foreach (Tile diag in diagonalTiles)
            {
                if (diag.tilePiece != null)
                {
                    if (diag.tilePiece is King)
                    {
                        Debug.Log($"King at {diag.name}, cannot move to {tile.name}");
                    }
                }

                diagName += " " + diag.name;
            }

            Debug.Log($"{tile.name}: {uniName} + {diagName}");
        }

        return validMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        List<Tile> invalidMoves = GetUniDirectionalTiles(false);

        foreach (Tile tile in invalidMoves)
        {
            Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
