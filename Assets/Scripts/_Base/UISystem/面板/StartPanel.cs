using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_Scene;

namespace ChenChen_UISystem
{
    public class StartPanel : PanelBase
    {
        static readonly string path = "UI/Panel/StartPanel";
        public StartPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Button>("BtnPlay").onClick.AddListener(() =>
            {
                // 当按下这个按钮时进行的方法
                SceneSystem.Instance.SetScene(new InitScene());
            });
        }
    }
}