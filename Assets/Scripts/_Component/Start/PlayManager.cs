using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string root_save_name = "GameSave";

    public Data_GameSave SaveDate;

    private void Start()
    {
        //加载开始场景
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
        ES3.Save(root_save_name, SaveDate);
        Debug.Log($"成功保存存档{root_save_name}资源");
    }

    public bool Load()
    {
        // 加载存档资源
        if (ES3.KeyExists(root_save_name))
        {
            SaveDate = ES3.Load<Data_GameSave>(root_save_name);
            MapManager.Instance.LoadSceneMapFromSave(SaveDate);
            Debug.Log($"成功加载存档{root_save_name}资源");
            return true;
        }
        else
        {
            Debug.Log($"未能加载存档{root_save_name}资源");
            SaveDate = new Data_GameSave(root_save_name);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        //Save();
    }
}
