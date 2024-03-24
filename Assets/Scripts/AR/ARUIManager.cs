using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ARUIManager : MonoBehaviour
{
    [SerializeField] private Button _placementButton;
    [SerializeField] private Button _adjustmentButton;

    private bool _placing = false;
    public bool placing
    {
        get {return _placing;}
        set
        {
            if (_placing != value)
            {
                _placing = value;
                OnPlacementChanged(_placing);
            }
        }
    }

    private bool _adjusting = false;
    public bool adjusting
    {
        get {return _adjusting;}
        set
        {
            if (_adjusting != value)
            {
                _adjusting = value;
                OnAdjustmentChanged(_adjusting);
            }
        }
    }

    public static Action<bool> PlacementChange;
    public static Action<bool> AdjustmentChange;

    private void OnPlacementChanged(bool flag)
    {
        PlacementChange?.Invoke(flag);

        ColorUtility.TryParseHtmlString("#FF6161", out Color newCol);
        _placementButton.targetGraphic.color = flag ? newCol : Color.white;
    }

    private void OnAdjustmentChanged(bool flag)
    {
        AdjustmentChange?.Invoke(flag);

        ColorUtility.TryParseHtmlString("#FF6161", out Color newCol);
        _adjustmentButton.targetGraphic.color = adjusting ? newCol : Color.white;
    }

    public void ChangePlacement()
    { 
        placing = !placing;
        adjusting = false;
    }

    public void ChangeAdjustment()
    {
        adjusting = !adjusting;
        placing = false;
    }

    private void Awake()
    {
    }

    private void Start()
    {

    }
}
