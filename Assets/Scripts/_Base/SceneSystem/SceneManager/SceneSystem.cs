using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Scene
{
    public class SceneSystem : Singleton<SceneSystem>
    {
        SceneBase currentScene;

        /// <summary>
        /// �����³������˳��ɳ���
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