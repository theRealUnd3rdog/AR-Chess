using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public Material highlightMaterial;
    public Material invalidMaterial;
    public Material killMaterial;
    public Tile currentTileSelected;
    public Piece currentPieceSelected;
    public Tile[] tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void UnhighlightAllTiles()
    {
        foreach (Tile tile in tiles)
            tile.Unhighlight();
    }

    public static void DeselectPiece()
    {
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
