using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_BuildingSystem;

public class BlueprintGenerator
{
    [Header("����")]
    public string blueprintName;
    [Header("����")]
    public BlueprintType blueprintType;
    [Header("������")]
    public int blueprintWorkload;
    [Header("Ԥ��ͼ")]
    public Sprite blueprintPreviewSprite;
    [Header("�Ƿ����ϰ���")]
    public bool blueprintIsObstacle;
    [Header("������ɵ���Ƭ")]
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

        // ����Ƿ��Ѵ���ͬ������ͼ�����ļ�
        string existingFolderPath = $"{saveRootPath}/{blueprintName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("Blueprint with the same name already exists!");
            return;
        }

        // ������ͼ����
        BlueprintData blueprintData = ScriptableObject.CreateInstance<BlueprintData>();

        // ���û�����Ϣ
        blueprintData.Name = blueprintName;
        blueprintData.Type = blueprintType;
        blueprintData.Workload = blueprintWorkload;
        blueprintData.PreviewSprite = blueprintPreviewSprite;
        blueprintData.IsObstacle = blueprintIsObstacle;
        blueprintData.TileBase = blueprintTileBase == null ? null : blueprintTileBase;

        // �����ļ���
        string folderPath = $"{saveRootPath}/{blueprintName}";
        AssetDatabase.CreateFolder(saveRootPath, blueprintName);

        // ������ͼ������ָ��·��
        string blueprintDataPath = $"{folderPath}/{blueprintName}_BlueprintData.asset";
        AssetDatabase.CreateAsset(blueprintData, blueprintDataPath);

        CreateBlueprintPrefab<Building>(blueprintName, blueprintData);

        CreateScript(folderPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Blueprint and Prefab generated successfully!\n" +
            "But also need to setting actually BlueprintBase script");

        void CreateBlueprintPrefab<T>(string name, BlueprintData data) where T : BlueprintBase
        {
            if (data.Prefab == null)
            {
                // ���������SpriteRenderer��������ʾ��
                GameObject prefab = new GameObject(name);
                SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                if (data.PreviewSprite != null) spriteRenderer.sprite = data.PreviewSprite;
                else
                {
                    spriteRenderer.sprite = null;
                    Debug.LogWarning("���ɵ�Ԥ��ͼΪ��");
                }
                spriteRenderer.sortingLayerName = "Above";
                // ��Ӵ�����
                BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                // ��Ӷ�Ӧ����ͼ����
                BlueprintBase blueprintBase = prefab.AddComponent<T>();
                blueprintBase.Data = data;
                // ����·��������ΪԤ�Ƽ�
                PrefabUtility.SaveAsPrefabAsset(prefab, $"{folderPath}/{blueprintName}_Prefab.prefab");
                // ������ʱԤ�Ƽ�����
                Object.DestroyImmediate(prefab);
                // ��
                GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{folderPath}/{blueprintName}_Prefab.prefab");
                data.Prefab = savedPrefab;
            }
        }

        void CreateScript(string folderPath)
        {
            // ���ɽű���·��
            string scriptPath = $"{folderPath}/{blueprintName}_Script.cs";

            // ���ɽű�������
            string scriptContent = $@"
using UnityEngine;
namespace ChenChen_BuildingSystem
{{
    public class {blueprintName}_Script : Building
    {{

    }}
}}";

            // ����������ű�
            System.IO.File.WriteAllText(scriptPath, scriptContent);
        }
    }
}
