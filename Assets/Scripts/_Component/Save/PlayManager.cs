using ChenChen_BuildingSystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
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
            PanelManager.TogglePanel(new SettingPanel());
        }
    }

    /// <summary>
    /// �½�һ����Ϸ�浵�����浽�б����ر����½��Ĵ浵
    /// </summary>
    /// <param name="saveName"></param>
    public Data_GameSave Save(string saveName = null, Data_MapSave data_MapSave = null)
    {
        if (SceneSystem.Instance.CurSceneType != SceneType.Main)
        {
            Debug.LogWarning("δ������Ϸ���޷�����");
            return null;
        }
        // �½��浵
        string saveDate = DateTime.Now.ToString();
        saveName = CurSave.SaveName == null ? "unnamed" : CurSave.SaveName;
        Data_GameSave saveData = new Data_GameSave(saveName, saveDate);
        // �����ͼ���ɲ���
        if (data_MapSave == null)
        {
            data_MapSave = new Data_MapSave();
        }
        saveData.SaveMap = data_MapSave;
        // �����ͼ�����е���Ʒ
        foreach (var thing in ThingSystemManager.Instance.transform.gameObject.GetComponentsInChildren<ThingBase>())
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
        // ����ȫ������
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
        MapManager.Instance.LoadSceneMapFromSave(gameSave);
        GameManager.Instance.PawnGeneratorTool.LoadScenePawnFromSave(gameSave);
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
        }
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("��Ϸ�˳����Զ�����");
    }
}
