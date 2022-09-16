using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    public Vector2Int Pos { get; }
    public Vector2Int Size { get; }
    public List<GameObject> InstantiatedObjects { get; }

    public Room(Vector2Int pos, Vector2Int size)
    {
        Pos = pos;
        Size = size;
        InstantiatedObjects = new List<GameObject>();
    }

    public bool isTouching(Room other, Map map)
    {
        for (int x = Pos.x - 1; x <= Pos.x + Size.x; x++)
            for (int y = Pos.y - 1; y <= Pos.y + Size.y; y++)
            {
                if (x < 0 || y < 0)
                    continue;
                if (map[x, y].Occupied)
                    return true;
            }
        return false;
    }

    public void Instantiate(GameObject roomPrefab, Vector2 pointSize)
    {
        for(int x = Pos.x; x < Pos.x + Size.x; x++)
            for(int y = Pos.y; y < Pos.y + Size.y; y++)
                InstantiatedObjects.Add(Object.Instantiate(roomPrefab, new Vector3(x * pointSize.x, 0f, y * pointSize.y), Quaternion.identity));
    }

    public void MarkSpaceOccupied(Map map)
    {
        for (int x = Pos.x; x < Pos.x + Size.x; x++)
            for (int y = Pos.y; y < Pos.y + Size.y; y++)
                map[x, y].Occupied = true;
    }
}

public class Corridor
{
    public List<GameObject> InstantiatedObjects { get; }
    public List<Point> CorridorPoints { get; }

    public Corridor()
    {
        InstantiatedObjects = new List<GameObject>();
        CorridorPoints = new List<Point>();
    }

    public void Instantiate(GameObject corridorPrefab, Vector2 pointSize, Map map)
    {
        foreach(var point in CorridorPoints.Where(p => !map[p.Pos.x, p.Pos.y].Occupied))
            InstantiatedObjects.Add(Object.Instantiate(corridorPrefab, new Vector3(point.Pos.x * pointSize.x, 0f, point.Pos.y * pointSize.y), Quaternion.identity));
    }
}

public class Map
{
    private Grid grid;

    public int Width => grid.Width;
    public int Height => grid.Height;
    public List<Room> Rooms { get; }
    public List<Corridor> Corridors { get; }

    public Map(Vector2Int gridSize)
    {
        grid = new Grid(gridSize.x, gridSize.y);
        Rooms = new List<Room>();
        Corridors = new List<Corridor>();
    }

    public Point this[int x, int y] => grid[x, y];
    public Point this[Vector2 p] => grid[(int)p.x, (int)p.y];
}
