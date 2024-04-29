using ChenChen_MapGenerator;
using ChenChen_Scene;
using ChenChen_UISystem;
using CI.QuickSave;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string test_save = "TEST_SAVE";
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
        var writer = QuickSaveWriter.Create(root_save_name);
        writer.Write(test_save, SaveDate).Commit();
        Debug.Log($"成功保存存档{test_save}于{Application.persistentDataPath}");
    }

    public bool Load()
    {
        var reader = QuickSaveReader.Create(root_save_name);
        // 加载存档资源
        if (reader.Exists(test_save))
        {
            Debug.Log($"成功加载存档{test_save}资源于{Application.persistentDataPath}");
            SaveDate = reader.Read<Data_GameSave>(test_save);
            MapManager.Instance.LoadSceneMapFromSave(SaveDate.MapSave);
            return true;
        }
        else
        {
            Debug.Log($"未能加载存档{test_save}资源于{Application.persistentDataPath}");
            SaveDate = new Data_GameSave(test_save);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        //Save();
    }
}
