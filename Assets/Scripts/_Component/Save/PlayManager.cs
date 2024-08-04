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

    [Header("本次游戏加载的存档")]
    public Data_GameSave CurSave = new Data_GameSave();
    public string CurSaveName => CurSave.SaveName;

    private void Start()
    {
        //加载开始场景
        SceneSystem.Instance.SetScene(new StartScene());
        CurSave = new Data_GameSave(); 

        if (ES3.KeyExists(root_save_name))
        {
            ES3.LoadInto(root_save_name, CurSave);
            if (CurSave.IsNew)
            {
                Debug.Log($"成功加载存档{root_save_name}资源, 但存档是新的");
            }
            else
            {
                Debug.Log($"成功加载存档{root_save_name}资源");
            }
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
            PanelManager.Instance.TogglePanel(new Panel_Setting());
        }
    }

    /// <summary>
    /// 保存游戏存档
    /// </summary>
    public Data_GameSave Save()
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("未进入游戏，无法保存");
            return null;
        }
        // 新建存档
        Data_GameSave saveData = new Data_GameSave("存档", DateTime.Now.ToString());
        // 保存摄像机
        saveData.CameraPosition = Camera.main.transform.position;
        saveData.CameraMoveSpeed = Camera.main.GetComponent<CameraController>().MoveSpeed;
        saveData.CameraZoomSpeed = Camera.main.GetComponent<CameraController>().ZoomSpeed;
        saveData.CameraUseKeyboard = Camera.main.GetComponent<CameraController>().UseKeyboard;
        saveData.CameraUseEdge = Camera.main.GetComponent<CameraController>().UseEdge;
        // 保存声音大小
        saveData.BGMVolume = AudioManager.Instance.bgmSource.volume;
        saveData.SFXVolume = AudioManager.Instance.sfxSource.volume;
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
        foreach (var pawn in GameManager.Instance.PawnGeneratorTool.PawnList_All)
        {
            Data_PawnSave newPawnSave = new Data_PawnSave(
                pawn.transform.position,
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
                    if (crop != null)
                    {
                        Data_CropSave cropSave = new Data_CropSave(
                            crop.transform.position,
                            crop.CurNutrient,
                            crop.CurPeriodIndex);
                        data_FarmWorkSpaceSave.crops.Add(cropSave);
                    }
                }
                saveData.SaveFarmWorkSpace.Add(data_FarmWorkSpaceSave);
            }
            // 其他
        }
        CurSave = saveData;
        CurSave.IsNew = false;
        // 最后，写入存档
        ES3.Save(root_save_name, CurSave);
        Debug.Log($"成功保存存档{CurSave.SaveName}资源, 日期{CurSave.SaveDate}");
        return saveData;
    }

    /// <summary>
    /// 加载游戏存档
    /// </summary>
    /// <param name="gameSave"></param>
    public void Load()
    {
        if (CurSave == null)
        {
            Debug.LogWarning("存档为空无法加载");
            return;
        }
        // 加载摄像机位置
        Camera.main.transform.position = CurSave.CameraPosition;
        Camera.main.GetComponent<CameraController>().MoveSpeed = CurSave.CameraMoveSpeed;
        Camera.main.GetComponent<CameraController>().ZoomSpeed = CurSave.CameraZoomSpeed;
        Camera.main.GetComponent<CameraController>().UseKeyboard = CurSave.CameraUseKeyboard;
        Camera.main.GetComponent<CameraController>().UseEdge = CurSave.CameraUseEdge;
        // 加载声音大小
        AudioManager.Instance.bgmSource.volume = CurSave.BGMVolume;
        AudioManager.Instance.sfxSource.volume = CurSave.SFXVolume;
        // 加载游戏时间
        GameManager.Instance.InitGameTime(
            CurSave.currentSeason,
            CurSave.currentDay,
            CurSave.currentHour,
            CurSave.currentMinute
            );
        // 加载地图
        MapManager.Instance.LoadSceneMapFromSave(CurSave);
        // 加载物品
        MapManager.Instance.ItemCreator.LoadItemFromSave(CurSave);
        // 加载Pawn
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(CurSave);
        // 加载Monster
        GameManager.Instance.MonsterGeneratorTool.LoadMonstersFromSave(CurSave);
        // 加载种植区
        GameManager.Instance.WorkSpaceTool.LoadFarmWorkSpaceFromSave(CurSave);
    }

    /// <summary>
    /// 删除游戏存档，只是变成新的
    /// </summary>
    /// <param name="save"></param>
    public void Delete()
    {
        CurSave = new();
        ES3.Save(root_save_name, CurSave);
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("游戏退出，自动保存");
        //if(CurSave != null)
        //{
        //    Save();
        //}
    }
}
