using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_BuildingMenu : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/BuildingMenuPanel";

        public Panel_BuildingMenu() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            base.OnEnter();
            UITool.TryGetChildComponentByName<Button>("Btn结构").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.Architectural));
            });
            UITool.TryGetChildComponentByName<Button>("Btn其他").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.Other));
            });
            UITool.TryGetChildComponentByName<Button>("Btn工作台").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.ToolTable));
            });
            UITool.TryGetChildComponentByName<Button>("Btn家具").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.Furniture));
            });
            UITool.TryGetChildComponentByName<Button>("Btn光照").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.Light));
            });
            UITool.TryGetChildComponentByName<Button>("Btn防卫").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.ThingType.Defend));
            });
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });

        }
    }
}
