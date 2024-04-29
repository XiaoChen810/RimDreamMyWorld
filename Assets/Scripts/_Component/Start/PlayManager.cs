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
        ES3.Save(root_save_name, SaveDate);
        Debug.Log($"�ɹ�����浵{root_save_name}��Դ");
    }

    public bool Load()
    {
        // ���ش浵��Դ
        if (ES3.KeyExists(root_save_name))
        {
            SaveDate = ES3.Load<Data_GameSave>(root_save_name);
            MapManager.Instance.LoadSceneMapFromSave(SaveDate);
            Debug.Log($"�ɹ����ش浵{root_save_name}��Դ");
            return true;
        }
        else
        {
            Debug.Log($"δ�ܼ��ش浵{root_save_name}��Դ");
            SaveDate = new Data_GameSave(root_save_name);
            return false;
        }
    }

    private void OnApplicationQuit()
    {
        //Save();
    }
}
