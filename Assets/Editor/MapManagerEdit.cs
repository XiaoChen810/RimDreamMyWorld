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
        if (GUILayout.Button("������ʱ��ͼ") )
        {
            MapManager.Instance.LoadOrGenerateSceneMap("��ʱ��ͼ");
        }
        if (GUILayout.Button("ɾ����ʱ��ͼ"))
        {
            MapManager.Instance.DeleteSceneMap("��ʱ��ͼ");
        }
    }
}
