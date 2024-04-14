using ChenChen_Scene;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private void Start()
    {
        SceneSystem.Instance.SetScene(new StartScene());
    }
}
