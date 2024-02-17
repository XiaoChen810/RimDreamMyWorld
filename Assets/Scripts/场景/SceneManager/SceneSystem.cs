using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScene
{
    public class SceneSystem
    {
        SceneBase currentScene;

        /// <summary>
        /// 设置新场景，退出旧场景
        /// </summary>
        /// <param name="scene"></param>
        public void SetScene(SceneBase scene)
        {
            currentScene?.OnExit();
            currentScene = scene;
            currentScene?.OnEnter();           
        }
    }
}