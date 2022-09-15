using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Point[,] grid;

    public int Width { get; }
    public int Height { get; }

    public Grid(int w, int h)
    {
        grid = new Point[w, h];
        Width = w;
        Height = h;
    }

    public Point this[Point p] => grid[p.Pos.x, p.Pos.y];
    public Point this[int x, int y] => grid[x, y];
}
