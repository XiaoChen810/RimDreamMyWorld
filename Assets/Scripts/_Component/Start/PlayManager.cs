using ChenChen_BuildingSystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string root_save_name = "GameSave";

    public List<Data_GameSave> SaveList = new List<Data_GameSave>();

    // 本次游戏生成地图的数据
    private Data_MapSave _mapSaveThisPlay;
    public Data_MapSave MapSaveThisPlay
    {
        get { return _mapSaveThisPlay; }
        set
        {
            if(_mapSaveThisPlay == null)
            {
                _mapSaveThisPlay = value;
            }
        }
    }

    private void Start()
    {
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
            PanelManager.Instance.GetTopPanel(out PanelBase panel);
            if (panel == null || panel.GetType() != typeof(SettingPanel))
            {
                PanelManager.Instance.AddPanel(new SettingPanel());
            }
            else
            {
                PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
            }
        }
    }

    /// <summary>
    /// 保存一个游戏存档到列表
    /// </summary>
    /// <param name="saveName"></param>
    public void Save(string saveName = "unnamed")
    {
        string saveDate = DateTime.Now.ToString();
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        saveData.SaveMap = MapSaveThisPlay;
        foreach (var thing in BuildingSystemManager.Instance.transform.gameObject.GetComponentsInChildren<ThingBase>())
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
        SaveList.Add(saveData);
        ES3.Save(root_save_name, SaveList);
        Debug.Log($"成功保存存档{saveName}资源, 日期{saveDate}");
    }

    /// <summary>
    /// 加载一个游戏存档
    /// </summary>
    /// <param name="gameSave"></param>
    public void Load(Data_GameSave gameSave)
    {
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
    }

    private void OnApplicationQuit()
    {
        ES3.Save(root_save_name, SaveList);
        Debug.Log("游戏退出，自动保存");
    }
}
