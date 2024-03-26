using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_BuildingSystem;

public class BlueprintGeneratorEditorWindow : EditorWindow
{
    private BlueprintGenerator blueprintGenerator;

    private string blueprintName;
    private BlueprintType blueprintType;
    private int blueprintWidth;
    private int blueprintHeight;
    private int blueprintWorkload;
    private Sprite blueprintPreviewSprite;
    private bool blueprintIsObstacle;
    private TileBase blueprintTileBase;

    [MenuItem("Window/Blueprint Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BlueprintGeneratorEditorWindow), false, "Blueprint Generator");
    }

    private void OnEnable()
    {
        blueprintGenerator = new BlueprintGenerator();
    }

    private void OnGUI()
    {
        GUILayout.Label("Blueprint Generator", EditorStyles.boldLabel);

        // 属性设置
        blueprintName = EditorGUILayout.TextField("名字", blueprintName);
        blueprintType = (BlueprintType)EditorGUILayout.EnumPopup("类型", blueprintType); 
        blueprintWorkload = EditorGUILayout.IntField("工作量", blueprintWorkload);
        blueprintPreviewSprite = (Sprite)EditorGUILayout.ObjectField("预览图", blueprintPreviewSprite, typeof(Sprite), false);
        blueprintIsObstacle = EditorGUILayout.Toggle("是否是障碍物", blueprintIsObstacle);
        blueprintTileBase = (TileBase)EditorGUILayout.ObjectField("瓦片", blueprintTileBase, typeof(TileBase), false);

        // 生成按钮
        if (GUILayout.Button("Generate Blueprint"))
        {
            if (blueprintGenerator != null)
            {
                // 设置蓝图生成器的属性
                blueprintGenerator.blueprintName = blueprintName;
                blueprintGenerator.blueprintType = blueprintType;
                blueprintGenerator.blueprintWorkload = blueprintWorkload;
                blueprintGenerator.blueprintPreviewSprite = blueprintPreviewSprite;
                blueprintGenerator.blueprintIsObstacle = blueprintIsObstacle;
                blueprintGenerator.blueprintTileBase = blueprintTileBase;

                // 调用生成蓝图的方法
                blueprintGenerator.GenerateBlueprint();
            }
            else
            {
                Debug.LogError("Could not find Blueprint Generator component in the scene.");
            }
        }
    }
}


