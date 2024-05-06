using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Scene
{
    public class SceneSystem : Singleton<SceneSystem>
    {
        SceneBase currentScene;
        public SceneBase CurScene
        {
            get
            {
                return currentScene;
            }
        }

        SceneType currentType;
        public SceneType CurSceneType
        {
            get
            {
                return currentType;
            }
        }
        /// <summary>
        /// 设置新场景，退出旧场景
        /// </summary>
        /// <param name="scene"></param>
        public void SetScene(SceneBase scene)
        {
            currentScene?.OnExit();
            currentScene = scene;
            currentType = scene.Type;
            currentScene?.OnEnter();           
        }
    }
}