using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_UI
{
    /// <summary>
    /// ������ڹ����ߣ�������Ϸ�����򣬼�¼��Ϸ�Ĺ�������
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

        /// <summary>
        /// ������������һ���¼�������һ��ͻ�������...
        /// </summary>
        /// <param name="content"> �������� </param>
        /// <param name="target"> ָ��Ŀ�� </param>
        public void Narrative(string content, GameObject target)
        {
            if(_narrativePanel != null)
            {
                string text = $"{GameManager.Instance.currentHour}:{GameManager.Instance.currentMinute} " + content;
                _narrativePanel.AddOneNarrativeLog(text, target);
            }    
        }
    }
}