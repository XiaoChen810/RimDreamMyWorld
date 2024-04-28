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
        //加载开始场景
        SceneSystem.Instance.SetScene(new StartScene());
        var reader = QuickSaveReader.Create(root_save_name);
        // 加载存档资源
        if (reader.Exists(test_save))
        {
            Debug.Log($"成功加载存档{test_save}资源于{Application.persistentDataPath}");
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
        Debug.Log($"成功保存存档{test_save}于{Application.persistentDataPath}");
    }
}
