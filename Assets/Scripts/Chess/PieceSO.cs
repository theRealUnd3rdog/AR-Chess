using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Coordinates
{
    public string[] coordinates;
}

[CreateAssetMenu(fileName = "PieceSO", menuName = "PieceSO")]
public class PieceSO : ScriptableObject
{
    public Coordinates[] columnCoordinates;
    public Piece[] pieces;
}
