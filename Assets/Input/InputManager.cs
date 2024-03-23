using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public static event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public static event EndTouchEvent OnEndTouch;
    private TouchControls _touchControls;

    private void Awake()
    {
        _touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls.Disable();
    }

    private void Start()
    {
        _touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        Debug.Log("Touch started " + _touchControls.Touch.TouchPosition.ReadValue<Vector2>());

        if (OnStartTouch != null)
        {
            OnStartTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        Debug.Log("Touch ended");

        if (OnEndTouch != null)
        {
            OnEndTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }
}
