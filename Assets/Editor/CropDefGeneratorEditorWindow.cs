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

        // 属性设置
        CropName = EditorGUILayout.TextField("名字", CropName);
        CropDescription = EditorGUILayout.TextField("描述", CropDescription);
        CropIcon = (Sprite)EditorGUILayout.ObjectField("图标", CropIcon, typeof(Sprite), false);
        GroupNutrientRequiries = EditorGUILayout.FloatField("生长到最后一共所需营养值", GroupNutrientRequiries);
        GroupSpeed = EditorGUILayout.FloatField("生长速度，每天增加多少营养值", GroupSpeed);
        for (int i = 0; i < CropsSprites.Count; i++)
        {
            CropsSprites[i] = EditorGUILayout.ObjectField("作物阶段 " + i, CropsSprites[i], typeof(Sprite), allowSceneObjects: true);
        }
        // 添加一个按钮，用于添加新的作物阶段图像
        if (GUILayout.Button("添加作物阶段"))
        {
            CropsSprites.Add(null); // 添加一个空的元素，表示新的作物阶段图像
        }

        // 生成按钮
        if (GUILayout.Button("Generate CropDef"))
        {
            if (Generator != null)
            {
                // 设置蓝图生成器的属性
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

                // 调用生成蓝图的方法
                Generator.GenerateCropDef();
            }
            else
            {
                Debug.LogError("Could not find CropDef Generator component in the scene.");
            }
        }
    }
}
