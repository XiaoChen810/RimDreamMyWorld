using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_BuildingSystem;

public class ThingDefGenerator
{
    [Header("名字")]
    public string ThingName;
    [Header("类型")]
    public ThingType ThingType;
    [Header("工作量")]
    public int ThingWorkload;
    [Header("耐久度")]
    public int ThingDurability;
    [Header("预览图")]
    public Sprite ThingPreviewSprite;
    [Header("是否是障碍物")]
    public bool ThingIsObstacle;
    [Header("最后生成的瓦片")]
    public TileBase ThingTileBase;
    [Header("是否生成独立脚本")]
    public bool CreateIndividualScript;

    private readonly string saveRootPath = "Assets/Resources/Prefabs/ThingDef";

    public void GenerateBlueprint()
    {
        if (string.IsNullOrEmpty(ThingName))
        {
            Debug.LogError("Blueprint name cannot be empty!");
            return;
        }

        if (ThingPreviewSprite == null)
        {
            Debug.LogError("Blueprint preview sprite cannot be null!");
            return;
        }

        // 检查是否已存在同名的蓝图数据文件
        string existingFolderPath = $"{saveRootPath}/{ThingName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("ThingDef with the same name already exists!");
            return;
        }

        // 创建蓝图数据
        ThingDef thingDef = ScriptableObject.CreateInstance<ThingDef>();

        // 设置基本信息
        thingDef.DefName = ThingName;
        thingDef.Type = ThingType;
        thingDef.Workload = ThingWorkload;
        thingDef.Durability = ThingDurability;
        thingDef.PreviewSprite = ThingPreviewSprite;
        thingDef.IsObstacle = ThingIsObstacle;
        thingDef.TileBase = ThingTileBase == null ? null : ThingTileBase;

        // 创建文件夹
        string folderPath = $"{saveRootPath}/{ThingName}";
        AssetDatabase.CreateFolder(saveRootPath, ThingName);

        // 保存数据至指定路径
        string ThingDataPath = $"{folderPath}/ThingDef_{ThingName}.asset";
        AssetDatabase.CreateAsset(thingDef, ThingDataPath);

        // 生成预制件和脚本
        switch (ThingType)
        {
            case ThingType.Building:
                CreateThingPrefab<Thing_Building>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Building");
                break;
            case ThingType.Tree:
                CreateThingPrefab<Thing_Trees>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Trees");
                break;
            case ThingType.Wall:
                CreateThingPrefab<Thing_Wall>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Wall");
                break;
            case ThingType.Floor:
                CreateThingPrefab<Thing_Floor>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Floor");
                break;
            default:
                CreateThingPrefab<Thing_Building>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Building");
                break;
        }
     
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ThingDef {thingDef.DefName} and Prefab generated successfully!\n" +
            "But also need to setting actually BlueprintBase script");

        void CreateThingPrefab<T>(string name, ThingDef def) where T : ThingBase
        {
            if (def.Prefab == null)
            {
                // 创建并添加SpriteRenderer，设置显示层
                GameObject prefab = new GameObject(name);
                SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                if (def.PreviewSprite != null) spriteRenderer.sprite = def.PreviewSprite;
                else
                {
                    spriteRenderer.sprite = null;
                    Debug.LogWarning("生成的预览图为空");
                }
                spriteRenderer.sortingLayerName = "Above";
                // 添加触发器
                BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                // 添加对应的基类
                ThingBase ThingBase = prefab.AddComponent<T>();
                ThingBase.Def = def;
                // 设置路径，保存为预制件
                PrefabUtility.SaveAsPrefabAsset(prefab, $"{folderPath}/{ThingName}_Prefab.prefab");
                // 销毁临时预制件对象
                Object.DestroyImmediate(prefab);
                // 存
                GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{folderPath}/{ThingName}_Prefab.prefab");
                def.Prefab = savedPrefab;
            }
        }

        void CreateScript(string folderPath, string classBase)
        {
            if (!CreateIndividualScript) return;

            // 生成脚本的路径
            string scriptPath = $"{folderPath}/{ThingName}_Script.cs";

            // 生成脚本的内容
            string scriptContent = $@"
using UnityEngine;
namespace ChenChen_BuildingSystem
{{
    public class {ThingName}_Script : {classBase}
    {{

    }}
}}";

            // 创建并保存脚本
            System.IO.File.WriteAllText(scriptPath, scriptContent);
        }
    }
}
