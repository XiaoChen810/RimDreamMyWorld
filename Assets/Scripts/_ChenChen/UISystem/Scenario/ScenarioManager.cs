using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_UISystem
{
    /// <summary>
    /// ������ڹ����ߣ�������Ϸ�����򣬼�¼��Ϸ�Ĺ�������
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
        /// ������������һ���¼�������һ��ͻ�������...
        /// </summary>
        /// <param name="content"> �������� </param>
        /// <param name="target"> ָ��Ŀ�� </param>
        public void Narrative(string content, GameObject target)
        {
            _narrativePanel.AddOneNarrativeLog(content, target);          
        }
    }
}