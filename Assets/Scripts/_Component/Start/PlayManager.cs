using ChenChen_Scene;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string test_save = "TEST_SAVE";
    public string GameSeed = "TEST_GAME";
    public Data_GameSave SaveDate;

    private void Start()
    {
        // 加载开始场景
        SceneSystem.Instance.SetScene(new StartScene());
        // 加载存档资源
        if (ES3.KeyExists(test_save))
        {
            Debug.Log($"成功加载存档{test_save}资源");
            SaveDate = ES3.Load<Data_GameSave>(test_save);
        }
        else
        {
            SaveDate = new Data_GameSave(test_save);
            SaveDate.SaveSeed = GameSeed;
            ES3.Save(test_save, SaveDate);
        }

        // 设置随机种子
        Random.InitState(SaveDate.SaveSeed.GetHashCode());
    }
}
