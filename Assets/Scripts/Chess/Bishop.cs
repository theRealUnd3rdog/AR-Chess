using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bishop : Piece
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
        List<Tile> validMoves = GetDiagonalTiles();

        foreach (Tile tile in validMoves)
        {
            //Debug.Log("Valid Moves: " + tile.name);
        }

        return validMoves;
    }

    public override List<Tile> GetPseudoValidMoves()
    {
        List<Tile> pseudoValidMoves = GetValidMoves();

        CalculatePseudoValidMoves(pseudoValidMoves);

        return pseudoValidMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        List<Tile> invalidMoves = GetDiagonalTiles(false);

        foreach (Tile tile in invalidMoves)
        {
            //Debug.Log("Invalid Moves: " + tile.name);
        }
        
        return invalidMoves;
    }
}
