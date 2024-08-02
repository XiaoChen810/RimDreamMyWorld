using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_UI
{
    /// <summary>
    /// 故事情节管理者，控制游戏的走向，记录游戏的故事内容
    /// </summary>
    public class ScenarioManager : SingletonMono<ScenarioManager>
    {
        private PanelManager _panelManager;
        private NarrativePanel _narrativePanel;

        public bool Open = true;

        protected override void Awake()
        {
            base.Awake();
            if (!Open) return;
            _panelManager = new PanelManager();
            _panelManager.AddPanel(new NarrativePanel(this));
            _narrativePanel = _panelManager.GetTopPanel() as NarrativePanel;
        }
    }
}