using System.Collections;
using System.Collections.Generic;
using UIϵͳ;
using UnityEngine;
using UnityEngine.SceneManagement;
using ��ͼ����;

namespace ����
{
    public class MainScene : SceneBase
    {
        readonly string sceneName = "Main";

        public override void OnEnter()
        {
            // ���س���
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
            Debug.Log($"{sceneName}�������");
        }
    }
}