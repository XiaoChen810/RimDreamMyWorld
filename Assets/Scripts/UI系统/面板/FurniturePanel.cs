using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBuildingSystem;

namespace MyUISystem
{
    public class FurniturePanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/FurnitureMenu";
        public FurniturePanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            InitContent(BuildingSystemManager.Instance._FurnitureBlueprintsDict);
        }


    }
}