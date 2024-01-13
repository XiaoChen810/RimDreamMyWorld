using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("GenerateMap"))
        {
            ((MapCreator)target).GenerateMap();
        }
        if (GUILayout.Button("CleanMap"))
        {
            ((MapCreator)target).CleanMap();
        }
    }
}
