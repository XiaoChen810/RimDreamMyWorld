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

        // ��������
        ThingName = EditorGUILayout.TextField("����", ThingName);
        ThingType = (ThingType)EditorGUILayout.EnumPopup("����", ThingType); 
        ThingWorkload = EditorGUILayout.IntField("������", ThingWorkload);
        ThingDurability = EditorGUILayout.IntField("�;ö�",ThingDurability);
        ThingPreviewSprite = (Sprite)EditorGUILayout.ObjectField("Ԥ��ͼ", ThingPreviewSprite, typeof(Sprite), false);
        ThingIsObstacle = EditorGUILayout.Toggle("�Ƿ����ϰ���", ThingIsObstacle);
        ThingTileBase = (TileBase)EditorGUILayout.ObjectField("��Ƭ", ThingTileBase, typeof(TileBase), false);
        CreateIndividualScript = EditorGUILayout.Toggle("�Ƿ����ɶ����ű�", CreateIndividualScript);

        // ���ɰ�ť
        if (GUILayout.Button("Generate ThingDef"))
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
                ThingGenerator.CreateIndividualScript = CreateIndividualScript;

                // ����������ͼ�ķ���
                ThingGenerator.GenerateThingDef();
            }
            else
            {
                Debug.LogError("Could not find ThingDef Generator component in the scene.");
            }
        }
    }
}




