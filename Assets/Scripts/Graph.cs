using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Numeric
{
    public static bool AlmostEqual(float x, float y)
    {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2;
    }
}

public class Vertex
{
    public Vector2 Pos { get; }

    public Vertex(Vector2 pos)
    {
        Pos = pos;
    }
    
    public bool Equals(Vertex other)
    {
        return Pos.Equals(other.Pos);
    }
    
    public override bool Equals(object obj)
    {
        return obj is Vertex other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return Pos.GetHashCode();
    }
    
    public static bool operator ==(Vertex left, Vertex right)
    {
        if(ReferenceEquals(left, null)) return false;
        if(ReferenceEquals(right, null)) return false;
        return left.Pos == right.Pos;
    }
    
    public static bool operator !=(Vertex left, Vertex right)
    {
        return !(left == right);
    }
    
    public static bool AlmostEqual(Vertex left, Vertex right)
    {
        return Numeric.AlmostEqual(left.Pos.x, right.Pos.x) && Numeric.AlmostEqual(left.Pos.y, right.Pos.y);
    }
}

public class Edge : IEquatable<Edge>
{
    public Vertex A { get; }
    public Vertex B { get; }
    public bool IsBad { get; set; }
    
    public Edge(Vertex A, Vertex B)
    {
        this.A = A;
        this.B = B;
        IsBad = false;
    }
    
    public float Length => Vector2.Distance(A.Pos, B.Pos);
    
    public static bool AlmostEqual(Edge left, Edge right)
    {
        return Vertex.AlmostEqual(left.A, right.A) && Vertex.AlmostEqual(left.B, right.B)
               || Vertex.AlmostEqual(left.A, right.B) && Vertex.AlmostEqual(left.B, right.A);
    }

    public bool Equals(Edge other)
    {
        if (other == null)
            return false;
        return (A.Pos == other.A.Pos || A.Pos == other.B.Pos)
               && (B.Pos == other.B.Pos || B.Pos == other.A.Pos);
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge e)
            return Equals(e);
        return false;
    }

    public override int GetHashCode()
    {
        return A.GetHashCode() ^ B.GetHashCode();
    }
}

public class Triangle
{
    public Vertex A { get; }
    public Vertex B { get; }
    public Vertex C { get; }
    public bool IsBad { get; set; }

    public Triangle(Vertex A, Vertex B, Vertex C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
        IsBad = false;
    }
    
    public bool CircumCircleContains(Vertex V)
    {
        Vector2 a = A.Pos;
        Vector2 b = B.Pos;
        Vector2 c = C.Pos;
        Vector2 v = V.Pos;

        float aSqr = a.sqrMagnitude;
        float bSqr = b.sqrMagnitude;
        float cSqr = c.sqrMagnitude;
        
        float circumX = (aSqr * (c.y - b.y) + bSqr * (a.y - c.y) + cSqr * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
        float circumY = (aSqr * (c.x - b.x) + bSqr * (a.x - c.x) + cSqr * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));
        
        Vector2 circum = new Vector2(circumX / 2f, circumY / 2f);
        float circumRadius = Vector2.SqrMagnitude(a - circum);
        float dist = Vector2.SqrMagnitude(v - circum);
        return dist <= circumRadius;
    }
    
    public bool ContainsVertex(Vertex v)
    {
        return Vector2.Distance(v.Pos, A.Pos) < 0.01f
               || Vector2.Distance(v.Pos, B.Pos) < 0.01f
               || Vector2.Distance(v.Pos, C.Pos) < 0.01f;
    }
}

public class Graph
{
    public List<Vertex> Vertices { get; }
    public List<Edge> Edges { get; }
    public List<Edge> EdgesMST { get; private set; }

    public Graph()
    {
        Vertices = new List<Vertex>();
        Edges = new List<Edge>();
        EdgesMST = new List<Edge>();
    }

    private int h(Point current, Point target)
    {
        return Mathf.Abs(current.Pos.x - target.Pos.x) + Mathf.Abs(current.Pos.y - target.Pos.y);
    }
    
    private IEnumerable<Edge> AdjEdges(Vertex vertex)
    {
        return Edges.Where(e => e.A == vertex || e.B == vertex).ToList();
    }
    
    public void BowyerWatson(Map map)
    {
        List<Triangle> tris = new List<Triangle>();
        
        // Super triangle
        Vertex A = new Vertex(new Vector2(-1f * Mathf.Sqrt(3) / 3f * map.Height, 0f));
        Vertex B = new Vertex(new Vector2(map.Width + Mathf.Sqrt(3) / 3f * map.Height, 0f));
        Vertex C = new Vertex(new Vector2(map.Width / 2f, map.Height + Mathf.Sqrt(3) / 2f * map.Width));
        tris.Add(new Triangle(A, B, C));
        
        // Delaunay loop
        foreach (Vertex vertex in Vertices)
        {
            List<Edge> polygon = new List<Edge>();
            foreach (var t in tris.Where(t => t.CircumCircleContains(vertex)))
            {
                t.IsBad = true;
                polygon.Add(new Edge(t.A, t.B));
                polygon.Add(new Edge(t.B, t.C));
                polygon.Add(new Edge(t.C, t.A));
            }

            tris.RemoveAll(t => t.IsBad);

            for (int i = 0; i < polygon.Count; i++)
            {
                for (int j = i + 1; j < polygon.Count; j++)
                {
                    if (Edge.AlmostEqual(polygon[i], polygon[j]))
                    {
                        polygon[i].IsBad = true;
                        polygon[j].IsBad = true;
                    }
                }
            }

            polygon.RemoveAll(e => e.IsBad);

            tris.AddRange(polygon.Select(edge => new Triangle(edge.A, edge.B, vertex)));
        }
        
        tris.RemoveAll(t => t.ContainsVertex(A) || t.ContainsVertex(B) || t.ContainsVertex(C));
        
        HashSet<Edge> edgeSet = new HashSet<Edge>();

        foreach (var t in tris)
        {
            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);
            
            if(edgeSet.Add(ab))
                Edges.Add(ab);
            if(edgeSet.Add(bc))
                Edges.Add(bc);
            if(edgeSet.Add(ca))
                Edges.Add(ca);
        }
    }

    public void PrimMST()
    {
        // Not the most optimal implementation :)
        List<Vertex> T = new List<Vertex>();
        List<Edge> U = new List<Edge>();
        List<Edge> C = new List<Edge>();
        T.Add(Vertices[0]);
        List<Edge> H = AdjEdges(T[0]).ToList();

        while (T.Count < Vertices.Count)
        {
            Edge edge = H[0];
            foreach (var e in H.Where(e => e.Length < edge.Length))
                edge = e;

            bool added = false;
            if (!T.Contains(edge.A))
            {
                T.Add(edge.A);
                added = true;
                foreach (var e in AdjEdges(edge.A))
                    if (!C.Contains(e) && !H.Contains(e))
                        H.Add(e);
            }
            if (!T.Contains(edge.B))
            {
                T.Add(edge.B);
                added = true;
                foreach(var e in AdjEdges(edge.B))
                    if (!C.Contains(e) && !H.Contains(e))
                        H.Add(e);
            }
            if(added)
                U.Add(edge);
            C.Add(edge);
            H.Remove(edge);
        }
        EdgesMST = U;
    }
    
    public IEnumerable<Point> AStar(Map map, IEnumerable<Edge> edges)
    {
        List<Point> outPoints = new List<Point>();

        foreach (var edge in edges)
        {
            Corridor corridor = new Corridor();
            Point start = map[edge.A.Pos];
            Point end = map[edge.B.Pos];

            List<Point> points = new List<Point>();
            Dictionary<Point, int> G = new Dictionary<Point, int>();

            // Add starting point
            points.Add(start);
            G.Add(start, 0);
            foreach (var p in start.PointsAround(map, false))
                G.Add(p, 1);
        
            // Move through points and add them
            while (true)
            {
                Point min = G.Aggregate((l, r) => l.Value + h(l.Key, end) < r.Value + h(r.Key, end) ? l : r).Key;
                points.Add(min);
                if (min == end)
                    break;
                foreach (var p in min.PointsAround(map, false))
                {
                    if (!G.ContainsKey(p))
                        G.Add(p, G[min] + 1);
                    else
                        G[p] = G[min] + 1;
                }
                G.Remove(min);
            }
            
            corridor.CorridorPoints.AddRange(points);
            map.Corridors.Add(corridor);
            outPoints.AddRange(points);
        }
        
        return outPoints;
    }
}
