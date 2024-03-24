using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ARBoard : MonoBehaviour
{
    private ARSelectionInteractable _selection;
    private ARTranslationInteractable _translation;
    private ARRotationInteractable _rotation;
    private ARScaleInteractable _scale;

    private void Awake()
    {
        _selection = GetComponent<ARSelectionInteractable>();
        _translation = GetComponent<ARTranslationInteractable>();
        _rotation = GetComponent<ARRotationInteractable>();
        _scale = GetComponent<ARScaleInteractable>();

        ARUIManager.AdjustmentChange += MRS;
    }

    private void OnDestroy()
    {
        ARUIManager.AdjustmentChange -= MRS;
    }

    public void MRS(bool flag)
    {
        _selection.enabled = flag;
        _translation.enabled = flag;
        _rotation.enabled = flag;
        _scale.enabled = flag;

        _selection.selectionVisualization.SetActive(flag);
    }
}
