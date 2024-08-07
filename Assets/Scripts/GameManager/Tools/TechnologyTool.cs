using System.Collections.Generic;
using UnityEngine;

public class TechnologyTool : MonoBehaviour
{
    [Header("已经解锁的科技")]
    public List<string> Unlocked = new List<string>();

    /// <summary>
    /// 全部科技的列表
    /// </summary>
    public Dictionary<string, TechnologyDef> TotalTechnologyDef = new Dictionary<string, TechnologyDef>();

    private void Start()
    {
        LoadAllTechnologyDefData();
    }

    private void LoadAllTechnologyDefData()
    {
        // 加载所有TechnologyDef资源
        TechnologyDef[] defs = Resources.LoadAll<TechnologyDef>("Prefabs/TechnologyDef");

        foreach (var def in defs)
        {
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
                Debug.LogError("Failed to load TechnologyDef.");
            }
        }

        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 解锁一个新的科技
    /// </summary>
    /// <param name="name"></param>
    public void UnlockNewTechnology(string name)
    {
        // 先确保所有前置科技已经解锁
        if (ArePreloadTechnologiesUnlocked(name))
        {
            if (!Unlocked.Contains(name))
            {
                Unlocked.Add(name);
                // 更新前置科技为此科技的科技
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
            Debug.LogWarning($"无法解锁科技 '{name}' 因为不存在");
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
    /// 判断该科技是否已经解锁了
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsUnlock(string name)
    {
        return Unlocked.Contains(name);
    }


}
