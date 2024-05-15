using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UISystem
{
    public class ScenarioPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/ScenarioPanel";
        public ScenarioPanel() : base(new UIType(path))
        {
        }
    }
}