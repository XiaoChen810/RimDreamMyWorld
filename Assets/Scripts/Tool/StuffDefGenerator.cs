using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StuffDefGenerator 
{
    [Header("名字")]
    public string StuffName;
    [Header("描述")]
    public string StuffDescription;
    [Header("图片")]
    public Sprite StuffIcon;

    private readonly string saveRootPath = "Assets/Resources/Prefabs/StuffDef";

    public void GenerateStuffDef()
    {
        if (string.IsNullOrEmpty(StuffName))
        {
            Debug.LogError("Stuff name cannot be empty!");
            return;
        }

        if (StuffIcon == null)
        {
            Debug.LogError("Stuff sprites cannot be null!");
            return;
        }

        // 检查是否已存在同名的数据文件
        string existingFolderPath = $"{saveRootPath}/{StuffName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("StuffDef with the same name already exists!");
            return;
        }

        // 创建定义
        StuffDef stuffDef = ScriptableObject.CreateInstance<StuffDef>();

        // 设置基本信息
        stuffDef.Name = StuffName;
        stuffDef.Description = string.IsNullOrEmpty(StuffDescription) ? "没有描述" : StuffDescription;
        stuffDef.Icon = StuffIcon;

        // 创建文件夹
        string folderPath = $"{saveRootPath}/{StuffName}";
        AssetDatabase.CreateFolder(saveRootPath, StuffName);

        // 保存数据至指定路径
        string dataPath = $"{folderPath}/StuffDef_{StuffName}.asset";
        AssetDatabase.CreateAsset(stuffDef, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ThingDef {stuffDef.Name} and Prefab generated successfully!");
    }
}
