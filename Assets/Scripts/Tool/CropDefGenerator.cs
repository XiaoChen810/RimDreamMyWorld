using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CropDefGenerator
{
    [Header("名字")]
    public string CropName;
    [Header("描述")]
    public string CropDescription;
    [Header("图片")]
    public Sprite CropIcon;
    [Header("生长到最后一共所需营养值")]
    public float GroupNutrientRequiries;
    [Header("生长速度，每天增加多少营养值")]
    public float GroupSpeed;
    [Header("所有阶段的图像")]
    public List<Sprite> CropsSprites;

    private readonly string saveRootPath = "Assets/Resources/Prefabs/CropDef";

    public void GenerateCropDef()
    {
        if (string.IsNullOrEmpty(CropName))
        {
            Debug.LogError("Crop name cannot be empty!");
            return;
        }

        if (CropsSprites == null)
        {
            Debug.LogError("Crop sprites cannot be null!");
            return;
        }

        // 检查是否已存在同名的数据文件
        string existingFolderPath = $"{saveRootPath}/{CropName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("ThingDef with the same name already exists!");
            return;
        }

        // 创建定义
        CropDef cropDef = ScriptableObject.CreateInstance<CropDef>();

        // 设置基本信息
        cropDef.CropName = CropName;
        cropDef.CropDescription = string.IsNullOrEmpty(CropDescription) ? "没有描述" : CropDescription;
        cropDef.CropIcon = CropIcon;
        cropDef.GroupNutrientRequiries = GroupNutrientRequiries;
        cropDef.GroupSpeed = GroupSpeed;
        cropDef.CropsSprites = CropsSprites;

        // 创建文件夹
        string folderPath = $"{saveRootPath}/{CropName}";
        AssetDatabase.CreateFolder(saveRootPath, CropName);

        // 保存数据至指定路径
        string dataPath = $"{folderPath}/ThingDef_{CropName}.asset";
        AssetDatabase.CreateAsset(cropDef, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ThingDef {cropDef.CropName} and Prefab generated successfully!");
    }
}
