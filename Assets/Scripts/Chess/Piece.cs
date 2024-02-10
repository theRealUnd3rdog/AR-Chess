using System;
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

    // In the Piece class
    public abstract List<Tile> GetValidMoves();
    public abstract List<Tile> GetInvalidMoves();
}
