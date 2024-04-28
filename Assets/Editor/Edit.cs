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

        // ��������
        ThingName = EditorGUILayout.TextField("����", ThingName);
        ThingType = (ThingType)EditorGUILayout.EnumPopup("����", ThingType); 
        ThingWorkload = EditorGUILayout.IntField("������", ThingWorkload);
        ThingDurability = EditorGUILayout.IntField("�;ö�",ThingDurability);
        ThingPreviewSprite = (Sprite)EditorGUILayout.ObjectField("Ԥ��ͼ", ThingPreviewSprite, typeof(Sprite), false);
        ThingIsObstacle = EditorGUILayout.Toggle("�Ƿ����ϰ���", ThingIsObstacle);
        ThingTileBase = (TileBase)EditorGUILayout.ObjectField("��Ƭ", ThingTileBase, typeof(TileBase), false);

        // ���ɰ�ť
        if (GUILayout.Button("Generate Blueprint"))
        {
            if (ThingGenerator != null)
            {
                // ������ͼ������������
                ThingGenerator.ThingName = ThingName;
                ThingGenerator.ThingType = ThingType;
                ThingGenerator.ThingWorkload = ThingWorkload;
                ThingGenerator.ThingDurability = ThingDurability;
                ThingGenerator.ThingPreviewSprite = ThingPreviewSprite;
                ThingGenerator.ThingIsObstacle = ThingIsObstacle;
                ThingGenerator.ThingTileBase = ThingTileBase;

                // ����������ͼ�ķ���
                ThingGenerator.GenerateBlueprint();
            }
            else
            {
                Debug.LogError("Could not find Blueprint Generator component in the scene.");
            }
        }
    }
}


