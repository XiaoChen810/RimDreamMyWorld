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
            // 异步加载场景
            GameManager.Instance.StartCoroutine(LoadSceneCo());
        }

        public override void OnExit()
        {
            // Do nothing
        }

        private void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            // 确保场景加载完成并且回调函数不为 null
            if (asyncOperation.isDone && OnPostLoad != null)
            {
                // 执行加载后动作
                OnPostLoad?.Invoke();
                GameManager.Instance.AnimatorTool.Animation_EndLoadingScene();
                GameManager.Instance.StartGame();
            }
        }

        private IEnumerator LoadSceneCo()
        {
            // 执行加载前动作
            OnPreLoad?.Invoke();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += OnSceneLoaded;
            asyncLoad.allowSceneActivation = false;

            // 过场动画
            GameManager.Instance.AnimatorTool.Animation_LoadingScene();
            Slider progressSilder = GameManager.Instance.AnimatorTool.ProgressSilder;
            bool animationPlayed = false;
            float timeAnimation = 0;
            // 等待异步加载操作完成
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