using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Castle GetCastle(RookSide side)
    {
        Castle castle = default;

        foreach (Castle c in castles)
        {
            if (side == c.side)
                castle = c;
        }

        return castle;
    }

    public void ChangeCastling(RookSide side, bool castle)
    {
        for (int i = 0; i < castles.Length; i++)
        {
            if (side == castles[i].side)
            {
                castles[i].canCastle = castle;

                break;
            }
        }
    }

    public void StoreCastle(RookSide side, Tile king, Tile rook, bool castle)
    {
        for (int i = 0; i < castles.Length; i++)
        {
            if (side == castles[i].side)
            {
                castles[i].kingTile = king;
                castles[i].rookTile = rook;
                castles[i].canCastle = castle;

                break;
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        // Initialize castle sides
        castles[0].side = RookSide.Left;
        castles[1].side = RookSide.Right;

        TileManager.MoveOnTile += StartCastle;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        TileManager.MoveOnTile -= StartCastle;
    }

    public void StartCastle(Tile tile, Piece piece)
    {
        // If piece is king
        if (piece == this)
        {
            // check side and castle
            Castle leftCastle = GetCastle(RookSide.Left);
            Castle rightCastle = GetCastle(RookSide.Right);

            if (leftCastle.kingTile == tile)
            {
                if (leftCastle.canCastle)
                {
                    Rook rook = PieceManager.Instance.GetRook(Team, RookSide.Left);
                    rook.MoveToTileNonNotify(leftCastle.rookTile);

                    Debug.Log("Rook moved");
                }
            }
            else if (rightCastle.kingTile == tile)
            {
                if (rightCastle.canCastle)
                {
                    Rook rook = PieceManager.Instance.GetRook(Team, RookSide.Right);
                    rook.MoveToTileNonNotify(rightCastle.rookTile);
                }
            }
        }
    }

    public List<Tile> GetCastleTiles(RookSide side)
    {
        List<Tile> castleTiles = new List<Tile>();

        int indexCount = 1;
        int times = 2;

        List<int> indexes = side == RookSide.Right ? 
                            GetRightIndexes(times + 1, Array.IndexOf(TileManager.Instance.tiles, currentTile)) :
                            GetLeftIndexes(times + 1, Array.IndexOf(TileManager.Instance.tiles, currentTile));

        Tile kingTile = null;
        Tile rookTile = null;
        bool castling = false;

        // Reset the castle
        StoreCastle(side, kingTile, rookTile, castling);

        foreach (int index in indexes)
        {
            Tile tile = TileManager.Instance.tiles[index];

            if (tile.IsTileTaken())
                break;

            if (indexCount != times + 1)
                castleTiles.Add(tile);

            if (indexCount == 1)
                rookTile = tile;

            if (indexCount == 2)
                kingTile = tile;

            int dependantCount = side == RookSide.Right ? 
                                        (Team == PieceTeam.White ? times : times + 1) :
                                        (Team == PieceTeam.White ? times + 1 : times);

            if (indexCount == dependantCount)
            {
                Rook rook = PieceManager.Instance.GetRook(Team, side);

                if (rook != null)
                {
                    if (rook.IsTileNear(tile) && rook.currentMove <= 0)
                    {
                        castling = true;
                        StoreCastle(side, kingTile, rookTile, castling);
                    }
                }
            }

            indexCount++;
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

        List<Tile> leftCastleTiles = GetCastleTiles(RookSide.Left);
        List<Tile> rightCastleTiles = GetCastleTiles(RookSide.Right);

        List<Tile> invalidMoves = PieceManager.Instance.GetAllValidMovesTeam(Team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White);

        foreach (Tile kingTile in invalidMoves)
        {
            if (pseudoValidMoves.Contains(kingTile))
                pseudoValidMoves.Remove(kingTile);
            
            // if it contains any tiles in a castle side, then castling on that side is false
            if (leftCastleTiles.Contains(kingTile))
                ChangeCastling(RookSide.Left, false);

            if (rightCastleTiles.Contains(kingTile))
                ChangeCastling(RookSide.Right, false);
        }

        Castle leftCastle = GetCastle(RookSide.Left);
        Castle rightCastle = GetCastle(RookSide.Right);

        if (currentMove <= 0)
        {
            if (leftCastle.canCastle)
                pseudoValidMoves.AddRange(leftCastleTiles);
            
            if (rightCastle.canCastle)
                pseudoValidMoves.AddRange(rightCastleTiles);
        }   
        else
        {
            ChangeCastling(RookSide.Left, false);
            ChangeCastling(RookSide.Right, false);
        }

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
