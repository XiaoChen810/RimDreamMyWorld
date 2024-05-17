using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_UISystem
{
    /// <summary>
    /// 故事情节管理者，控制游戏的走向，记录游戏的故事内容
    /// </summary>
    public class ScenarioManager : SingletonMono<ScenarioManager>
    {
        private PanelManager _panelManager;
        private NarrativePanel _narrativePanel;

        protected override void Awake()
        {
            base.Awake();
            _panelManager = new PanelManager();
            _panelManager.AddPanel(new NarrativePanel(this));
            _narrativePanel = _panelManager.GetTopPanel() as NarrativePanel;
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.H))
            {
                _narrativePanel.AddOneNarrativeLog("Test" + DateTime.Now.ToString(), null);
            }
        }

        /// <summary>
        /// 叙述，可能是一个事件，或者一个突发情况等...
        /// </summary>
        /// <param name="content"> 叙事内容 </param>
        /// <param name="target"> 指向目标 </param>
        public void Narrative(string content, GameObject target)
        {
            _narrativePanel.AddOneNarrativeLog(content, target);          
        }
    }
}