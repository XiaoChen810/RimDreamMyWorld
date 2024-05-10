using System.Collections;
using System.Collections.Generic;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChenChen_Scene
{
    public class StartScene : SceneBase
    {
        readonly string sceneName = "Start";
        private static readonly SceneType sceneType = SceneType.Start;
        private PanelManager panelManager;

        public StartScene() : base(sceneType) { }

        public override void OnEnter()
        {
            // 加载场景
            if(SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
                SceneManager.sceneLoaded += WhenSceneLoaded;
            }
            else
            {
                panelManager = new PanelManager();
                panelManager.AddPanel(new StartPanel());
            }

        }


        public override void OnExit()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }


        private void WhenSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            panelManager = new PanelManager();
            panelManager.AddPanel(new StartPanel());
            Debug.Log($"{sceneName}场景加载完毕");
        } 
    }
}