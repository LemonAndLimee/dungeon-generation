using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomsDungeonGenerator), true)]
public class RoomDungeonGeneratorEditor : Editor
{
    RoomsDungeonGenerator generator;

    private void Awake()
    {
        generator = (RoomsDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Dungeon"))
        {
            generator.GenerateDungeon();
        }
        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
        }
        if (GUILayout.Button("Add Room"))
        {
            generator.GenerateNewRoom();
        }
        
    }
}
