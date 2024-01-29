using System.Collections;
using System.Collections.Generic;
using UI系统;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace 场景
{
    public class StartScene : SceneBase
    {
        readonly string sceneName = "Start";
        PanelManager panelManager;
        public override void OnEnter()
        {
            // 加载场景
            if(SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
                SceneManager.sceneLoaded += WhenSceneLoaded;
            }
            panelManager = new PanelManager();
            panelManager.AddPanel(new StartPanel());
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