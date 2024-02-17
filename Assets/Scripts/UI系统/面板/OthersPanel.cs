using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBuildingSystem;

namespace MyUISystem
{
    public class OthersPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/OthersMenu";
        public OthersPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            InitContent(BuildingSystemManager.Instance._BuildingBlueprintsDict);

            //UITool.GetOrAddChildComponent<Button>("BtnBlueprintµöÓãµã").onClick.AddListener(() =>
            //{
            //    mf_UseBlueprint("µöÓãµã");
            //});

        }
    }
}