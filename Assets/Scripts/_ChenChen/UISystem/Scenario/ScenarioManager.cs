using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UISystem
{
    public class ScenarioManager : SingletonMono<ScenarioManager>
    {
        private PanelManager _panelManager;

        protected override void Awake()
        {
            base.Awake();
            _panelManager.AddPanel(new ScenarioPanel());
        }
    }
}