using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : EditorWindow
{
    [MenuItem("Window/Dungeon Generator")]
    public static void ShowWindow()
    {
        GetWindow<DungeonGenerator>("Dungeon Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Button("Generate Dungeon");
        GUILayout.Button("Clear Dungeon");
    }
}
