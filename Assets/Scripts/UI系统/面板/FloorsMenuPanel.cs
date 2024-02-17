using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBuildingSystem;

namespace MyUISystem
{
    public class FloorsMenuPanel : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/FloorsMenu";
        public FloorsMenuPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            UITool.GetOrAddChildComponent<Button>("BtnBlueprintÄ¾µØ°å").onClick.AddListener(() =>
            {
                mf_UseBlueprint("Ä¾µØ°å");
            });
        }

    }
}