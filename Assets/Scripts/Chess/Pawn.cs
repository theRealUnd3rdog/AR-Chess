using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
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

        // First move of pawn contains 2 steps, after the first move made, default it to 1
        int value = this.currentMove <= 0 ? 2 : 1;

        foreach (int index in GetForwardIndexes(value, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile forwardTile = TileManager.Instance.tiles[index];
            
            // Check if tile is taken by piece first
            if (forwardTile.IsTileTaken())
                break;

            validMoves.Add(forwardTile);     
        }

        foreach (int index in GetDFrontLeftIndexes(1))
        {
            Tile dForwardLeftTile = TileManager.Instance.tiles[index];

            if (dForwardLeftTile.IsTileTaken())
            {
                if (this.Team != dForwardLeftTile.tilePiece.Team)
                {
                    validMoves.Add(dForwardLeftTile);
                }
                
                break;
            }
        }

        foreach (int index in GetDFrontRightIndexes(1))
        {
            Tile dForwardRightTile = TileManager.Instance.tiles[index];

            if (dForwardRightTile.IsTileTaken())
            {
                if (this.Team != dForwardRightTile.tilePiece.Team)
                {
                    validMoves.Add(dForwardRightTile);
                }
                
                break;
            }
                
        }

        foreach (Tile tile in validMoves)
        {
            Debug.Log("Valid Moves: " + tile.name);
        }
        
        return validMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        List<Tile> invalidMoves = new List<Tile>();

        int value = this.currentMove <= 0 ? 2 : 1;

        foreach (int index in GetForwardIndexes(value, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile forwardTile = TileManager.Instance.tiles[index];

            if (forwardTile.IsTileTaken())
            {
                if (this.Team == forwardTile.tilePiece.Team)
                {
                    invalidMoves.Add(forwardTile);
                }
                
                break;
            }
        }

        foreach (Tile tile in invalidMoves)
        {
            Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
