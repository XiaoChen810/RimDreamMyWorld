using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]

public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("����һ������С��"))
        {
            GameManager.Instance.����һ������С��();
        }
        if (GUILayout.Button("�˻ؿ�ʼ����"))
        {
            GameManager.Instance.�˻ؿ�ʼ����();
        }
        if (GUILayout.Button("����Χ����һ������"))
        {
            GameManager.Instance.����Χ����һ������();
        }
    }
}
