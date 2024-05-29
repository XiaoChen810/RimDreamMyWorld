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

    public List<Data_GameSave> SaveList = new List<Data_GameSave>();
    public PanelManager PanelManager { get; private set; }

    [Header("������Ϸ���صĴ浵")]
    public Data_GameSave CurSave;

    private void Start()
    {
        PanelManager = new PanelManager();
        //���ؿ�ʼ����
        SceneSystem.Instance.SetScene(new StartScene());

        if (ES3.KeyExists(root_save_name))
        {
            SaveList = ES3.Load<List<Data_GameSave>>(root_save_name);
            Debug.Log($"�ɹ����ش浵{root_save_name}��Դ");
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
            PanelManager.TogglePanel(new SettingPanel(), SceneType.Main);
        }
    }

    /// <summary>
    /// �½�һ����Ϸ�浵�����浽�б����ر����½��Ĵ浵
    /// </summary>
    /// <param name="saveName"></param>
    public Data_GameSave Save(string saveName = null)
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("δ������Ϸ���޷�����");
            return null;
        }
        // �½��浵
        string saveDate = DateTime.Now.ToString();
        saveName = saveName == null ? "unnamed" : CurSave.SaveName;
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        // ���������λ��
        saveData.CameraPosition = Camera.main.transform.position;
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
                    Data_CropSave cropSave = new Data_CropSave(
                        crop.transform.position,
                        crop.CurNutrient,
                        crop.CurPeriodIndex
                        );
                    data_FarmWorkSpaceSave.crops.Add(cropSave);
                }
                saveData.SaveFarmWorkSpace.Add(data_FarmWorkSpaceSave);
            }
            // ����
        }
        // �ж��Ƿ���Ҫ���ǵ�
        for (int i = 0; i < SaveList.Count; i++)
        {
            if (SaveList[i].SaveName == saveData.SaveName)
            {
                SaveList[i] = saveData;
                Debug.Log("������");
            }
        }
        if(!SaveList.Contains(saveData)) SaveList.Add(saveData);
        // ��󣬱���浵
        ES3.Save(root_save_name, SaveList);
        Debug.Log($"�ɹ�����浵{saveName}��Դ, ����{saveDate}");
        return saveData;
    }

    /// <summary>
    /// ����һ����Ϸ�浵
    /// </summary>
    /// <param name="gameSave"></param>
    public void Load(Data_GameSave gameSave)
    {
        // ���������λ��
        Camera.main.transform.position = gameSave.CameraPosition;
        // ������Ϸʱ��
        GameManager.Instance.InitGameTime(
            gameSave.currentSeason,
            gameSave.currentDay,
            gameSave.currentHour,
            gameSave.currentMinute
            );
        // ���ص�ͼ
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
        // ������Ʒ
        MapManager.Instance.ItemCreator.LoadItemFromSave(gameSave);
        // ����Pawn
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(gameSave);
        // ����Monster
        GameManager.Instance.MonsterGeneratorTool.LoadMonstersFromSave(gameSave);
        // ������ֲ��
        GameManager.Instance.WorkSpaceTool.LoadFarmWorkSpaceFromSave(gameSave);
        CurSave = gameSave;
    }

    /// <summary>
    /// ɾ��һ����Ϸ�浵
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
        Debug.Log("��Ϸ�˳����Զ�����");
    }
}
