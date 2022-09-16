using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : EditorWindow
{
    private Map map;
    private Graph graph;
    
    [SerializeField] private Vector2Int gridSize = new Vector2Int(100, 100);
    [SerializeField] private Vector2 pointSize = new Vector2(1f, 1f);
    [SerializeField] private GameObject roomObj;
    [SerializeField] private GameObject corridorObj;
    [SerializeField] private int roomEdgeMin = 2;
    [SerializeField] private int roomEdgeMax = 5;
    [SerializeField] private int roomAmount = 60;
    
    [MenuItem("Window/Dungeon Generator")]
    public static void ShowWindow()
    {
        GetWindow<DungeonGenerator>("Dungeon Generator");
    }

    private void OnEnable()
    {
        var data = EditorPrefs.GetString("DungeonGenerator", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);
    }

    private void OnDisable()
    {
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("DungeonGenerator", data);
    }

    private void OnGUI()
    {
        // Title
        GUIStyleState textStyle = new GUIStyleState()
        {
            textColor = Color.white
        };
        GUIStyle titleStyle = new GUIStyle
        {
            fontSize = 40,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = textStyle,
            margin = new RectOffset(0, 0, 20, 20)
        };
        GUILayout.Label("Dungeon Generator", titleStyle);
        
        // Map settings section
        GUILayout.Label("Map Settings", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.Vector2IntField("Map Size", gridSize);
        pointSize = EditorGUILayout.Vector2Field("Point Size", pointSize);
        roomEdgeMin = EditorGUILayout.IntField("Room Edge Min Length", roomEdgeMin);
        roomEdgeMax = EditorGUILayout.IntField("Room Edge Max Length", roomEdgeMax);
        roomAmount = EditorGUILayout.IntField("Room Amount", roomAmount);
        
        // Prefab settings section
        GUILayout.Space(15);
        GUILayout.Label("Instantiation Settings (Prefabs must match point size)", EditorStyles.boldLabel);
        roomObj = (GameObject)EditorGUILayout.ObjectField("Room Prefab", roomObj, typeof(GameObject), false);
        corridorObj = (GameObject)EditorGUILayout.ObjectField("Corridor Prefab", corridorObj, typeof(GameObject), false);

        // Action buttons
        GUILayout.Space(15);
        if (GUILayout.Button("Generate Dungeon"))
            generateDungeon();
        if (GUILayout.Button("Clear Dungeon"))
            clearDungeon();
    }

    private void generateDungeon()
    {
        clearDungeon();
        
        map = new Map(gridSize);
        graph = new Graph();
        placeRoomsRandomly();
        passVertices();
        graph.BowyerWatson(map);
        graph.PrimMST();
        graph.AStar(map, graph.EdgesMST);
        instantiateCorridors();
    }

    private void clearDungeon()
    {
        if (map != null)
        {
            foreach (var obj in map.Rooms.SelectMany(room => room.InstantiatedObjects))
                if(obj)
                    DestroyImmediate(obj);
            foreach (var obj in map.Corridors.SelectMany(room => room.InstantiatedObjects))
                if(obj)
                    DestroyImmediate(obj);
            map = null;
        }
        graph = null;
    }

    private void passVertices()
    {
        foreach (var v in map.Rooms.Select(room => new Vertex(room.Pos + (Vector2)room.Size / 2f)))
            graph.Vertices.Add(v);
    }

    private void placeRoomsRandomly()
    {
        int counter = roomAmount;
        while (counter > 0)
        {
            Vector2Int size = new Vector2Int(Random.Range(roomEdgeMin, roomEdgeMax), Random.Range(roomEdgeMin, roomEdgeMax));
            Vector2Int pos = new Vector2Int(Random.Range(0, map.Width - size.x), Random.Range(0, map.Height - size.y));
            Room room = new Room(pos, size);

            bool isTouching = map.Rooms.Any(r => room.isTouching(r, map));
            if (!isTouching)
            {
                map.Rooms.Add(room);
                room.Instantiate(roomObj, pointSize);
                room.MarkSpaceOccupied(map);
                counter--;
            }
        }
    }

    private void instantiateCorridors()
    {
        foreach (var corridor in map.Corridors)
            corridor.Instantiate(corridorObj, pointSize, map);
    }
}
