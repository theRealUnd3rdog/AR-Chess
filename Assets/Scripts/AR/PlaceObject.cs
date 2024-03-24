using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject _board;
    private bool _canPlaceObject;
    
    private ARRaycastManager _aRRaycastManager;
    private ARPlaneManager _aRPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _aRRaycastManager = GetComponent<ARRaycastManager>();
        _aRPlaneManager = GetComponent<ARPlaneManager>();

        ARUIManager.PlacementChange += ChangePlacementBool;
    }

    private void OnDestroy()
    {
        ARUIManager.PlacementChange -= ChangePlacementBool;
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();

        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();

        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void ChangePlacementBool(bool flag)
    {
        _canPlaceObject = flag;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (!_canPlaceObject)
            return;

        // Do not support multi touch if there is more than one finger
        if (finger.index != 0)
            return;

        // Cast a ray from finger screen pos to XR position
        if (_aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                // Get unity world space pose
                Pose pose = hit.pose;

                if (_aRPlaneManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    _board.transform.position = pose.position;
                    Vector3 position = _board.transform.position;
                    Vector3 cameraPosition = Camera.main.transform.position;

                    Vector3 direction = cameraPosition - position;
                    Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;

                    Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, _board.transform.up.normalized); // (0, 1, 0)
                    Quaternion targetRotation = Quaternion.Euler(scaledEuler);

                    _board.transform.rotation = _board.transform.rotation * targetRotation;
                }
            }
        }
    }
}
