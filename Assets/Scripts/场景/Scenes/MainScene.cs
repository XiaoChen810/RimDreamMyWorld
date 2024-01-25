using System.Collections;
using System.Collections.Generic;
using UI系统;
using UnityEngine;
using UnityEngine.SceneManagement;
using 地图生成;

namespace 场景
{
    public class MainScene : SceneBase
    {
        readonly string sceneName = "Main";

        public override void OnEnter()
        {
            // 加载场景
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
                SceneManager.sceneLoaded += WhenSceneLoaded;
            }

            MapManager.Instance.LoadSceneMap("MainMap");

        }


        public override void OnExit()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }


        private void WhenSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log($"{sceneName}加载完毕");
        }
    }
}