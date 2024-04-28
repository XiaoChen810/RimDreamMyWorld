using ChenChen_Scene;
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
        var reader = QuickSaveReader.Create(root_save_name);
        // ���ش浵��Դ
        if (reader.Exists(test_save))
        {
            Debug.Log($"�ɹ����ش浵{test_save}��Դ��{Application.persistentDataPath}");
            SaveDate = reader.Read<Data_GameSave>(test_save);
        }
        else
        {
            SaveDate = new Data_GameSave(test_save);
        }
    }

    private void OnApplicationQuit()
    {
        var writer = QuickSaveWriter.Create(root_save_name);
        writer.Write(test_save, SaveDate).Commit();
        Debug.Log($"�ɹ�����浵{test_save}��{Application.persistentDataPath}");
    }
}
