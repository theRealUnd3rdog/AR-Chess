using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static Action<bool> OnPromotion;
    public Pawn pawnToPromote {set; get;}

    [SerializeField] private Canvas _promotionCanvas;

    [SerializeField] private bool _promotion;

    public bool Promotion
    {
        get {return _promotion;}
        set
        {
            if (_promotion != value)
            {
                _promotion = value;
                OnPromotionChange(_promotion);
            }
        }
    }

    private void OnPromotionChange(bool promotionState)
    {
        OnPromotion?.Invoke(promotionState);
    }

    public void OnPromotionStateChange(bool promotionState)
    {
        _promotionCanvas.gameObject.SetActive(promotionState);
    }

    public void UpgradeToQueen() => UpgradeToPiece<Queen>();
    public void UpgradeToKnight() => UpgradeToPiece<Knight>();
    public void UpgradeToBishop() => UpgradeToPiece<Bishop>();
    public void UpgradeToRook() => UpgradeToPiece<Rook>();

    public void UpgradeToPiece<T>()
    {
        Tile tileToSpawn = pawnToPromote.currentTile;
        PieceTeam team = pawnToPromote.Team;

        PieceManager.KillPiece(pawnToPromote);
        PieceManager.InstantiatePiece<T>(tileToSpawn, team);

        Promotion = false;
    }

    private void Awake()
    {
        Instance = this;
        OnPromotion += OnPromotionStateChange;
    }

    private void OnDestroy()
    {
        OnPromotion -= OnPromotionStateChange;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Promotion = !Promotion;
    }
}
