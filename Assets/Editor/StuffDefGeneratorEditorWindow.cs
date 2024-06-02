using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StuffDefGeneratorEditorWindow : EditorWindow
{ 
    private StuffDefGenerator Generator;

    private string StuffName;
    private string StuffDescription;
    private Sprite StuffIcon;

    [MenuItem("Window/StuffDef Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(StuffDefGeneratorEditorWindow), false, "StuffDef Generator");
    }

    private void OnEnable()
    {
        Generator = new StuffDefGenerator();
    }

    private void OnGUI()
    {
        GUILayout.Label("StuffDef Generator", EditorStyles.boldLabel);

        // ��������
        StuffName = EditorGUILayout.TextField("����", StuffName);
        StuffDescription = EditorGUILayout.TextField("����", StuffDescription);
        StuffIcon = (Sprite)EditorGUILayout.ObjectField("ͼ��", StuffIcon, typeof(Sprite), false);

        // ���ɰ�ť
        if (GUILayout.Button("Generate StuffDef"))
        {
            if (Generator != null)
            {
                // ������ͼ������������
                Generator.StuffName = StuffName;
                Generator.StuffDescription = StuffDescription;
                Generator.StuffIcon = StuffIcon;

                // ����������ͼ�ķ���
                Generator.GenerateStuffDef();
            }
            else
            {
                Debug.LogError("Could not find CropDef Generator component in the scene.");
            }
        }
    }
}
