using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyBuildingSystem;
using static UnityEngine.GraphicsBuffer;

public class BlueprintGeneratorEditorWindow : EditorWindow
{
    private BlueprintGenerator blueprintGenerator;

    private string blueprintName;
    private BlueprintType blueprintType;
    private int blueprintWidth;
    private int blueprintHeight;
    private int blueprintWorkload;
    private Sprite blueprintPreviewSprite;
    private bool blueprintStackable;
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
        blueprintName = EditorGUILayout.TextField("Name", blueprintName);
        blueprintType = (BlueprintType)EditorGUILayout.EnumPopup("Type", blueprintType); 
        blueprintWidth = EditorGUILayout.IntField("Width", blueprintWidth);
        blueprintHeight = EditorGUILayout.IntField("Height", blueprintHeight);
        blueprintWorkload = EditorGUILayout.IntField("Workload", blueprintWorkload);
        blueprintPreviewSprite = (Sprite)EditorGUILayout.ObjectField("Preview Sprite", blueprintPreviewSprite, typeof(Sprite), false);
        blueprintStackable = EditorGUILayout.Toggle("Stackable", blueprintStackable);
        blueprintTileBase = (TileBase)EditorGUILayout.ObjectField("TileBase", blueprintTileBase, typeof(TileBase), false);

        // ���ɰ�ť
        if (GUILayout.Button("Generate Blueprint"))
        {
            if (blueprintGenerator != null)
            {
                // ������ͼ������������
                blueprintGenerator.blueprintName = blueprintName;
                blueprintGenerator.blueprintType = blueprintType;
                blueprintGenerator.blueprintWidth = blueprintWidth;
                blueprintGenerator.blueprintHeight = blueprintHeight;
                blueprintGenerator.blueprintWorkload = blueprintWorkload;
                blueprintGenerator.blueprintPreviewSprite = blueprintPreviewSprite;
                blueprintGenerator.blueprintStackable = blueprintStackable;
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


