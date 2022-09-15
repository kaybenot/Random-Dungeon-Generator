using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vector2Int Pos { get; }
    public List<Vertex> Adj { get; }

    public Vertex(Vector2Int pos)
    {
        Pos = pos;
        Adj = new List<Vertex>();
    }
}

public class Graph
{
    public List<Vertex> Vertices = new List<Vertex>();
}
