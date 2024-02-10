using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece {private set; get;}
    private Material _highlightMaterial;
    private Material _invalidMaterial;
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

        _rend = GetComponent<Renderer>();
        _originalMaterial = _rend.material;
    }

    private void OnDestroy()
    {
        _childEventHandler.onChildAdded.RemoveListener(StorePiece);
        _childEventHandler.onChildRemoved.RemoveListener(RemovePiece);
    }

    private void OnMouseEnter()
    {
        _rend.material = _highlightMaterial;
 
        if (piece != null)
        {
            Debug.Log($"{piece.Type} from {piece.Team}");
            
            List<Tile> tilesToMove = piece.GetValidMoves();

            if (tilesToMove != null)
            {
                Unhighlight();

                foreach (Tile tiles in tilesToMove)
                {
                    tiles.Highlight(_highlightMaterial);
                }
            }
        }
    }

    private void OnMouseExit()
    {
        // Unhighlight all tiles
        TileManager.Instance.UnhighlightAllTiles();
    }

    public bool IsTileTaken()
    {
        Piece tilePiece = piece;

        if (tilePiece != null)
            return true;

        return false;
    }

    private void StorePiece() => piece = this.transform.GetComponentInChildren<Piece>();
    private void RemovePiece() => piece = null;

    public void Highlight(Material material) => _rend.material = material;
    public void Unhighlight() => _rend.material = _originalMaterial;
}
