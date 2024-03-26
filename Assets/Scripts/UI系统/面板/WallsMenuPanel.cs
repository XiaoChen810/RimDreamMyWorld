using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_BuildingSystem;

namespace MyUISystem
{
    public class WallsMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/WallsMenu";
        public WallsMenuPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            InitContent(BuildingSystemManager.Instance._WallBlueprintsDict);

        }
    }
}
