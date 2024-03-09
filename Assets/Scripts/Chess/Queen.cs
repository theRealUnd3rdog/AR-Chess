using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        return validMoves;
    }

    public override List<Tile> GetPseudoValidMoves()
    {
        List<Tile> pseudoValidMoves = GetValidMoves();

        CalculatePseudoValidMoves(pseudoValidMoves);

        /* // Check if this team is checked
        if (GameManager.IsChecked(Team))
        {
            List<Tile> legalMoves = new List<Tile>();
            List<Piece> checkedPieces = PieceManager.Instance.GetPiecesThatAreCheckingKing(oppositeTeam);

            foreach (Piece piece in checkedPieces)
            {
                if (piece is Queen)
                {
                    List<Tile> oppositePieceMoves = piece.GetValidMoves();
                    int kingIndex = oppositePieceMoves.IndexOf(kingTile);

                    if (kingIndex != -1)
                    {
                        for (int i = kingIndex; i >= 0; i--)
                        {
                            legalMoves.Add(oppositePieceMoves[i]);
                        }
                    }

                    legalMoves.Add(piece.currentTile);

                    break;
                }
            }

            foreach (Tile tile in legalMoves)
            {
                Debug.Log("Pseudo valid moves: " + tile.name);
            }

            // Check if any element from pseudoValidMoves is contained in validMoves
            var commonMoves = pseudoValidMoves.Intersect(legalMoves).ToList();

            // If any common moves found, clear validMoves and add only common moves
            if (commonMoves.Any())
            {
                pseudoValidMoves.Clear();
                pseudoValidMoves.AddRange(commonMoves);

                return pseudoValidMoves;
            }

            legalMoves.Clear();
        } */
        // Even if not checked, check if king is in threat

        return pseudoValidMoves;
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
            //Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
