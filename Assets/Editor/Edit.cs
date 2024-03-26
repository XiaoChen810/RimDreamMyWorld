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

        // ��������
        blueprintName = EditorGUILayout.TextField("����", blueprintName);
        blueprintType = (BlueprintType)EditorGUILayout.EnumPopup("����", blueprintType); 
        blueprintWorkload = EditorGUILayout.IntField("������", blueprintWorkload);
        blueprintPreviewSprite = (Sprite)EditorGUILayout.ObjectField("Ԥ��ͼ", blueprintPreviewSprite, typeof(Sprite), false);
        blueprintIsObstacle = EditorGUILayout.Toggle("�Ƿ����ϰ���", blueprintIsObstacle);
        blueprintTileBase = (TileBase)EditorGUILayout.ObjectField("��Ƭ", blueprintTileBase, typeof(TileBase), false);

        // ���ɰ�ť
        if (GUILayout.Button("Generate Blueprint"))
        {
            if (blueprintGenerator != null)
            {
                // ������ͼ������������
                blueprintGenerator.blueprintName = blueprintName;
                blueprintGenerator.blueprintType = blueprintType;
                blueprintGenerator.blueprintWorkload = blueprintWorkload;
                blueprintGenerator.blueprintPreviewSprite = blueprintPreviewSprite;
                blueprintGenerator.blueprintIsObstacle = blueprintIsObstacle;
                blueprintGenerator.blueprintTileBase = blueprintTileBase;

                // ����������ͼ�ķ���
                blueprintGenerator.GenerateBlueprint();
            }
            else
            {
                Debug.LogError("Could not find Blueprint Generator component in the scene.");
            }
        }
    }
}


