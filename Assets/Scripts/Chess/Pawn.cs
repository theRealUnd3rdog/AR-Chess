using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
    public bool enPassent = false;
    public Tile enPassentTile;
    private Pawn _enemyEnPassentPawn;
    private Tile _enPassentKillTile;

    protected override void Start()
    {
        base.Start();
        
        TileManager.MoveOnTile += KillEnPassent;
        TileManager.MoveOnTile += ChangeEnPassent;
        

        TileManager.MoveOnTile += NotifyIfOnPromotionTile;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        TileManager.MoveOnTile -= KillEnPassent;
        TileManager.MoveOnTile -= ChangeEnPassent;

        TileManager.MoveOnTile -= NotifyIfOnPromotionTile;
    }

    private void ChangeEnPassent(Tile tile, Piece piece)
    {
        if (enPassent)
            enPassent = false;

        if (currentMove > 1)
            return;

        if (piece == this && !enPassent)
        {
            if (tile == enPassentTile)
                enPassent = true;
        }
    }

    private void NotifyIfOnPromotionTile(Tile tile, Piece piece)
    {
        bool flag = GameManager.IsPawnOnPromotionTile(this);

        if (flag)
        {
            Debug.LogWarning($"{this.name} is on a promotion tile");

            UIManager.Instance.pawnToPromote = this;
            UIManager.Instance.Promotion = true;
        }
    }
    
    private void KillEnPassent(Tile tile, Piece piece)
    {
        if (piece == this)
        {
            if (_enemyEnPassentPawn != null && tile == _enPassentKillTile)
            {
                //if (_enemyEnPassentPawn.enPassent)
                PieceManager.KillPiece(_enemyEnPassentPawn);
            }
        }
    }

    private Pawn GetEnpassentablePawnRight()
    {
        Pawn pawn = null;

        foreach (int index in GetRightIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile rightTile = TileManager.Instance.tiles[index];

            // check if tile is taken
            if (rightTile.IsTileTaken())
            {
                // check if piece is not same team
                if (this.Team != rightTile.tilePiece.Team)
                {
                    // get enemy pawn
                    Pawn enemyPawn = rightTile.tilePiece.GetComponent<Pawn>();

                    if (enemyPawn != null)
                    {
                        Debug.Log("Found enpassent enemy: " + enemyPawn.name);

                        if (enemyPawn.enPassent)
                            return enemyPawn;
                    }
                }
            } 
        }

        return pawn;
    }

    private Pawn GetEnpassentablePawnLeft()
    {
        Pawn pawn = null;

        foreach (int index in GetLeftIndexes(1, Array.IndexOf(TileManager.Instance.tiles, currentTile)))
        {
            Tile leftTile = TileManager.Instance.tiles[index];

            // check if tile is taken
            if (leftTile.IsTileTaken())
            {
                // check if piece is not same team
                if (this.Team != leftTile.tilePiece.Team)
                {
                    // get enemy pawn
                    Pawn enemyPawn = leftTile.tilePiece.GetComponent<Pawn>();

                    if (enemyPawn != null)
                    {
                        Debug.Log("Found enpassent enemy: " + enemyPawn.name);
                        if (enemyPawn.enPassent)
                            return enemyPawn;
                    }
                }
            } 
        }

        return pawn;
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

            if (value == 2)
            {
                enPassentTile = forwardTile;
            }

            validMoves.Add(forwardTile);
        }

        foreach (int index in GetDFrontLeftIndexes(1))
        {
            Tile dForwardLeftTile = TileManager.Instance.tiles[index];

            Pawn enemyPawn = GetEnpassentablePawnLeft();

            if (enemyPawn != null)
            {
                if (enemyPawn.enPassent)
                {
                    validMoves.Add(dForwardLeftTile);
                    _enemyEnPassentPawn = enemyPawn;
                    _enPassentKillTile = dForwardLeftTile;

                    break;
                }
            }

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

            Pawn enemyPawn = GetEnpassentablePawnRight();

            if (enemyPawn != null)
            {
                if (enemyPawn.enPassent)
                {
                    validMoves.Add(dForwardRightTile);
                    _enemyEnPassentPawn = enemyPawn;

                    _enPassentKillTile = dForwardRightTile;

                    break;
                }
            }

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
            //Debug.Log("Invalid Moves: " + tile.name);
        }

        return invalidMoves;
    }
}
