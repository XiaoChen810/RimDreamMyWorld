using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StuffDefGenerator 
{
    [Header("����")]
    public string StuffName;
    [Header("����")]
    public string StuffDescription;
    [Header("ͼƬ")]
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

        // ����Ƿ��Ѵ���ͬ���������ļ�
        string existingFolderPath = $"{saveRootPath}/{StuffName}";
        if (System.IO.Directory.Exists(existingFolderPath))
        {
            Debug.LogError("StuffDef with the same name already exists!");
            return;
        }

        // ��������
        StuffDef stuffDef = ScriptableObject.CreateInstance<StuffDef>();

        // ���û�����Ϣ
        stuffDef.Name = StuffName;
        stuffDef.Description = string.IsNullOrEmpty(StuffDescription) ? "û������" : StuffDescription;
        stuffDef.Icon = StuffIcon;

        // �����ļ���
        string folderPath = $"{saveRootPath}/{StuffName}";
        AssetDatabase.CreateFolder(saveRootPath, StuffName);

        // ����������ָ��·��
        string dataPath = $"{folderPath}/StuffDef_{StuffName}.asset";
        AssetDatabase.CreateAsset(stuffDef, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"ThingDef {stuffDef.Name} and Prefab generated successfully!");
    }
}
