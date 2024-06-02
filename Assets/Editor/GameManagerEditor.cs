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
        if (GUILayout.Button("≤‚ ‘∞¥≈•"))
        {
            GameManager.Instance.≤‚ ‘∞¥≈•();
        }      
    }
}
