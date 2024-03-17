using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public Material highlightMaterial;
    public Material invalidMaterial;
    public Material killMaterial;
    public Tile currentTileSelected;
    public Piece currentPieceSelected;
    public List<Tile> tilesToMove = new List<Tile>();
    public Tile[] tiles;

    public static event Action<Tile, Piece> MoveOnTile;

    private void Awake()
    {
        Instance = this;
    }

    public static void NotifyMoveOnTilePiece(Tile tile, Piece piece)
    {
        MoveOnTile?.Invoke(tile, piece);

        Debug.Log($"{piece.name} moved to {tile.name}");
    }

    public static Tile GetTileByName(string tileName)
    {
        return Instance.tiles.FirstOrDefault(tile => tile.gameObject.name == tileName);
    }

   public static List<Tile> GetTilesByNames(string[] tileNames)
    {
        // Use LINQ to filter tiles based on the provided names
        return Instance.tiles.Where(tile => tileNames.Contains(tile.gameObject.name)).ToList();
    } 

    public void UnhighlightAllTiles()
    {
        foreach (Tile tile in tiles)
            tile.Unhighlight();
    }

    public static void DeselectPiece()
    {
        if (Instance.currentTileSelected != null)
            Instance.currentTileSelected.Skippable = false;

        Instance.currentTileSelected = null;
        Instance.currentPieceSelected = null;
        
        Instance.UnhighlightAllTiles();
    }
    

    public static bool IfWithinBounds(int index)
    {
        if (index >= 0 && index < Instance.tiles.Length)
            return true;

        return false;
    }
}
