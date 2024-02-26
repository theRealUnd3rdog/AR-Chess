using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece tilePiece {private set; get;}
    private Material _highlightMaterial;
    private Material _invalidMaterial;
    private Material _killMaterial;
    private Material _originalMaterial;
    private Renderer _rend;

    private TileChildEventHandler _childEventHandler;

    private void Awake()
    {
        _childEventHandler = GetComponent<TileChildEventHandler>();

        _childEventHandler.onChildAdded.AddListener(StorePiece);
        _childEventHandler.onChildRemoved.AddListener(RemovePiece);
    }

    private void Start()
    {
        _highlightMaterial = TileManager.Instance.highlightMaterial;
        _invalidMaterial = TileManager.Instance.invalidMaterial;
        _killMaterial = TileManager.Instance.killMaterial;

        _rend = GetComponent<Renderer>();
        _originalMaterial = _rend.material;
    }

    private void OnDestroy()
    {
        _childEventHandler.onChildAdded.RemoveListener(StorePiece);
        _childEventHandler.onChildRemoved.RemoveListener(RemovePiece);
    }

    /* private void OnMouseEnter()
    {
        if (piece != null)
        {
            _rend.material = _highlightMaterial;
        }
    } */

    private void OnMouseDown()
    {
        // Check the previous current selected piece
        Piece currentPiece = TileManager.Instance.currentPieceSelected;

        if (tilePiece != null && GameManager.Instance.CurrentTeam == tilePiece.Team)
            DisplayMoves(this.tilePiece);

        if (currentPiece != null)
        {
            // Grab the current set of valid tiles to move to
            List<Tile> tilesToMove = new List<Tile>();

            try
            {
                tilesToMove = currentPiece.GetValidMoves();
            }
            catch (System.NotImplementedException e)
            {
                Debug.LogWarning("GetValidMoves method not implemented for this piece: " + e.Message);
            }

            // Check if it contains this tile
            if (tilesToMove.Contains(this))
            {
                // Check if it's an enemy piece, if it is, kill it
                if (tilePiece != null)
                {
                    if (GameManager.Instance.CurrentTeam != tilePiece.Team)
                    {
                        PieceManager.KillPiece(tilePiece);
                    }
                }

                DisplayMoves(currentPiece);
                currentPiece.MoveToTile(this);
            }
        }
    }

    private void DisplayMoves(Piece piece)
    {
        TileManager.Instance.currentTileSelected = this;
        TileManager.Instance.currentPieceSelected = piece;

        TileManager.Instance.UnhighlightAllTiles();

        _rend.material = _highlightMaterial;

        List<Tile> tilesToMove = new List<Tile>();
        List<Tile> invalidTiles = new List<Tile>();

        try
        {
            tilesToMove = piece.GetValidMoves();
        }
        catch (System.NotImplementedException e)
        {
            Debug.LogWarning("GetValidMoves method not implemented for this piece: " + e.Message);
        }

        try
        {
            invalidTiles = piece.GetInvalidMoves();
        }
        catch (System.NotImplementedException e)
        {
            Debug.LogWarning("GetInvalidMoves method not implemented for this piece: " + e.Message);
        }

        if (tilesToMove != null)
        {
            // Check if valid tiles are there
            if (tilesToMove.Count > 0)
            {
                foreach (Tile tiles in tilesToMove)
                {
                    tiles.Highlight(_highlightMaterial);
                }

                // Check if there are any killable chess pieces
                foreach (Tile killT in tilesToMove)
                {
                    if (killT.tilePiece != null && killT.tilePiece.Team != piece.Team)
                    {
                        killT.Highlight(_killMaterial);
                    }
                }
            }
            // If not, there aren't any places to move
            else
            {
                foreach (Tile invalidT in invalidTiles)
                {
                    invalidT.Highlight(_invalidMaterial);
                }
            }
        }
    }

    /* private void OnMouseExit()
    {
        // Unhighlight all tiles

        TileManager.Instance.UnhighlightAllTiles();
    } */

    public bool IsTileTaken()
    {
        return tilePiece != null;
    }

    private void StorePiece() => tilePiece = this.transform.GetComponentInChildren<Piece>();
    private void RemovePiece() => tilePiece = null;

    public void Highlight(Material material) => _rend.material = material;
    public void Unhighlight() => _rend.material = _originalMaterial;
}
