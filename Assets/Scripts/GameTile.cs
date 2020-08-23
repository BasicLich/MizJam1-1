using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : Tile
{
    [SerializeField]
    bool canBeMoved = true;
    public bool CanBeMoved { get { return canBeMoved; } }
}
