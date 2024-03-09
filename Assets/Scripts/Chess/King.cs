using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    [System.Serializable]
    public struct Castle
    {
        public Tile kingTile;
        public Tile rookTile;
        public RookSide side;
        public bool canCastle;
    }

    public Castle[] castles = new Castle[2];

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public List<Tile> GetCastleTiles()
    {
        List<Tile> castleTiles = new List<Tile>();
        
        // initialize a count so you can only get the second tile
        int value = 2;
        int count = 1;

        foreach (int index in GetRightIndexes(value, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile rightTile = TileManager.Instance.tiles[index];
            
            if (rightTile.IsTileTaken())
            {
                break;
            }

            if (count == 1)
            {
                castles[0].rookTile = rightTile;
            }

            // Get second tile
            if (count == value)
            {
                castleTiles.Add(rightTile);

                castles[0].kingTile = rightTile;
                castles[0].side = RookSide.Right;
            }

            count++;
        }

        count = 1;

        foreach (int index in GetLeftIndexes(value, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile leftTile = TileManager.Instance.tiles[index];
            
            if (leftTile.IsTileTaken())
            {
                break;
            }

            if (count == 1)
            {
                castles[1].rookTile = leftTile;
            }

            // Get second tile
            if (count == value)
            {
                castleTiles.Add(leftTile);

                castles[1].kingTile = leftTile;
                castles[1].side = RookSide.Left;
            }

            count++;
        }

        return castleTiles;
    }

    public override List<Tile> GetValidMoves()
    {
        List<Tile> validMoves = new List<Tile>();

        List<Tile> uniTiles = GetUniDirectionalTiles(true, 1);
        List<Tile> diagonalTiles = GetDiagonalTiles(true, 1);

        validMoves.AddRange(uniTiles);
        validMoves.AddRange(diagonalTiles);

        return validMoves;
    }

    public override List<Tile> GetPseudoValidMoves()
    {
        List<Tile> pseudoValidMoves = GetValidMoves();

        List<Tile> invalidMoves = PieceManager.Instance.GetAllValidMovesTeam(Team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White);

        foreach (Tile kingTile in invalidMoves)
        {
            if (pseudoValidMoves.Contains(kingTile))
                pseudoValidMoves.Remove(kingTile);

            //Debug.Log("Invalid moves: " + kingTile.name);
        }

        foreach (Tile tile in pseudoValidMoves)
        {
            //Debug.Log("Valid Moves: " + tile.name);
        }

        if (currentMove <= 0)
            pseudoValidMoves.AddRange(GetCastleTiles());

        return pseudoValidMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        List<Tile> invalidMoves = new List<Tile>();

        List<Tile> uniTiles = GetUniDirectionalTiles(false, 1);
        List<Tile> diagonalTiles = GetDiagonalTiles(false, 1);

        invalidMoves.AddRange(uniTiles);
        invalidMoves.AddRange(diagonalTiles);

        foreach (Tile tile in invalidMoves)
        {
            //Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
