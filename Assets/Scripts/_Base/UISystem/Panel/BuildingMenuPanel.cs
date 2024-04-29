using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class BuildingMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/BuildingMenuPanel";

        public BuildingMenuPanel(Callback onEnter, Callback onExit) : base(new UIType(path), onEnter, onExit) { }

        /// <summary>
        /// 添加各种菜单按钮的监听函数
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // 选择建造墙体类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn墙体").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new WallsMenuPanel());
            });
            // 选择建造地板类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn地板").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new FloorsMenuPanel());
            });
            // 选择建造其他类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn其他").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new OthersPanel());
            });
            // 选择建造家具类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn家具").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new FurniturePanel());
            });
            // 关闭菜单的按钮
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });

        }
    }
}
