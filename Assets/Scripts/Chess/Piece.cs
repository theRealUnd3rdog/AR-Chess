using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Object;

public enum PieceType
{
    Bishop,
    King,
    Knight,
    Queen,
    Rook,
    Pawn,
}
public enum PieceTeam
{
    White,
    Black,
}

public struct LegalPieceMoves
{
    public Piece piece;
    public List<Tile> moves;

    // Constructor to initialize the moves list
    public LegalPieceMoves(Piece piece)
    {
        this.piece = piece;
        this.moves = new List<Tile>(); // Initialize the moves list
    }
}

public abstract class Piece : NetworkBehaviour
{
    public PieceType Type;
    public PieceTeam Team;
    public Tile currentTile;
    public int currentMove = 0;
    public bool isCheckingKing = false;
    //public bool isThreatToKing = false;

    // In the Piece class
    public abstract List<Tile> GetValidMoves();
    public abstract List<Tile> GetPseudoValidMoves();
    public abstract List<Tile> GetInvalidMoves();

    protected virtual void Start()
    {
        // Subscribe to the MoveMade event
        GameManager.MoveMade += OnMoveMade;
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        GameManager.MoveMade -= OnMoveMade;
    }

    // Event handler for the MoveMade event
    private void OnMoveMade(PieceTeam team)
    {
        // Toggle the current team
        GameManager.Instance.ServerSetCurrentTeam((team == PieceTeam.White) ? PieceTeam.Black : PieceTeam.White);
        TileManager.DeselectPiece();

        CheckKing(team);
        //Debug.Log("Team changed");
    }

    public void CheckKingOnSelection()
    {
        PieceTeam oppositeTeam = Team == PieceTeam.White ? PieceTeam.Black : PieceTeam.White;

        GameManager.SetThreatenPieces(Team, PieceManager.Instance.GetPiecesThreatningKing(oppositeTeam));
    }

    /* public void ThreatKing()
    {
        foreach (Tile tile in GetValidMoves())
        {
            //Debug.Log("Valid Moves: " + tile.name);

            if (tile.IsTileTaken() && tile.tilePiece is King)
            {
                if (tile.tilePiece.Team != this.Team)
                {
                    isThreatToKing = true;
                    break;
                }
            }
            else
            {
                isThreatToKing = false;
            }
        }
    } */

    /// <summary>
    /// This method changes the isCheckingKing bool depending on whether it is on the king or not
    /// </summary>
    public void CheckKing(PieceTeam team)
    {
        foreach (Tile tile in GetValidMoves())
        {
            //Debug.Log("Valid Moves: " + tile.name);

            if (tile.IsTileTaken() && tile.tilePiece is King)
            {
                if (tile.tilePiece.Team != this.Team)
                {
                    isCheckingKing = true;
                    break;
                }
            }
            else
            {
                isCheckingKing = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void ServerMoveToTile(Tile tile)
    {
        MoveToTile(tile);
    }
    
    [ObserversRpc]
    public virtual void MoveToTile(Tile tile)
    {
        // Define base behaviour
        this.transform.position = tile.transform.position;
        this.transform.SetParent(tile.transform);
        this.currentTile = tile;
        this.currentTile.Skippable = false;

        currentMove++;

        GameManager.NotifyMoveMade(Team);
        TileManager.NotifyMoveOnTilePiece(tile, this);
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void ServerMoveToTileNonNotify(Tile tile)
    {
        MoveToTileNonNotify(tile);
    }

    [ObserversRpc]
    public virtual void MoveToTileNonNotify(Tile tile)
    {
        this.transform.position = tile.transform.position;
        this.transform.SetParent(tile.transform);
        this.currentTile = tile;
        this.currentTile.Skippable = false;

        currentMove++;

        // Unsubscribe OnMoveMade
        GameManager.MoveMade -= OnMoveMade;
        CameraBehaviour.DisableCamera = true;

        CheckKing(Team);

        GameManager.NotifyMoveMade(Team);
        
        CameraBehaviour.DisableCamera = false;
        GameManager.MoveMade += OnMoveMade;
    }

    public virtual List<int> GetForwardIndexes(int times, int currentIndex)
    {
        List<int> indexes = new List<int>();
        int currentRow = int.Parse(currentTile.name.Substring(1));

        for (int i = 0; i < times; i++)
        {
            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;

            if (currentIndex < 0 || currentIndex > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[currentIndex];
            currentRow = int.Parse(newTile.name.Substring(1));

            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;

            int index = Team == PieceTeam.White ? ++currentIndex : --currentIndex;
            indexes.Add(index);
        }

        return indexes;
    }

    public virtual List<int> GetBackwardIndexes(int times, int currentIndex)
    {
        List<int> indexes = new List<int>();
        int currentRow = int.Parse(currentTile.name.Substring(1));

        for (int i = 0; i < times; i++)
        {
            if (currentRow == (Team == PieceTeam.White ? 1 : 8))
                break;

            /* if (currentIndex < 0 || currentIndex > TileManager.Instance.tiles.Length)
                break; */

            Tile newTile = TileManager.Instance.tiles[currentIndex];
            currentRow = int.Parse(newTile.name.Substring(1));

            if (currentRow == (Team == PieceTeam.White ? 1 : 8))
                break;

            int index = Team == PieceTeam.White ? --currentIndex : ++currentIndex;
            indexes.Add(index);
        }

        return indexes;
    }

    public virtual List<int> GetRightIndexes(int times, int currentIndex)
    {
        List<int> indexes = new List<int>();
        int currentRow = int.Parse(currentTile.name.Substring(1));
        char currentColumn = currentTile.name[0];

        for (int i = 0; i < times; i++)
        {
            if (currentColumn == (Team == PieceTeam.White ?  'H' : 'A'))
                break;

            int index = Team == PieceTeam.White ? currentIndex += 8 : currentIndex -= 8;

            if (index < 0 || index > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[index];
            currentColumn = newTile.name[0];

            if (newTile.name == (Team == PieceTeam.White ? ('H' + currentRow).ToString() : ('A' + currentRow).ToString()))
                break;

            indexes.Add(index);
        }

        return indexes;
    }

    public virtual List<int> GetLeftIndexes(int times, int currentIndex)
    {
        List<int> indexes = new List<int>();
        int currentRow = int.Parse(currentTile.name.Substring(1));
        char currentColumn = currentTile.name[0];

        for (int i = 0; i < times; i++)
        {
            if (currentColumn == (Team == PieceTeam.White ?  'A' : 'H'))
                break;

            int index = Team == PieceTeam.White ? currentIndex -= 8 : currentIndex += 8;

            if (index < 0 || index > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[index];
            currentColumn = newTile.name[0];

            if (newTile.name == (Team == PieceTeam.White ? ('A' + currentRow).ToString() : ('H' + currentRow).ToString()))
                break;
                
            indexes.Add(index);
        }

        return indexes;
    }

    public virtual List<int> GetDFrontRightIndexes(int times, Tile tileToStart = null)
    {
        if (tileToStart == null)
            tileToStart = currentTile;

        List<int> indexes = GetRightIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart));
        List<int> diagonalIndexes = new List<int>();

        int currentRow = int.Parse(tileToStart.name.Substring(1));
        char currentColumn = tileToStart.name[0];

        for (int i = 0; i < indexes.Count; i++)
        {
            if (currentColumn == (Team == PieceTeam.White ? 'H' : 'A'))
                break;

            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;

            int newIndex = Team == PieceTeam.White ? indexes[i] + i + 1 : indexes[i] - i - 1;

            if (newIndex < 0 || newIndex > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[newIndex];

            currentRow = int.Parse(newTile.name.Substring(1));

            diagonalIndexes.Add(newIndex);

            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;
        }

        return diagonalIndexes;
    }

    public virtual List<int> GetDFrontLeftIndexes(int times, Tile tileToStart = null)
    {
        if (tileToStart == null)
            tileToStart = currentTile;

        List<int> indexes = GetLeftIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart));
        List<int> diagonalIndexes = new List<int>();

        int currentRow = int.Parse(tileToStart.name.Substring(1));
        char currentColumn = tileToStart.name[0];

        for (int i = 0; i < indexes.Count; i++)
        {
            if (currentColumn == (Team == PieceTeam.White ? 'A' : 'H'))
                break;

            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;

            int newIndex = Team == PieceTeam.White ? indexes[i] + i + 1 : indexes[i] - i - 1;

            if (newIndex < 0 || newIndex > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[newIndex];

            currentRow = int.Parse(newTile.name.Substring(1));

            diagonalIndexes.Add(newIndex);

            if (currentRow == (Team == PieceTeam.White ? 8 : 1))
                break;
        }

        return diagonalIndexes;
    }

    public virtual List<int> GetDBackRightIndexes(int times, Tile tileToStart = null)
    {
        if (tileToStart == null)
            tileToStart = currentTile;

        List<int> indexes = GetRightIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart));
        List<int> diagonalIndexes = new List<int>();

        int currentRow = int.Parse(tileToStart.name.Substring(1));
        char currentColumn = tileToStart.name[0];

        for (int i = 0; i < indexes.Count; i++)
        {
            if (currentRow == (Team == PieceTeam.White ? 1 : 8))
                break;

            int newIndex = Team == PieceTeam.White ? indexes[i] - i - 1 : indexes[i] + i + 1;
            if (newIndex < 0 || newIndex > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[newIndex];

            currentRow = int.Parse(newTile.name.Substring(1));
            currentColumn = newTile.name[0];

            diagonalIndexes.Add(newIndex);

            if (currentRow == (Team == PieceTeam.White ? 1 : 8) || currentColumn == (Team == PieceTeam.White ? 'H' : 'A'))
                break;
        }

        return diagonalIndexes;
    }

    public virtual List<int> GetDBackLeftIndexes(int times, Tile tileToStart = null)
    {
        if (tileToStart == null)
            tileToStart = currentTile;

        List<int> indexes = GetLeftIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart));
        List<int> diagonalIndexes = new List<int>();

        int currentRow = int.Parse(tileToStart.name.Substring(1));
        char currentColumn = tileToStart.name[0];

        for (int i = 0; i < indexes.Count; i++)
        {
            if (currentRow == (Team == PieceTeam.White ? 1 : 8))
                break;

            int newIndex = Team == PieceTeam.White ? indexes[i] - i - 1 : indexes[i] + i + 1;
            if (newIndex < 0 || newIndex > TileManager.Instance.tiles.Length)
                break;

            Tile newTile = TileManager.Instance.tiles[newIndex];

            currentRow = int.Parse(newTile.name.Substring(1));
            currentColumn = newTile.name[0];

            diagonalIndexes.Add(newIndex);

            if (currentRow == (Team == PieceTeam.White ? 1 : 8) || currentColumn == (Team == PieceTeam.White ? 'H' : 'A'))
                break;
        }

        return diagonalIndexes;
    }

    public List<Tile> GetUniDirectionalTiles(bool valid = true, int times = 8, Tile tileToStart = null)
    {
        List<Tile> uniTiles = new List<Tile>();

        if (tileToStart == null)
            tileToStart = currentTile;

        foreach (int index in GetForwardIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart)))
        {
            Tile forwardTile = TileManager.Instance.tiles[index];
            
            if (forwardTile.IsTileTaken())
            {
                if (this.Team != forwardTile.tilePiece.Team && valid)
                {
                    uniTiles.Add(forwardTile);
                }
                else

                if (this.Team == forwardTile.tilePiece.Team && !valid)
                {
                    uniTiles.Add(forwardTile);
                    break;
                }

                break;
            }

            if (valid)
                uniTiles.Add(forwardTile);
        }

        foreach (int index in GetBackwardIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart)))
        {
            Tile backwardTile = TileManager.Instance.tiles[index];
            
            if (backwardTile.IsTileTaken())
            {
                if (this.Team != backwardTile.tilePiece.Team && valid)
                {
                    uniTiles.Add(backwardTile);
                }

                if (this.Team == backwardTile.tilePiece.Team && !valid)
                {
                    uniTiles.Add(backwardTile);
                    break;
                }
                
                break;
            }

            if (valid)
                uniTiles.Add(backwardTile);
        }

        foreach (int index in GetRightIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart)))
        {
            Tile rightTile = TileManager.Instance.tiles[index];
            
            if (rightTile.IsTileTaken())
            {
                if (this.Team != rightTile.tilePiece.Team && valid)
                {
                    uniTiles.Add(rightTile);
                }

                if (this.Team == rightTile.tilePiece.Team && !valid)
                {
                    uniTiles.Add(rightTile);
                    break;
                }
                
                break;
            }

            if (valid)
                uniTiles.Add(rightTile);
        }

        foreach (int index in GetLeftIndexes(times, Array.IndexOf(TileManager.Instance.tiles, tileToStart)))
        {
            Tile leftTile = TileManager.Instance.tiles[index];
            
            if (leftTile.IsTileTaken())
            {
                if (this.Team != leftTile.tilePiece.Team && valid)
                {
                    uniTiles.Add(leftTile);
                }

                if (this.Team == leftTile.tilePiece.Team && !valid)
                {
                    uniTiles.Add(leftTile);
                    break;
                }
                
                break;
            }

            if (valid)
                uniTiles.Add(leftTile);
        }

        return uniTiles;
    }

    public List<Tile> GetDiagonalTiles(bool valid = true, int times = 8, Tile tileToStart = null)
    {
        List<Tile> diagonalTiles = new List<Tile>();

        if (tileToStart == null)
            tileToStart = currentTile;

        foreach (int index in GetDFrontLeftIndexes(times, tileToStart))
        {
            Tile dForwardLeftTile = TileManager.Instance.tiles[index];

            if (dForwardLeftTile.IsTileTaken())
            {
                if (this.Team != dForwardLeftTile.tilePiece.Team && valid)
                {
                    diagonalTiles.Add(dForwardLeftTile);
                }

                if (this.Team == dForwardLeftTile.tilePiece.Team && !valid)
                {
                    diagonalTiles.Add(dForwardLeftTile);
                    break;
                }
                
                break;
            }

            if (valid)
                diagonalTiles.Add(dForwardLeftTile);
        }

        foreach (int index in GetDFrontRightIndexes(times, tileToStart))
        {
            Tile dForwardRightTile = TileManager.Instance.tiles[index];

            if (dForwardRightTile.IsTileTaken())
            {
                if (this.Team != dForwardRightTile.tilePiece.Team && valid)
                {
                    diagonalTiles.Add(dForwardRightTile);
                }

                if (this.Team == dForwardRightTile.tilePiece.Team && !valid)
                {
                    diagonalTiles.Add(dForwardRightTile);
                    break;
                }
                
                break;
            }

            if (valid)
                diagonalTiles.Add(dForwardRightTile);
        }

        foreach (int index in GetDBackLeftIndexes(times, tileToStart))
        {
            Tile dBackwardLeftTile = TileManager.Instance.tiles[index];

            if (dBackwardLeftTile.IsTileTaken())
            {
                if (this.Team != dBackwardLeftTile.tilePiece.Team && valid)
                {
                    diagonalTiles.Add(dBackwardLeftTile);
                }

                if (this.Team == dBackwardLeftTile.tilePiece.Team && !valid)
                {
                    diagonalTiles.Add(dBackwardLeftTile);
                    break;
                }
                
                break;
            }

            if (valid)
                diagonalTiles.Add(dBackwardLeftTile);
        }

        foreach (int index in GetDBackRightIndexes(times, tileToStart))
        {
            Tile dBackwardRightTile = TileManager.Instance.tiles[index];

            if (dBackwardRightTile.IsTileTaken())
            {
                if (this.Team != dBackwardRightTile.tilePiece.Team && valid)
                {
                    diagonalTiles.Add(dBackwardRightTile);
                }

                if (this.Team == dBackwardRightTile.tilePiece.Team && !valid)
                {
                    diagonalTiles.Add(dBackwardRightTile);
                    break;
                }
                
                break;
            }

            if (valid)
                diagonalTiles.Add(dBackwardRightTile);
        }

        return diagonalTiles;
    }

    public virtual List<Tile> CalculatePseudoValidMoves(List<Tile> pseudoValidMoves)
    {
        Tile kingTile = PieceManager.Instance.GetPieceCurrentTile<King>(Team);

        List<Piece> threatningPieces = GameManager.GetThreatPieces(Team);

        List<LegalPieceMoves> legalPieceMoves = new List<LegalPieceMoves>(); 

        // If there are any threatning pieces king is in threat
        if (threatningPieces.Count > 0)
        {
            List<Tile> legalMoves = new List<Tile>();

            foreach (Piece piece in threatningPieces)
            {
                LegalPieceMoves pieceMoves = new LegalPieceMoves(piece);

                List<Tile> oppositePieceMoves = piece.GetValidMoves();
                int kingIndex = oppositePieceMoves.IndexOf(kingTile);

                legalMoves.Add(piece.currentTile);

                pieceMoves.piece = piece;
                pieceMoves.moves.Add(piece.currentTile);

                if (kingIndex != -1)
                {
                    for (int i = kingIndex; i >= 0; i--)
                    {
                        legalMoves.Add(oppositePieceMoves[i]);
                        pieceMoves.moves.Add(oppositePieceMoves[i]);

                        if (piece.IsTileNear(oppositePieceMoves[i]))
                        {
                            break;
                        }   
                            
                    }
                }

                legalPieceMoves.Add(pieceMoves);
            }

            bool blocksAllPaths = true;

            // Go through each piece and their legal moves and see if all pieces can be blocked against
            for (int i = 0; i < legalPieceMoves.Count; i++)
            {
                var commonMoves = pseudoValidMoves.Intersect(legalPieceMoves[i].moves).ToList();

                if (legalPieceMoves.Count > 1 && legalPieceMoves[i].piece.isCheckingKing)
                {
                    blocksAllPaths = false;
                    break;
                }

                if (commonMoves.Any())
                    continue;
                else
                {
                    blocksAllPaths = false;
                    break;
                }
            }

            if (blocksAllPaths)
            {
                // Check if any element from pseudoValidMoves is contained in validMoves
                var commonMoves = pseudoValidMoves.Intersect(legalMoves).ToList();

                // If any common moves found, clear validMoves and add only common moves
                if (commonMoves.Any())
                {
                    pseudoValidMoves.Clear();
                    pseudoValidMoves.AddRange(commonMoves);

                    return pseudoValidMoves;
                }
            }
            else
            {
                pseudoValidMoves.Clear();
            }
        }

        return pseudoValidMoves;
    }

    public virtual bool IsTileNear(Tile tileToCheck)
    {
        List<Tile> moves = new List<Tile>();

        List<Tile> uniTiles = GetUniDirectionalTiles(true, 1);
        List<Tile> diagonalTiles = GetDiagonalTiles(true, 1);

        moves.AddRange(uniTiles);
        moves.AddRange(diagonalTiles);

        if (moves.Contains(tileToCheck))
            return true;
        else
            return false;
    }

    public virtual bool IsNotSameTeam(Piece piece)
    {
        return this.Team != piece.Team;
    }
}
