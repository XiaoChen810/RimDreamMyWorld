using ChenChen_BuildingSystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string root_save_name = "GameSave";

    public Data_GameSave SaveData;

    private void Start()
    {
        //���ؿ�ʼ����
        SceneSystem.Instance.SetScene(new StartScene());
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

    public void Save()
    {
        SaveData.SaveThings.Clear();
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
            SaveData.SaveThings.Add(newThingSave);
            Debug.Log($"Save a thing: {thingDef.DefName}");
        }

        ES3.Save(root_save_name, SaveData);
        Debug.Log($"�ɹ�����浵{root_save_name}��Դ");
    }

    public bool Load()
    {
        // ���ش浵��Դ
        if (ES3.KeyExists(root_save_name))
        {
            SaveData = ES3.Load<Data_GameSave>(root_save_name);
            MapManager.Instance.LoadSceneMapFromSave(SaveData);
            Debug.Log($"�ɹ����ش浵{root_save_name}��Դ");
            return true;
        }
        else
        {
            Debug.Log($"δ�ܼ��ش浵{root_save_name}��Դ");
            SaveData = new Data_GameSave(root_save_name);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        //Save();
    }
}
