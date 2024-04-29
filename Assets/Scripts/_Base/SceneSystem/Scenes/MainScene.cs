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
        private PanelManager panelManager;
        public Action OnCompleteAction;
        public MainScene(Action action = null)
        {
            this.OnCompleteAction = action;
        }

        public override void OnEnter()
        {
            // 加载场景
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadSceneAsync(sceneName);
                SceneManager.sceneLoaded += WhenSceneLoaded;
            }
            else
            {
                panelManager = new PanelManager();
                // panelManager.AddPanel(new MainPanel());
            }
        }


        public override void OnExit()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }

        private void WhenSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            panelManager = new PanelManager();
            if (MapManager.Instance.CurrentMapName == null)
            {
                MapManager.Instance.LoadOrGenerateSceneMap("MainMap");
            }
            // 初始化寻路算法的节点
            AstarPath.active.Scan();
            OnCompleteAction();
            Debug.Log($"{sceneName}场景加载完毕");
        }
    }
}