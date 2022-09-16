using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Vector2Int Pos;
    public bool Occupied;

    public Point(int x, int y)
    {
        Pos = new Vector2Int(x, y);
        Occupied = false;
    }
    
    public bool DownValid() => Pos.y - 1 >= 0;
    public Point Down(Map map) => map[Pos.x, Pos.y - 1];
    public bool LeftValid() => Pos.x - 1 >= 0;
    public Point Left(Map map) => map[Pos.x - 1, Pos.y];
    public bool UpValid(Map map) => Pos.y + 1 < map.Height;
    public Point Up(Map map) => map[Pos.x, Pos.y + 1];
    public bool RightValid(Map map) => Pos.x + 1 < map.Width;
    public Point Right(Map map) => map[Pos.x + 1, Pos.y];
    public bool UpLeftValid(Map map) => LeftValid() && UpValid(map);
    public bool UpRightValid(Map map) => RightValid(map) && UpValid(map);
    public bool DownLeftValid() => LeftValid() && DownValid();
    public bool DownRightValid(Map map) => RightValid(map) && DownValid();
    public Point UpLeft(Map map) => map[Pos.x - 1, Pos.y + 1];
    public Point UpRight(Map map) => map[Pos.x + 1, Pos.y + 1];
    public Point DownLeft(Map map) => map[Pos.x - 1, Pos.y - 1];
    public Point DownRight(Map map) => map[Pos.x + 1, Pos.y - 1];

    public IEnumerable<Point> PointsAround(Map map, bool diagonals = true)
    {
        List<Point> points = new List<Point>();
        if(LeftValid()) points.Add(Left(map));
        if(RightValid(map)) points.Add(Right(map));
        if(UpValid(map)) points.Add(Up(map));
        if(DownValid()) points.Add(Down(map));
        if (!diagonals) return points;
        if(UpLeftValid(map)) points.Add(UpLeft(map));
        if(UpRightValid(map)) points.Add(UpRight(map));
        if(DownLeftValid()) points.Add(DownLeft(map));
        if(DownRightValid(map)) points.Add(DownRight(map));
        return points;
    }
}
