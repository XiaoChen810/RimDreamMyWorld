using ChenChen_Thing;
using ChenChen_Map;
using ChenChen_Scene;
using ChenChen_UI;
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
            PanelManager.TogglePanel(new SettingPanel(), SceneType.Main);
        }
    }

    /// <summary>
    /// 新建一个游戏存档并保存到列表，返回本次新建的存档
    /// </summary>
    /// <param name="saveName"></param>
    public Data_GameSave Save(string saveName = null)
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("未进入游戏，无法保存");
            return null;
        }
        // 新建存档
        string saveDate = DateTime.Now.ToString();
        saveName = saveName == null ? "unnamed" : CurSave.SaveName;
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        // 保存摄像机位置
        saveData.CameraPosition = Camera.main.transform.position;
        // 保存游戏时长
        saveData.currentSeason = GameManager.Instance.currentSeason;
        saveData.currentDay = GameManager.Instance.currentDay;
        saveData.currentHour = GameManager.Instance.currentHour;
        saveData.currentMinute = GameManager.Instance.currentMinute;
        // 保存地图生成参数
        saveData.SaveMap = MapManager.Instance.CurMapSave;
        // 保存地图上所有的物品
        foreach (var thing in ThingSystemManager.Instance.GetAllThingsInstance())
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
        }
        // 保存全部棋子
        foreach (var pawnObj in GameManager.Instance.PawnGeneratorTool.PawnsList)
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
        // 保存全部怪物
        foreach(var monster in GameManager.Instance.MonsterGeneratorTool.MonstersList)
        {
            Data_MonsterSave newMonsterSave = new Data_MonsterSave(
                monster.IndexId,
                monster.transform.position
                );
            saveData.SaveMonster.Add(newMonsterSave);
        }
        // 保存工作区
        foreach(var workSpace in GameManager.Instance.WorkSpaceTool.TotalWorkSpaceDictionary)
        {
            // 如果是种植区
            if(workSpace.Value.WorkSpaceType == WorkSpaceType.Farm)
            {
                WorkSpace_Farm workSpace_Farm = workSpace.Value.GetComponent<WorkSpace_Farm>();
                Data_FarmWorkSpaceSave data_FarmWorkSpaceSave = new Data_FarmWorkSpaceSave(workSpace_Farm.name, workSpace_Farm.CurCrop.CropName, workSpace_Farm.SR.bounds);
                // 遍历全部作物列表
                foreach(var cell in workSpace_Farm.Cells)
                {
                    Crop crop = cell.Value.Crop;
                    Data_CropSave cropSave = new Data_CropSave(
                        crop.transform.position,
                        crop.CurNutrient,
                        crop.CurPeriodIndex
                        );
                    data_FarmWorkSpaceSave.crops.Add(cropSave);
                }
                saveData.SaveFarmWorkSpace.Add(data_FarmWorkSpaceSave);
            }
            // 其他
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
        // 加载摄像机位置
        Camera.main.transform.position = gameSave.CameraPosition;
        // 加载游戏时间
        GameManager.Instance.InitGameTime(
            gameSave.currentSeason,
            gameSave.currentDay,
            gameSave.currentHour,
            gameSave.currentMinute
            );
        // 加载地图
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
        // 加载物品
        MapManager.Instance.ItemCreator.LoadItemFromSave(gameSave);
        // 加载Pawn
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(gameSave);
        // 加载Monster
        GameManager.Instance.MonsterGeneratorTool.LoadMonstersFromSave(gameSave);
        // 加载种植区
        GameManager.Instance.WorkSpaceTool.LoadFarmWorkSpaceFromSave(gameSave);
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
            ES3.Save(root_save_name, SaveList);
        }
    }

    private void OnApplicationQuit()
    {
        Save(CurSave.SaveName);
        Debug.Log("游戏退出，自动保存");
    }
}
