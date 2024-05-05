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

    private void Start()
    {
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
            PanelManager.Instance.GetTopPanel(out PanelBase panel);
            if (panel == null || panel.GetType() != typeof(SettingPanel))
            {
                if(SceneSystem.Instance.CurScene.GetType() == typeof(MainScene))
                {
                    PanelManager.Instance.AddPanel(new SettingPanel());
                }                  
            }
            else
            {
                PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
            }
        }
    }

    /// <summary>
    /// �½�һ����Ϸ�浵�����浽�б����ر����½��Ĵ浵
    /// </summary>
    /// <param name="saveName"></param>
    public Data_GameSave Save(string saveName = "unnamed", Data_MapSave data_MapSave = null)
    {
        string saveDate = DateTime.Now.ToString();
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        if (data_MapSave == null)
        {
            data_MapSave = new Data_MapSave();
        }
        saveData.SaveMap = data_MapSave;
        foreach (var thing in BuildingSystemManager.Instance.transform.gameObject.GetComponentsInChildren<ThingBase>())
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
            Debug.Log($"Save a thing: {thingDef.DefName}");
        }
        SaveList.Add(saveData);
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
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
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
        }
    }

    private void OnApplicationQuit()
    {
        ES3.Save(root_save_name, SaveList);
        Debug.Log("��Ϸ�˳����Զ�����");
    }
}
