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
        var writer = QuickSaveWriter.Create(root_save_name);
        writer.Write(test_save, SaveDate).Commit();
        Debug.Log($"�ɹ�����浵{test_save}��{Application.persistentDataPath}");
    }

    public bool Load()
    {
        var reader = QuickSaveReader.Create(root_save_name);
        // ���ش浵��Դ
        if (reader.Exists(test_save))
        {
            Debug.Log($"�ɹ����ش浵{test_save}��Դ��{Application.persistentDataPath}");
            SaveDate = reader.Read<Data_GameSave>(test_save);
            MapManager.Instance.LoadSceneMapFromSave(SaveDate.MapSave);
            return true;
        }
        else
        {
            Debug.Log($"δ�ܼ��ش浵{test_save}��Դ��{Application.persistentDataPath}");
            SaveDate = new Data_GameSave(test_save);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        //Save();
    }
}
