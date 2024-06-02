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

        // 属性设置
        StuffName = EditorGUILayout.TextField("名字", StuffName);
        StuffDescription = EditorGUILayout.TextField("描述", StuffDescription);
        StuffIcon = (Sprite)EditorGUILayout.ObjectField("图标", StuffIcon, typeof(Sprite), false);

        // 生成按钮
        if (GUILayout.Button("Generate StuffDef"))
        {
            if (Generator != null)
            {
                // 设置蓝图生成器的属性
                Generator.StuffName = StuffName;
                Generator.StuffDescription = StuffDescription;
                Generator.StuffIcon = StuffIcon;

                // 调用生成蓝图的方法
                Generator.GenerateStuffDef();
            }
            else
            {
                Debug.LogError("Could not find CropDef Generator component in the scene.");
            }
        }
    }
}
