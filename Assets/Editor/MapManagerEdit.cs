using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ChenChen_MapGenerator;

[CustomEditor(typeof(MapManager))]
public class MapManagerEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成临时地图") )
        {
            MapManager.Instance.LoadOrGenerateSceneMap("临时地图");
        }
        if (GUILayout.Button("删除临时地图"))
        {
            MapManager.Instance.DeleteSceneMap("临时地图");
        }
    }
}
