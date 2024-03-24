using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

public class Tile : NetworkBehaviour
{
    public Piece tilePiece {private set; get;}
    private bool _skippable;
    private bool _selectable;
    public bool Skippable
    {
        get {return _skippable;}
        set
        {
            if (_skippable != value)
            {
                _skippable = value;
                OnSkipChanged(_skippable);
            }
        }
    }

    // Event that runs whenever the tile skippable value changes
    public event System.Action<bool> SkipChange;

    private Material _highlightMaterial;
    private Material _invalidMaterial;
    private Material _killMaterial;
    private Material _originalMaterial;
    private Renderer _rend;
    private Collider _coll;

    private TileChildEventHandler _childEventHandler;

    private void OnSkipChanged(bool newSkipState)
    {
        SkipChange?.Invoke(newSkipState);
    }

    public virtual void OnSkipStateChanged(bool newSkipState)
    {

    }

    private void Awake()
    {
        _selectable = true;
        _coll = GetComponent<Collider>();
        _childEventHandler = GetComponent<TileChildEventHandler>();

        _childEventHandler.onChildAdded.AddListener(StorePiece);
        _childEventHandler.onChildRemoved.AddListener(RemovePiece);

        SkipChange += OnSkipStateChanged;
        UIManager.OnPromotion += ChangeSelection;
        ARUIManager.PlacementChange += ChangeSelection;
        ARUIManager.AdjustmentChange += ChangeSelection;

        GameManager.StateChanged += OnChangeGameState;

        InputManager.OnEndTouch += OnTilePress;
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

        SkipChange -= OnSkipStateChanged;
        UIManager.OnPromotion -= ChangeSelection;
        GameManager.StateChanged -= OnChangeGameState;
        InputManager.OnEndTouch -= OnTilePress;

        ARUIManager.PlacementChange -= ChangeSelection;
        ARUIManager.AdjustmentChange -= ChangeSelection;
    }

    private void OnChangeGameState(GameState state)
    {
        if (state == GameState.Ended)
        {
            ChangeSelection(true);
        }
    }

    private void ChangeSelection(bool flag) => _selectable = !flag;

    /* private void OnMouseEnter()
    {
        if (piece != null)
        {
            _rend.material = _highlightMaterial;
        }
    } */

    private void OnTilePress(Vector2 screenPos, float time)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
        RaycastHit hit;

        if (_coll.Raycast(ray, out hit, 1000.0f))
        {
            if (!_selectable)
                return;

            // Check the previous current selected piece
            Piece currentPiece = TileManager.Instance.currentPieceSelected;

            // Check if a tile has a piece and check if they are of current team
            if (tilePiece != null)
            {
                if (GameManager.Instance.CurrentTeam == tilePiece.Team && GameManager.Instance.PlayerTeam == tilePiece.Team)
                {
                    // Previous tile selected should no longer be skippable
                    if (TileManager.Instance.currentTileSelected != null)
                        TileManager.Instance.currentTileSelected.Skippable = false;

                    Skippable = true;

                    tilePiece.CheckKingOnSelection(); 
                    
                    // Select the current piece
                    currentPiece = tilePiece;
                    TileManager.Instance.currentPieceSelected = tilePiece;
                    // Select the current tile
                    TileManager.Instance.currentTileSelected = this;

                    TileManager.Instance.tilesToMove = tilePiece.GetPseudoValidMoves();

                    DisplayMoves(tilePiece, TileManager.Instance.tilesToMove);
                }
            }

            if (currentPiece != null)
            {
                // Grab the current set of valid tiles to move to
                List<Tile> tilesToMove = new List<Tile>();
                List<Tile> pseudoValidMoves = TileManager.Instance.tilesToMove;

                try
                {
                    tilesToMove = pseudoValidMoves;
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
                    
                    currentPiece.ServerMoveToTile(this);
                }
            }
        }
        
    }

    private void DisplayMoves(Piece piece, List<Tile> moves)
    {
        TileManager.Instance.UnhighlightAllTiles();

        _rend.material = _highlightMaterial;

        List<Tile> tilesToMove = new List<Tile>();
        List<Tile> invalidTiles = new List<Tile>();

        try
        {
            tilesToMove = moves;
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
        if (!Skippable)
            return tilePiece != null;
        else
        {
            return false;
        }
            
    }

    public void StorePiece() => tilePiece = this.transform.GetComponentInChildren<Piece>();
    public void RemovePiece() => tilePiece = null;

    public void Highlight(Material material) => _rend.material = material;
    public void Unhighlight() => _rend.material = _originalMaterial;
}
