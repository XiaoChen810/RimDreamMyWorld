using System.Collections;
using System.Collections.Generic;
using UIϵͳ;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ����
{
    public class StartScene : SceneBase
    {
        readonly string sceneName = "Start";
        PanelManager panelManager;
        public override void OnEnter()
        {
            // ���س���
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
            Debug.Log($"{sceneName}�����������");
        } 
    }
}