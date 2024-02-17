using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyScene;

namespace MyUISystem
{
    public class StartPanel : PanelBase
    {
        static readonly string path = "UI/Panel/StartPanel";
        public StartPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddChildComponent<Button>("BtnPlay").onClick.AddListener(() =>
            {
                // 当按下这个按钮时进行的方法
                GameManager.Instance.SceneSystem.SetScene(new MainScene());
            });
        }
    }
}