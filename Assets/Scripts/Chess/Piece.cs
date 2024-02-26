using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

public abstract class Piece : MonoBehaviour
{
    public PieceType Type;
    public PieceTeam Team;
    public Tile currentTile;
    public int currentMove = 0;

    // In the Piece class
    public abstract List<Tile> GetValidMoves();
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
        GameManager.Instance.CurrentTeam = (team == PieceTeam.White) ? PieceTeam.Black : PieceTeam.White;
        TileManager.DeselectPiece();

        //Debug.Log("Team changed");
    }

    public virtual void MoveToTile(Tile tile)
    {
        // Define base behaviour
        this.transform.position = tile.transform.position;
        this.transform.SetParent(tile.transform);
        this.currentTile = tile;

        currentMove++;

        GameManager.NotifyMoveMade(Team);
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

    public virtual bool IsNotSameTeam(Piece piece)
    {
        return this.Team != piece.Team;
    }
}
