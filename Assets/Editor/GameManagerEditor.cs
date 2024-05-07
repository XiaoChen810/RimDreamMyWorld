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
        if (GUILayout.Button("退回开始场景"))
        {
            GameManager.Instance.退回开始场景();
        }
        if (GUILayout.Button("生成一个敌人"))
        {
            GameManager.Instance.生成一个敌人();
        }
    }
}
