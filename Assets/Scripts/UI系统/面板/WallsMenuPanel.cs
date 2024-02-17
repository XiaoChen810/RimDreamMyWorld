using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBuildingSystem;

namespace MyUISystem
{
    public class WallsMenuPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/WallsMenu";
        public WallsMenuPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddChildComponent<Button>("BtnBlueprint木墙").onClick.AddListener(() =>
            {
                mf_UseBlueprint("木墙");
            });

        }
    }
}
