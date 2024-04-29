using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_BuildingSystem;

namespace ChenChen_UISystem
{
    public class FloorsMenuPanel : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/FloorsMenu";
        public FloorsMenuPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            InitContent(ThingType.Floor);
            UITool.TryGetChildComponentByName<Button>("Btn¹Ø±Õ").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

    }
}