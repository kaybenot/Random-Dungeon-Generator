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
        for(int x = 0; x < w; x++)
            for(int y = 0; y < h; y++)
                grid[x, y] = new Point(x, y);
        Width = w;
        Height = h;
    }
    
    public Point this[int x, int y] => grid[x, y];
}
