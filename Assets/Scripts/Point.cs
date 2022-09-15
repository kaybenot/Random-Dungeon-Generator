using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Vector2Int Pos = POS_UNINIT;
    public bool Occupied = false;

    public static readonly Vector2Int POS_UNINIT = new Vector2Int(-1, -1);
}
