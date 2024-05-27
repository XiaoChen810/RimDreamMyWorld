using ChenChen_Thing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TechnologyTool : MonoBehaviour
{
    [Header("�Ѿ������ĿƼ�")]
    public List<string> Unlocked = new List<string>();

    /// <summary>
    /// ȫ���Ƽ����б�
    /// </summary>
    public Dictionary<string, TechnologyDef> TotalTechnologyDef = new Dictionary<string, TechnologyDef>();

    private void Start()
    {
        LoadAllDefData();
    }

    private void LoadAllDefData()
    {
        // ��ȡָ��·���µ�����TechnologyDef�ļ�
        string[] DataFiles = AssetDatabase.FindAssets("t:TechnologyDef", new[] { "Assets/Resources/Prefabs/TechnologyDef" });

        foreach (var dataFile in DataFiles)
        {
            // ����GUID����Def
            string dataAssetPath = AssetDatabase.GUIDToAssetPath(dataFile);
            TechnologyDef def = AssetDatabase.LoadAssetAtPath<TechnologyDef>(dataAssetPath);

            if (def != null)
            {
                if (!TotalTechnologyDef.ContainsKey(def.TechnologyName))
                {
                    TotalTechnologyDef.Add(def.TechnologyName, def);
                }
                else
                {
                    Debug.LogWarning($"TechnologyDef with name '{def.TechnologyName}' already exists. Skipping.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load TechnologyDef at path '{dataAssetPath}'.");
            }
        }
    }

    /// <summary>
    /// ����һ���µĿƼ�
    /// </summary>
    /// <param name="name"></param>
    public void UnlockNewTechnology(string name)
    {
        // ��ȷ������ǰ�ÿƼ��Ѿ�����
        if (ArePreloadTechnologiesUnlocked(name))
        {
            if (!Unlocked.Contains(name))
            {
                Unlocked.Add(name);
                // ����ǰ�ÿƼ�Ϊ�˿Ƽ��ĿƼ�
                UpdatePreloadTechnologies(name);
            }
        }

        bool ArePreloadTechnologiesUnlocked(string techName)
        {
            if (TotalTechnologyDef.ContainsKey(techName))
            {
                TechnologyDef techDef = TotalTechnologyDef[techName];
                foreach (string preTech in techDef.TechnologyPreLoad)
                {
                    if (!Unlocked.Contains(preTech))
                    {
                        return false;
                    }
                }
                return true;
            }
            Debug.LogWarning($"�޷������Ƽ� '{name}' ��Ϊ������");
            return false;
        }

        void UpdatePreloadTechnologies(string newTechName)
        {
            if (TotalTechnologyDef.ContainsKey(newTechName))
            {
                foreach (var tech in TotalTechnologyDef.Values)
                {
                    if (tech.TechnologyPreLoad.Contains(newTechName))
                    {
                        UnlockNewTechnology(tech.TechnologyName);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �жϸÿƼ��Ƿ��Ѿ�������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsUnlock(string name)
    {
        return Unlocked.Contains(name);
    }


}
