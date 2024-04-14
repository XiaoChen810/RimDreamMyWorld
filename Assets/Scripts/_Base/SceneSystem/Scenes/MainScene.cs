using System.Collections;
using System.Collections.Generic;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChenChen_MapGenerator;

namespace ChenChen_Scene
{
    public class MainScene : SceneBase
    {
        readonly string sceneName = "Main";
        private PanelManager panelManager;
        public override void OnEnter()
        {
            // 加载场景
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
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
            // panelManager.AddPanel(new MainPanel());
            MapManager.Instance.LoadSceneMap("MainMap", StaticDef.Seed_PlayerSelect_WhenMapGenerate);
            GameManager.Instance.生成测试棋子();
            Debug.Log($"{sceneName}场景加载完毕");
        }
    }
}