using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : EditorWindow
{
    private Grid grid;
    private Vector2Int gridSize;
    private Vector2 pointSize;
    private GameObject roomObj;
    private GameObject corridorObj;
    
    [MenuItem("Window/Dungeon Generator")]
    public static void ShowWindow()
    {
        GetWindow<DungeonGenerator>("Dungeon Generator");
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
        gridSize = EditorGUILayout.Vector2IntField("Map Size", new Vector2Int(10, 10));
        pointSize = EditorGUILayout.Vector2Field("Point Size", new Vector2(1f, 1f));
        
        // Prefab settings section
        GUILayout.Space(15);
        GUILayout.Label("Instantiation Settings (Prefabs must match point size)", EditorStyles.boldLabel);
        roomObj = (GameObject)EditorGUILayout.ObjectField("Room Prefab", null, typeof(GameObject), false);
        corridorObj = (GameObject)EditorGUILayout.ObjectField("Corridor Prefab", null, typeof(GameObject), false);

        // Action buttons
        GUILayout.Space(15);
        if (GUILayout.Button("Generate Dungeon"))
            generateDungeon();
        if (GUILayout.Button("Clear Dungeon"))
            clearDungeon();
    }

    private void generateDungeon()
    {
        grid = new Grid(gridSize.x, gridSize.y);
    }

    private void clearDungeon()
    {
        grid = null;
    }

    private void placeRoomsRandomly()
    {
        
    }
}
