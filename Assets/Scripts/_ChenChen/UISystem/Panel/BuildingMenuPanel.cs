using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class BuildingMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/BuildingMenuPanel";

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
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Wall));
            });
            // 选择建造地板类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn地板").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Floor));
            });
            // 选择建造其他类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn其他").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Other));
            });
            // 选择建造家具类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn家具").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_BuildingSystem.ThingType.Furniture));
            });
            // 关闭菜单的按钮
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });

        }
    }
}
