using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_BuildingSystem;

public class BlueprintGeneratorEditorWindow : EditorWindow
{
    private ThingDefGenerator ThingGenerator;

    private string ThingName;
    private ThingType ThingType;
    private int ThingWorkload;
    private int ThingDurability;
    private Sprite ThingPreviewSprite;
    private bool ThingIsObstacle;
    private TileBase ThingTileBase;

    [MenuItem("Window/Blueprint Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BlueprintGeneratorEditorWindow), false, "Blueprint Generator");
    }

    private void OnEnable()
    {
        ThingGenerator = new ThingDefGenerator();
    }

    private void OnGUI()
    {
        GUILayout.Label("Blueprint Generator", EditorStyles.boldLabel);

        // 属性设置
        ThingName = EditorGUILayout.TextField("名字", ThingName);
        ThingType = (ThingType)EditorGUILayout.EnumPopup("类型", ThingType); 
        ThingWorkload = EditorGUILayout.IntField("工作量", ThingWorkload);
        ThingDurability = EditorGUILayout.IntField("耐久度",ThingDurability);
        ThingPreviewSprite = (Sprite)EditorGUILayout.ObjectField("预览图", ThingPreviewSprite, typeof(Sprite), false);
        ThingIsObstacle = EditorGUILayout.Toggle("是否是障碍物", ThingIsObstacle);
        ThingTileBase = (TileBase)EditorGUILayout.ObjectField("瓦片", ThingTileBase, typeof(TileBase), false);

        // 生成按钮
        if (GUILayout.Button("Generate Blueprint"))
        {
            if (ThingGenerator != null)
            {
                // 设置蓝图生成器的属性
                ThingGenerator.ThingName = ThingName;
                ThingGenerator.ThingType = ThingType;
                ThingGenerator.ThingWorkload = ThingWorkload;
                ThingGenerator.ThingDurability = ThingDurability;
                ThingGenerator.ThingPreviewSprite = ThingPreviewSprite;
                ThingGenerator.ThingIsObstacle = ThingIsObstacle;
                ThingGenerator.ThingTileBase = ThingTileBase;

                // 调用生成蓝图的方法
                ThingGenerator.GenerateBlueprint();
            }
            else
            {
                Debug.LogError("Could not find Blueprint Generator component in the scene.");
            }
        }
    }
}


