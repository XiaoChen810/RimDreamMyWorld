using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CropDefGeneratorEditorWindow : EditorWindow
{
    private CropDefGenerator Generator;


    private string CropName;
    private string CropDescription;
    private Sprite CropIcon;
    private float GroupNutrientRequiries;
    private float GroupSpeed;
    private List<Object> CropsSprites = new List<Object>();

    [MenuItem("Window/CropDef Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CropDefGeneratorEditorWindow), false, "CropDef Generator");
    }

    private void OnEnable()
    {
        Generator = new CropDefGenerator();
    }

    private void OnGUI()
    {
        GUILayout.Label("CropDef Generator", EditorStyles.boldLabel);

        // ��������
        CropName = EditorGUILayout.TextField("����", CropName);
        CropDescription = EditorGUILayout.TextField("����", CropDescription);
        CropIcon = (Sprite)EditorGUILayout.ObjectField("ͼ��", CropIcon, typeof(Sprite), false);
        GroupNutrientRequiries = EditorGUILayout.FloatField("���������һ������Ӫ��ֵ", GroupNutrientRequiries);
        GroupSpeed = EditorGUILayout.FloatField("�����ٶȣ�ÿ�����Ӷ���Ӫ��ֵ", GroupSpeed);
        for (int i = 0; i < CropsSprites.Count; i++)
        {
            CropsSprites[i] = EditorGUILayout.ObjectField("����׶� " + i, CropsSprites[i], typeof(Sprite), allowSceneObjects: true);
        }
        // ���һ����ť����������µ�����׶�ͼ��
        if (GUILayout.Button("�������׶�"))
        {
            CropsSprites.Add(null); // ���һ���յ�Ԫ�أ���ʾ�µ�����׶�ͼ��
        }

        // ���ɰ�ť
        if (GUILayout.Button("Generate CropDef"))
        {
            if (Generator != null)
            {
                // ������ͼ������������
                Generator.CropName = CropName;
                Generator.CropDescription = CropDescription;
                Generator.CropIcon = CropIcon;
                Generator.GroupNutrientRequiries = GroupNutrientRequiries;
                Generator.GroupSpeed = GroupSpeed;
                List<Sprite> spriteList = new List<Sprite>();
                foreach (Object obj in CropsSprites)
                {
                    Sprite sprite = obj as Sprite;
                    if (sprite != null)
                    {
                        spriteList.Add(sprite);
                    }
                }
                Generator.CropsSprites = spriteList;

                // ����������ͼ�ķ���
                Generator.GenerateCropDef();
            }
            else
            {
                Debug.LogError("Could not find CropDef Generator component in the scene.");
            }
        }
    }
}
