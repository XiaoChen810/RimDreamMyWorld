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
            UITool.TryGetChildComponentByName<Button>("Btn�ṹ").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Architectural));
            });
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Other));
            });
            UITool.TryGetChildComponentByName<Button>("Btn����̨").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Tool));
            });
            UITool.TryGetChildComponentByName<Button>("Btn�Ҿ�").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Furniture));
            });
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Light));
            });
            UITool.TryGetChildComponentByName<Button>("Btn����").onClick.AddListener(() =>
            {
                PanelManager.AddPanel(new Panel_Things(ChenChen_Thing.BuildingType.Defend));
            });
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });

        }
    }
}
