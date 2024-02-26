using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KingMoves
{
    public List<Tile> invalidMoves;
    public List<Piece> blockingPieces; 
}

public class King : Piece
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

        List<Tile> uniTiles = GetUniDirectionalTiles(true, 1);
        List<Tile> diagonalTiles = GetDiagonalTiles(true, 1);

        validMoves.AddRange(uniTiles);
        validMoves.AddRange(diagonalTiles);

        List<Tile> invalidMoves = PieceManager.Instance.GetAllValidMovesTeam(Team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White);

        foreach (Tile kingTile in invalidMoves)
        {
            if (validMoves.Contains(kingTile))
                validMoves.Remove(kingTile);

            Debug.Log("Invalid moves: " + kingTile.name);
        }

        foreach (Tile tile in validMoves)
        {
            //Debug.Log("Valid Moves: " + tile.name);
        }

        // 1)
        // Look through valid moves from opposite pieces
        // If opposite piece is on attack king after this piece is moved, this cannot be moved.

        // 2)
        // If opposite piece is already on attack king, only pieces that block that check can be moved.

        // 3)
        // If opposite piece is blocking all paths of king, you are checked, enemy has won

        return validMoves;
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
            Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
