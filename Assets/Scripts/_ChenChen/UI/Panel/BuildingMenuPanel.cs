using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class BuildingMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/BuildingMenuPanel";

        public BuildingMenuPanel() : base(new UIType(path)) { }

        /// <summary>
        /// 添加各种菜单按钮的监听函数
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // 选择建造建筑构造类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn结构").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Architectural));
            });
            // 选择建造其他类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn其他").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Other));
            });
            // 选择建造工作台类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn工作台").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.ToolTable));
            });
            // 选择建造家具类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn家具").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Furniture));
            });
            // 选择建造光照类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn光照").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Light));
            });
            // 选择建造防卫类型的按钮
            UITool.TryGetChildComponentByName<Button>("Btn防卫").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new ThingsPanel(ChenChen_Thing.ThingType.Defend));
            });

            // 关闭菜单的按钮
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });

        }
    }
}
