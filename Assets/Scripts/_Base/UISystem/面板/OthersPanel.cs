using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_BuildingSystem;

namespace ChenChen_UISystem
{
    public class OthersPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/OthersMenu";
        public OthersPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            InitContent(BuildingSystemManager.Instance._BuildingBlueprintsDict);
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }
    }
}