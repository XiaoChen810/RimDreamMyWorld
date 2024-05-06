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
        private static readonly SceneType sceneType = SceneType.Main;
        private Action OnCompleteAction;

        public MainScene(Action action = null) : base(sceneType)
        {
            this.OnCompleteAction = action;
        }

        public override void OnEnter()
        {
            // 加载场景
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
            OnCompleteAction();
            Debug.Log($"{sceneName}场景加载完毕");
        }
    }
}