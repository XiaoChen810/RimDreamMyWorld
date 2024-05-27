using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_Thing;

public class ThingDefGeneratorEditorWindow : EditorWindow
{
    private ThingDefGenerator ThingGenerator;

    private string ThingName;
    private ThingType ThingType;
    private int ThingWorkload;
    private int ThingDurability;
    private Sprite ThingPreviewSprite;
    private bool ThingIsObstacle;
    private TileBase ThingTileBase;
    private bool CreateIndividualScript;

    [MenuItem("Window/ThingDef Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ThingDefGeneratorEditorWindow), false, "ThingDef Generator");
    }

    private void OnEnable()
    {
        ThingGenerator = new ThingDefGenerator();
    }

    private void OnGUI()
    {
        GUILayout.Label("ThingDef Generator", EditorStyles.boldLabel);

        // 属性设置
        ThingName = EditorGUILayout.TextField("名字", ThingName);
        ThingType = (ThingType)EditorGUILayout.EnumPopup("类型", ThingType); 
        ThingWorkload = EditorGUILayout.IntField("工作量", ThingWorkload);
        ThingDurability = EditorGUILayout.IntField("耐久度",ThingDurability);
        ThingPreviewSprite = (Sprite)EditorGUILayout.ObjectField("预览图", ThingPreviewSprite, typeof(Sprite), false);
        ThingIsObstacle = EditorGUILayout.Toggle("是否是障碍物", ThingIsObstacle);
        ThingTileBase = (TileBase)EditorGUILayout.ObjectField("瓦片", ThingTileBase, typeof(TileBase), false);
        CreateIndividualScript = EditorGUILayout.Toggle("是否生成独立脚本", CreateIndividualScript);

        // 生成按钮
        if (GUILayout.Button("Generate ThingDef"))
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
                ThingGenerator.CreateIndividualScript = CreateIndividualScript;

                // 调用生成蓝图的方法
                ThingGenerator.GenerateThingDef();
            }
            else
            {
                Debug.LogError("Could not find ThingDef Generator component in the scene.");
            }
        }
    }
}




