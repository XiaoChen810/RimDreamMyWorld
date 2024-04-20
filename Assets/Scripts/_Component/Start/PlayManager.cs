using ChenChen_Scene;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private static readonly string test_save = "TEST_SAVE";
    public string GameSeed = "TEST_GAME";
    public Data_GameSave SaveDate;

    private void Start()
    {
        // ���ؿ�ʼ����
        SceneSystem.Instance.SetScene(new StartScene());
        // ���ش浵��Դ
        if (ES3.KeyExists(test_save))
        {
            Debug.Log($"�ɹ����ش浵{test_save}��Դ");
            SaveDate = ES3.Load<Data_GameSave>(test_save);
        }
        else
        {
            SaveDate = new Data_GameSave(test_save);
            SaveDate.SaveSeed = GameSeed;
            ES3.Save(test_save, SaveDate);
        }

        // �����������
        Random.InitState(SaveDate.SaveSeed.GetHashCode());
    }
}
