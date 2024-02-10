using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<Tile> GetValidMoves()
    {
        List<Tile> validMoves = new List<Tile>();

        // Get current tile coordinates
        int currentIndex = Array.IndexOf(TileManager.Instance.tiles, currentTile);
        int currentX = currentIndex % 8; // Column index
        int currentY = currentIndex / 8; // Row index

        // Check all tiles in the same row (horizontal)
        for (int x = 0; x < 8; x++)
        {
            if (x != currentX)
            {
                Tile tileToAdd = TileManager.Instance.tiles[currentY * 8 + x];

                if (!tileToAdd.IsTileTaken())
                    validMoves.Add(tileToAdd);
            }
        }

        // Check all tiles in the same column (vertical)
        for (int y = 0; y < 8; y++)
        {
            if (y != currentY)
            {
                Tile tileToAdd = TileManager.Instance.tiles[y * 8 + currentX];

                if (!tileToAdd.IsTileTaken())
                    validMoves.Add(tileToAdd);
            }
        }

        return validMoves;
    }

    public override List<Tile> GetInvalidMoves()
    {
        throw new NotImplementedException();
    }
}
