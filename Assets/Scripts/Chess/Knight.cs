using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knight : Piece
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

        // Get right forward
        foreach (int rightIndex in GetRightIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> forwardIndexes = GetForwardIndexes(2, rightIndex);
            int count = 0;

            foreach (int forwardIndex in forwardIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile forwardTile = TileManager.Instance.tiles[forwardIndex];
            
                    if (forwardTile.IsTileTaken())
                    {
                        if (this.Team != forwardTile.tilePiece.Team)
                        {
                            validMoves.Add(forwardTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(forwardTile);    
                }
            }
        }
        
        // Get left forward
        foreach (int leftIndex in GetLeftIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> forwardIndexes = GetForwardIndexes(2, leftIndex);
            int count = 0;

            foreach (int forwardIndex in forwardIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile forwardTile = TileManager.Instance.tiles[forwardIndex];
            
                    if (forwardTile.IsTileTaken())
                    {
                        if (this.Team != forwardTile.tilePiece.Team)
                        {
                            validMoves.Add(forwardTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(forwardTile);    
                }
            }
        }

        // Get right back
        foreach (int rightIndex in GetRightIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> forwardIndexes = GetBackwardIndexes(2, rightIndex);
            int count = 0;

            foreach (int forwardIndex in forwardIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile forwardTile = TileManager.Instance.tiles[forwardIndex];
            
                    if (forwardTile.IsTileTaken())
                    {
                        if (this.Team != forwardTile.tilePiece.Team)
                        {
                            validMoves.Add(forwardTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(forwardTile);    
                }
            }
        }

        // Get left back
        foreach (int leftIndex in GetLeftIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> forwardIndexes = GetBackwardIndexes(2, leftIndex);
            int count = 0;

            foreach (int forwardIndex in forwardIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile forwardTile = TileManager.Instance.tiles[forwardIndex];
            
                    if (forwardTile.IsTileTaken())
                    {
                        if (this.Team != forwardTile.tilePiece.Team)
                        {
                            validMoves.Add(forwardTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(forwardTile);    
                }
            }
        }

        // Get forward right right
        foreach (int forwardIndex in GetForwardIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> rightIndexes = GetRightIndexes(2, forwardIndex);
            int count = 0;

            foreach (int rightIndex in rightIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile rightTile = TileManager.Instance.tiles[rightIndex];
            
                    if (rightTile.IsTileTaken())
                    {
                        if (this.Team != rightTile.tilePiece.Team)
                        {
                            validMoves.Add(rightTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(rightTile);    
                }
            }
        }

        // Get forward left left
        foreach (int forwardIndex in GetForwardIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> leftIndexes = GetLeftIndexes(2, forwardIndex);
            int count = 0;

            foreach (int leftIndex in leftIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile rightTile = TileManager.Instance.tiles[leftIndex];
            
                    if (rightTile.IsTileTaken())
                    {
                        if (this.Team != rightTile.tilePiece.Team)
                        {
                            validMoves.Add(rightTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(rightTile);    
                }
            }
        }

        // Get backward right right
        foreach (int backwardIndex in GetBackwardIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> rightIndexes = GetRightIndexes(2, backwardIndex);
            int count = 0;

            foreach (int rightIndex in rightIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile rightTile = TileManager.Instance.tiles[rightIndex];
            
                    if (rightTile.IsTileTaken())
                    {
                        if (this.Team != rightTile.tilePiece.Team)
                        {
                            validMoves.Add(rightTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(rightTile);    
                }
            }
        }

        // Get backward left left
        foreach (int backwardIndex in GetBackwardIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            List<int> leftIndexes = GetLeftIndexes(2, backwardIndex);
            int count = 0;

            foreach (int leftIndex in leftIndexes)
            {
                count++;

                // If the count is more than 1 then add it to the list
                if (count > 1)
                {
                    Tile rightTile = TileManager.Instance.tiles[leftIndex];
            
                    if (rightTile.IsTileTaken())
                    {
                        if (this.Team != rightTile.tilePiece.Team)
                        {
                            validMoves.Add(rightTile);
                        }
                        
                        break;
                    }

                    validMoves.Add(rightTile);    
                }
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
        throw new System.NotImplementedException();
    }
}
