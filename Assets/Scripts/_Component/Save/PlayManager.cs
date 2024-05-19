using ChenChen_BuildingSystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
using ChenChen_AI;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏流程的管理，保存进度，加载进度等
/// </summary>
public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string root_save_name = "GameSave";

    public List<Data_GameSave> SaveList = new List<Data_GameSave>();
    public PanelManager PanelManager { get; private set; }

    [Header("本次游戏加载的存档")]
    public Data_GameSave CurSave;

    private void Start()
    {
        PanelManager = new PanelManager();
        //加载开始场景
        SceneSystem.Instance.SetScene(new StartScene());

        if (ES3.KeyExists(root_save_name))
        {
            SaveList = ES3.Load<List<Data_GameSave>>(root_save_name);
            Debug.Log($"成功加载存档{root_save_name}资源");
        }
        else
        {
            Debug.Log($"未能加载存档{root_save_name}资源");
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            PanelManager.TogglePanel(new SettingPanel());
        }
    }

    /// <summary>
    /// 新建一个游戏存档并保存到列表，返回本次新建的存档
    /// </summary>
    /// <param name="saveName"></param>
    public Data_GameSave Save(string saveName = null, Data_MapSave data_MapSave = null)
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("未进入游戏，无法保存");
            return null;
        }
        // 新建存档
        string saveDate = DateTime.Now.ToString();
        saveName = CurSave.SaveName == null ? "unnamed" : CurSave.SaveName;
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        // 保存地图生成参数
        if (data_MapSave == null)
        {
            data_MapSave = new Data_MapSave();
        }
        saveData.SaveMap = data_MapSave;
        // 保存地图上所有的物品
        foreach (var thing in ThingSystemManager.Instance.transform.gameObject.GetComponentsInChildren<ThingBase>())
        {
            // 保存
            ThingDef thingDef = thing.Def;
            Data_ThingSave newThingSave = new Data_ThingSave(
                thingDef.DefName,
                thing.transform.position,
                thing.transform.rotation,
                thing.MapName,
                thing.LifeState);
            saveData.SaveThings.Add(newThingSave);
            Debug.Log($"Save a thing: {thingDef.DefName}");
        }
        // 保存全部棋子
        foreach (var pawnObj in GameManager.Instance.PawnsList)
        {
            Pawn pawn = pawnObj.GetComponent<Pawn>();
            Data_PawnSave newPawnSave = new Data_PawnSave(
                pawnObj.transform.position,
                pawn.Def,
                pawn.Attribute,
                pawn.Info
                );
            saveData.SavePawns.Add(newPawnSave);
        }
        // 判断是否有要覆盖的
        for (int i = 0; i < SaveList.Count; i++)
        {
            if (SaveList[i].SaveName == saveData.SaveName)
            {
                SaveList[i] = saveData;
                Debug.Log("覆盖了");
            }
        }
        if(!SaveList.Contains(saveData)) SaveList.Add(saveData);
        // 最后，保存存档
        ES3.Save(root_save_name, SaveList);
        Debug.Log($"成功保存存档{saveName}资源, 日期{saveDate}");
        return saveData;
    }

    /// <summary>
    /// 加载一个游戏存档
    /// </summary>
    /// <param name="gameSave"></param>
    public void Load(Data_GameSave gameSave)
    {
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(gameSave);
        CurSave = gameSave;
    }

    /// <summary>
    /// 删除一个游戏存档
    /// </summary>
    /// <param name="save"></param>
    public void Delete(Data_GameSave save)
    {
        if (SaveList.Contains(save))
        {
            SaveList.Remove(save);
        }
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("游戏退出，自动保存");
    }
}
