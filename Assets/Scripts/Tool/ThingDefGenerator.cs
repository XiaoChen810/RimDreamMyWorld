#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_Thing;

public class ThingDefGenerator
{
    [Header("����")]
    public string ThingName;
    [Header("����")]
    public ThingType ThingType;
    [Header("������")]
    public int ThingWorkload;
    [Header("�;ö�")]
    public int ThingDurability;
    [Header("Ԥ��ͼ")]
    public Sprite ThingPreviewSprite;
    [Header("�Ƿ����ϰ���")]
    public bool ThingIsObstacle;
    [Header("������ɵ���Ƭ")]
    public TileBase ThingTileBase;
    [Header("�Ƿ����ɶ����ű�")]
    public bool CreateIndividualScript;

    private readonly string saveRootPath = "Assets/Resources/Prefabs/ThingDef";

    public void GenerateThingDef()
    {
        if (string.IsNullOrEmpty(ThingName))
        {
            Debug.LogError("Thing name cannot be empty!");
            return;
        }

        if (ThingPreviewSprite == null)
        {
            Debug.LogError("Thing preview sprite cannot be null!");
            return;
        }

        // ����Ƿ��Ѵ���ͬ���������ļ�
        string existingFolderPath = $"{saveRootPath}/{ThingName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("ThingDef with the same name already exists!");
            return;
        }

        // ������ͼ����
        ThingDef thingDef = ScriptableObject.CreateInstance<ThingDef>();

        // ���û�����Ϣ
        thingDef.DefName = ThingName;
        thingDef.Type = ThingType;
        thingDef.Workload = ThingWorkload;
        thingDef.Durability = ThingDurability;
        thingDef.PreviewSprite = ThingPreviewSprite;
        thingDef.IsObstacle = ThingIsObstacle;
        thingDef.TileBase = ThingTileBase == null ? null : ThingTileBase;

        // �����ļ���
        string folderPath = $"{saveRootPath}/{ThingName}";
        AssetDatabase.CreateFolder(saveRootPath, ThingName);

        // ����������ָ��·��
        string ThingDataPath = $"{folderPath}/ThingDef_{ThingName}.asset";
        AssetDatabase.CreateAsset(thingDef, ThingDataPath);

        // ����Ԥ�Ƽ��ͽű�
        switch (ThingType)
        {
            case ThingType.Architectural:
                CreateThingPrefab<Thing_Architectural>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Architectural");
                break;
            case ThingType.Furniture:
                CreateThingPrefab<Thing_Furniture>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Building");
                break;
            case ThingType.Tree:
                CreateThingPrefab<Thing_Tree>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Trees");
                break;
            case ThingType.ToolTable:
                CreateThingPrefab<Thing_ToolTable>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_ToolTable");
                break;
            default:
                CreateThingPrefab<Thing_Furniture>(ThingName, thingDef);
                CreateScript(folderPath, "Thing_Building");
                break;
        }
     
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ThingDef {thingDef.DefName} and Prefab generated successfully!\n" +
            "But also need to setting actually BlueprintBase script");

        void CreateThingPrefab<T>(string name, ThingDef def) where T : Thing
        {
            if (def.Prefab == null)
            {
                // ���������SpriteRenderer��������ʾ��
                GameObject prefab = new GameObject(name);
                SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                if (def.PreviewSprite != null) spriteRenderer.sprite = def.PreviewSprite;
                else
                {
                    spriteRenderer.sprite = null;
                    Debug.LogWarning("���ɵ�Ԥ��ͼΪ��");
                }
                spriteRenderer.sortingLayerName = "Above";
                // ��Ӵ�����
                BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
                // ��Ӷ�Ӧ�Ļ���
                Thing ThingBase = prefab.AddComponent<T>();
                ThingBase.Def = def;
                // ����·��������ΪԤ�Ƽ�
                PrefabUtility.SaveAsPrefabAsset(prefab, $"{folderPath}/{ThingName}_Prefab.prefab");
                // ������ʱԤ�Ƽ�����
                Object.DestroyImmediate(prefab);
                // ��
                GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{folderPath}/{ThingName}_Prefab.prefab");
                def.Prefab = savedPrefab;
            }
        }

        void CreateScript(string folderPath, string classBase)
        {
            if (!CreateIndividualScript) return;

            // ���ɽű���·��
            string scriptPath = $"{folderPath}/{ThingName}_Script.cs";

            // ���ɽű�������
            string scriptContent = $@"
using UnityEngine;
namespace ChenChen_BuildingSystem
{{
    public class {ThingName}_Script : {classBase}
    {{

    }}
}}";

            // ����������ű�
            System.IO.File.WriteAllText(scriptPath, scriptContent);
        }
    }
}
#endif