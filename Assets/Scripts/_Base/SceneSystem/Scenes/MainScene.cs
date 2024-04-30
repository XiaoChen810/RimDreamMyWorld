using System.Collections;
using System.Collections.Generic;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChenChen_MapGenerator;
using System;

namespace ChenChen_Scene
{
    public class MainScene : SceneBase
    {
        readonly string sceneName = "Main";
        public Action OnCompleteAction;
        public MainScene(Action action = null)
        {
            this.OnCompleteAction = action;
        }

        public override void OnEnter()
        {
            // ���س���
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
                SceneManager.sceneLoaded += WhenSceneLoaded;
            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }

        private void WhenSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            MapManager.Instance.LoadOrGenerateSceneMap("MainMap");
            OnCompleteAction();
            Debug.Log($"{sceneName}�����������");
        }
    }
}