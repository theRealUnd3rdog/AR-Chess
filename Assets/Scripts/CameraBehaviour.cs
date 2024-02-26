using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private void Start()
    {
        GameManager.MoveMade += CameraMove;
    }

    private void OnDestroy()
    {
        GameManager.MoveMade -= CameraMove;
    }

    public void CameraMove(PieceTeam team)
    {
        transform.RotateAround (transform.position, transform.up, (team == PieceTeam.White) ? 180f: -180f);
    }
}
