using ChenChen_Thing;
using ChenChen_Map;
using ChenChen_Scene;
using ChenChen_UI;
using ChenChen_AI;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ���̵Ĺ���������ȣ����ؽ��ȵ�
/// </summary>
public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string root_save_name = "GameSave";

    [Header("������Ϸ���صĴ浵")]
    public Data_GameSave CurSave = new Data_GameSave();
    public string CurSaveName => CurSave.SaveName;

    private void Start()
    {
        //���ؿ�ʼ����
        SceneSystem.Instance.SetScene(new StartScene());
        CurSave = new Data_GameSave(); 

        if (ES3.KeyExists(root_save_name))
        {
            ES3.LoadInto(root_save_name, CurSave);
            if (CurSave.IsNew)
            {
                Debug.Log($"�ɹ����ش浵{root_save_name}��Դ, ���浵���µ�");
            }
            else
            {
                Debug.Log($"�ɹ����ش浵{root_save_name}��Դ");
            }
        }
        else
        {
            Debug.Log($"δ�ܼ��ش浵{root_save_name}��Դ");
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
    /// ������Ϸ�浵
    /// </summary>
    public Data_GameSave Save()
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("δ������Ϸ���޷�����");
            return null;
        }
        // �½��浵
        Data_GameSave saveData = new Data_GameSave("�浵", DateTime.Now.ToString());
        // ���������
        saveData.CameraPosition = Camera.main.transform.position;
        saveData.CameraMoveSpeed = Camera.main.GetComponent<CameraController>().MoveSpeed;
        saveData.CameraZoomSpeed = Camera.main.GetComponent<CameraController>().ZoomSpeed;
        saveData.CameraUseKeyboard = Camera.main.GetComponent<CameraController>().UseKeyboard;
        saveData.CameraUseEdge = Camera.main.GetComponent<CameraController>().UseEdge;
        // ����������С
        saveData.BGMVolume = AudioManager.Instance.bgmSource.volume;
        saveData.SFXVolume = AudioManager.Instance.sfxSource.volume;
        // ������Ϸʱ��
        saveData.currentSeason = GameManager.Instance.currentSeason;
        saveData.currentDay = GameManager.Instance.currentDay;
        saveData.currentHour = GameManager.Instance.currentHour;
        saveData.currentMinute = GameManager.Instance.currentMinute;
        // �����ͼ���ɲ���
        saveData.SaveMap = MapManager.Instance.CurMapSave;
        // �����ͼ�����е���Ʒ
        foreach (var thing in ThingSystemManager.Instance.GetAllThingsInstance())
        {
            // ����
            ThingDef thingDef = thing.Def;
            Data_ThingSave newThingSave = new Data_ThingSave(
                thingDef.DefName,
                thing.transform.position,
                thing.transform.rotation,
                thing.MapName,
                thing.LifeState);
            saveData.SaveThings.Add(newThingSave);
        }
        // ����ȫ������
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
        // ����ȫ������
        foreach(var monster in GameManager.Instance.MonsterGeneratorTool.MonstersList)
        {
            Data_MonsterSave newMonsterSave = new Data_MonsterSave(
                monster.IndexId,
                monster.transform.position
                );
            saveData.SaveMonster.Add(newMonsterSave);
        }
        // ���湤����
        foreach(var workSpace in GameManager.Instance.WorkSpaceTool.TotalWorkSpaceDictionary)
        {
            // �������ֲ��
            if(workSpace.Value.WorkSpaceType == WorkSpaceType.Farm)
            {
                WorkSpace_Farm workSpace_Farm = workSpace.Value.GetComponent<WorkSpace_Farm>();
                Data_FarmWorkSpaceSave data_FarmWorkSpaceSave = new Data_FarmWorkSpaceSave(workSpace_Farm.name, workSpace_Farm.CurCrop.CropName, workSpace_Farm.SR.bounds);
                // ����ȫ�������б�
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
            // ����
        }
        CurSave = saveData;
        CurSave.IsNew = false;
        // ���д��浵
        ES3.Save(root_save_name, CurSave);
        Debug.Log($"�ɹ�����浵{CurSave.SaveName}��Դ, ����{CurSave.SaveDate}");
        return saveData;
    }

    /// <summary>
    /// ������Ϸ�浵
    /// </summary>
    /// <param name="gameSave"></param>
    public void Load()
    {
        if (CurSave == null)
        {
            Debug.LogWarning("�浵Ϊ���޷�����");
            return;
        }
        // ���������λ��
        Camera.main.transform.position = CurSave.CameraPosition;
        Camera.main.GetComponent<CameraController>().MoveSpeed = CurSave.CameraMoveSpeed;
        Camera.main.GetComponent<CameraController>().ZoomSpeed = CurSave.CameraZoomSpeed;
        Camera.main.GetComponent<CameraController>().UseKeyboard = CurSave.CameraUseKeyboard;
        Camera.main.GetComponent<CameraController>().UseEdge = CurSave.CameraUseEdge;
        // ����������С
        AudioManager.Instance.bgmSource.volume = CurSave.BGMVolume;
        AudioManager.Instance.sfxSource.volume = CurSave.SFXVolume;
        // ������Ϸʱ��
        GameManager.Instance.InitGameTime(
            CurSave.currentSeason,
            CurSave.currentDay,
            CurSave.currentHour,
            CurSave.currentMinute
            );
        // ���ص�ͼ
        MapManager.Instance.LoadSceneMapFromSave(CurSave);
        // ������Ʒ
        MapManager.Instance.ItemCreator.LoadItemFromSave(CurSave);
        // ����Pawn
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(CurSave);
        // ����Monster
        GameManager.Instance.MonsterGeneratorTool.LoadMonstersFromSave(CurSave);
        // ������ֲ��
        GameManager.Instance.WorkSpaceTool.LoadFarmWorkSpaceFromSave(CurSave);
    }

    /// <summary>
    /// ɾ����Ϸ�浵��ֻ�Ǳ���µ�
    /// </summary>
    /// <param name="save"></param>
    public void Delete()
    {
        CurSave = new();
        ES3.Save(root_save_name, CurSave);
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("��Ϸ�˳����Զ�����");
        //if(CurSave != null)
        //{
        //    Save();
        //}
    }
}
