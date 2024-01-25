using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI系统
{

    /// <summary>
    /// UI面板的基类
    /// </summary>
    public abstract class PanelBase
    {
        public UIType UIType { get; private set; }

        public UITool UITool { get; private set; }

        public PanelManager PanelManager { get; private set; }

        public UIManager UIManager { get; private set; }

        public PanelBase(UIType UIType)
        {
            this.UIType = UIType;
        }

        public void Init(UITool uITool)
        {
            UITool = uITool;
        }

        public void Init(PanelManager panelManager)
        {
            PanelManager = panelManager;
        }

        public void Init(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public virtual void OnEnter() { }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnExit()
        {

        }
    }
}