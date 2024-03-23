using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public static bool DisableCamera;

    private void Start()
    {
        //GameManager.MoveMade += CameraMove;
        GameManager.StateChanged += ChangeCameraOnStateChanged;
    }

    private void OnDestroy()
    {
        //GameManager.MoveMade -= CameraMove;
        GameManager.StateChanged -= ChangeCameraOnStateChanged;
    }

    private void ChangeCameraOnStateChanged(GameState state)
    {
        if (state == GameState.Started)
        {
            PieceTeam team = GameManager.Instance.PlayerTeam;
            CameraMove(team);
        }
    }

    private void CameraMove(PieceTeam team)
    {
        if (!DisableCamera)
            transform.RotateAround (transform.position, transform.up, (team == PieceTeam.White) ? -90f: 90f);
    }
}
