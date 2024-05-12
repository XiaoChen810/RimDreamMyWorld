using System.Collections;
using System.Collections.Generic;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace ChenChen_Scene
{
    public class MainScene : SceneBase
    {
        readonly string sceneName = "Main";
        private static readonly SceneType sceneType = SceneType.Main;
        private Action OnPreLoad;
        private Action OnPostLoad;
        private float _transitionDelay;

        public MainScene(Action preLoad = null, Action postLoad = null, float transitionDelay = 0) : base(sceneType)
        {
            this.OnPreLoad = preLoad;
            this.OnPostLoad = postLoad;
            this._transitionDelay = transitionDelay;
        }

        public override void OnEnter()
        {
            // ִ�м���ǰ����
            OnPreLoad?.Invoke();
            // �첽���س���
            GameManager.Instance.StartCoroutine(LoadSceneCo());
        }

        public override void OnExit()
        {
            // Do nothing
        }

        private void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            // ȷ������������ɲ��һص�������Ϊ null
            if (asyncOperation.isDone && OnPostLoad != null)
            {
                // ִ�лص�����
                OnPostLoad?.Invoke();
                GameManager.Instance.AnimatorTool.Animation_EndLoadingScene();
                Debug.Log($"{sceneName}�����������");
            }
        }

        private IEnumerator LoadSceneCo()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += OnSceneLoaded;
            asyncLoad.allowSceneActivation = false;

            // ��������
            GameManager.Instance.AnimatorTool.Animation_LoadingScene();
            bool animationPlayed = false;
            float timeAnimation = 0;
            // �ȴ��첽���ز������
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.1f && !animationPlayed)
                {
                    timeAnimation += Time.deltaTime;
                    if(timeAnimation > 1)
                    {
                        animationPlayed = true;
                    }
                }

                if (animationPlayed)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}