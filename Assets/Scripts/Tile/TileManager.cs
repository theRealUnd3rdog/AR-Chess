using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public static bool IsTilePressed = false;
    public Material highlightMaterial;
    public Material invalidMaterial;
    public Material killMaterial;
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
}
