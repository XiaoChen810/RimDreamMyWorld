using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class ThingsPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/ThingsPanel";
        private ThingType ThingType;
        public ThingsPanel(ThingType thingType) : base(new UIType(path))
        {
            this.ThingType = thingType;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent(ThingType);
            UITool.TryGetChildComponentByName<Button>("Btn¹Ø±Õ").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
        }
    }
}