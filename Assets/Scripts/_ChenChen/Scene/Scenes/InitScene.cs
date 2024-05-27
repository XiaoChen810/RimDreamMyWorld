using ChenChen_UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChenChen_Scene
{
    public class InitScene : SceneBase
    {
        private static readonly string sceneName = "Init";
        private static readonly SceneType sceneType = SceneType.Init;
        private PanelManager panelManager;

        public InitScene() : base(sceneType) { }

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
                panelManager.AddPanel(new SelectionWindowPanel());
            }
        }

        public override void OnExit()
        {
            SceneManager.sceneLoaded -= WhenSceneLoaded;
        }


        private void WhenSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            panelManager = new PanelManager();
            panelManager.AddPanel(new SelectionWindowPanel());
            Debug.Log($"{sceneName}场景加载完毕");
        }
    }
}
