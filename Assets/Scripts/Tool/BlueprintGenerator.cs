using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyBuildingSystem;

public class BlueprintGenerator
{
    [Header("名字")]
    public string blueprintName;
    [Header("类型")]
    public BlueprintType blueprintType;
    [Header("长宽")]
    public int blueprintWidth;
    public int blueprintHeight;
    [Header("工作量")]
    public int blueprintWorkload;
    [Header("预览图")]
    public Sprite blueprintPreviewSprite;
    [Header("是否可以在上方继续放东西")]
    public bool blueprintStackable;
    [Header("最后生成的瓦片")]
    public TileBase blueprintTileBase;

    private readonly string saveRootPath = "Assets/Resources/Prefabs/Blueprints";

    public void GenerateBlueprint()
    {
        if (string.IsNullOrEmpty(blueprintName))
        {
            Debug.LogError("Blueprint name cannot be empty!");
            return;
        }

        if (blueprintPreviewSprite == null)
        {
            Debug.LogError("Blueprint preview sprite cannot be null!");
            return;
        }

        // 检查是否已存在同名的蓝图数据文件
        string existingFolderPath = $"{saveRootPath}/{blueprintName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("Blueprint with the same name already exists!");
            return;
        }

        // 创建蓝图数据
        BlueprintData blueprintData = ScriptableObject.CreateInstance<BlueprintData>();

        // 设置基本信息
        blueprintData.Name = blueprintName;
        blueprintData.Type = blueprintType;
        blueprintData.BuildingWidth = blueprintWidth;
        blueprintData.BuildingHeight = blueprintHeight;
        blueprintData.Workload = blueprintWorkload;
        blueprintData.PreviewSprite = blueprintPreviewSprite;
        blueprintData.Stackable = blueprintStackable;
        blueprintData.TileBase = blueprintTileBase == null ? null : blueprintTileBase;

        // 创建文件夹
        string folderPath = $"{saveRootPath}/{blueprintName}";
        AssetDatabase.CreateFolder(saveRootPath, blueprintName);

        // 保存蓝图数据至指定路径
        string blueprintDataPath = $"{folderPath}/{blueprintName}_BlueprintData.asset";
        AssetDatabase.CreateAsset(blueprintData, blueprintDataPath);

        CreateBlueprintPrefab<DefaultBlueprint>(blueprintName, blueprintData);

        CreateScript(folderPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Blueprint and Prefab generated successfully!\n" +
            "But also need to setting actually BlueprintBase script");

        void CreateBlueprintPrefab<T>(string name, BlueprintData data) where T : BlueprintBase
        {
            if (data.Prefab == null)
            {
                // 创建并添加SpriteRenderer，设置显示层
                GameObject prefab = new GameObject(name);
                SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                if (data.PreviewSprite != null) spriteRenderer.sprite = data.PreviewSprite;
                else
                {
                    spriteRenderer.sprite = null;
                    Debug.LogWarning("生成的预览图为空");
                }
                spriteRenderer.sortingLayerName = "Above";
                // 添加触发器
                BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                // 添加对应的蓝图基类
                BlueprintBase blueprintBase = prefab.AddComponent<T>();
                blueprintBase._BlueprintData = data;
                // 设置路径，保存为预制件
                PrefabUtility.SaveAsPrefabAsset(prefab, $"{folderPath}/{blueprintName}_Prefab.prefab");
                // 销毁临时预制件对象
                Object.DestroyImmediate(prefab);
                // 存
                GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{folderPath}/{blueprintName}_Prefab.prefab");
                data.Prefab = savedPrefab;
            }
        }

        void CreateScript(string folderPath)
        {
            // 生成脚本的路径
            string scriptPath = $"{folderPath}/{blueprintName}_Script.cs";

            // 生成脚本的内容
            string scriptContent = $@"
using UnityEngine;
using 建筑系统;

public class {blueprintName}_Script : BlueprintBase
{{
    protected override void OnEnable()
    {{
        base.OnEnable();
        Debug.LogError(""未设置正确的代码"");
    }}

    public override void Build(float thisWorkload)
    {{
    }}

    public override void Cancel()
    {{
    }}

    public override void Complete()
    {{
    }}

    public override void Placed()
    {{
    }}
}}";

            // 创建并保存脚本
            System.IO.File.WriteAllText(scriptPath, scriptContent);
        }
    }
}
