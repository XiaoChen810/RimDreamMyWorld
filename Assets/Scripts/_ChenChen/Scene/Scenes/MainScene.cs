using System.Collections;
using System.Collections.Generic;
using ChenChen_UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

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
                // ִ�м��غ���
                OnPostLoad?.Invoke();
                GameManager.Instance.AnimatorTool.Animation_EndLoadingScene();
                GameManager.Instance.StartGame();
            }
        }

        private IEnumerator LoadSceneCo()
        {
            // ִ�м���ǰ����
            OnPreLoad?.Invoke();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += OnSceneLoaded;
            asyncLoad.allowSceneActivation = false;

            // ��������
            GameManager.Instance.AnimatorTool.Animation_LoadingScene();
            Slider progressSilder = GameManager.Instance.AnimatorTool.ProgressSilder;
            bool animationPlayed = false;
            float timeAnimation = 0;
            // �ȴ��첽���ز������
            while (!asyncLoad.isDone)
            {
                yield return null;

                if (asyncLoad.progress >= 0.1f && !animationPlayed)
                {
                    progressSilder.value = Mathf.Clamp((timeAnimation / 1f), 0, 0.9f);
                    timeAnimation += Time.deltaTime;
                    if (timeAnimation > 1f)
                    {
                        animationPlayed = true;
                    }
                }

                if (animationPlayed)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }
        }
    }
}