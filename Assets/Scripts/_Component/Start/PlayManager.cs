using ChenChen_Scene;
using ChenChen_UISystem;
using UnityEngine;

public class PlayManager : SingletonMono<PlayManager>
{
    private void Start()
    {
        SceneSystem.Instance.SetScene(new StartScene());
    }
}
