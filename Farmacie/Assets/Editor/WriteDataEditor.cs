using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(WriteData))]
public class WriteDataEditor : Editor
{
    private WriteData _myScript { get { return (WriteData)target; } }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(5);

        GUILayout.Label("Database");
        GUILayout.Space(5);

        if (GUILayout.Button("Write Data"))
            _myScript.Init();
        if (GUILayout.Button("Write Medicamentation Dangers"))
            _myScript.MedicamentationDangers();

        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

    }
}