using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI系统
{
    /// <summary>
    /// 管理面板的
    /// </summary>
    public class PanelManager
    {
        private Stack<PanelBase> _panelsStack;

        private UIManager _manager;

        public PanelManager()
        {
            _panelsStack = new Stack<PanelBase>();
            _manager = new UIManager();
        }

        public void AddPanel(PanelBase nextPanel)
        {
            if(_panelsStack.Count > 0)
            {
                PanelBase currentPanel = _panelsStack.Peek();
                currentPanel.OnPause();
            }

            // 获取或创建nextPanel
            GameObject nextPanelObject = _manager.GetOrGenerateSingleUI(nextPanel.UIType);
            // 初始化nextPanel的UITool
            nextPanel.Init(new UITool(nextPanelObject));
            // 初始化nextPanel的PanelManager为自己
            nextPanel.Init(this);
            // 初始化nextPanel的UIManager
            nextPanel.Init(_manager);
            // 把nextPanel压入栈
            _panelsStack.Push(nextPanel);
            // 调用nextPanel进入时的方法
            nextPanel.OnEnter();
        }

        public void RemovePanel(PanelBase nextPanel)
        {
            // 移除顶点面板
            if(_panelsStack.Count > 0)
            {
                if( _panelsStack.Peek() == nextPanel)
                {
                    _panelsStack.Peek().OnExit();
                }
                else
                {
                    Debug.Log($"当前顶层UI不是{nextPanel.UIType.Name}");
                }
            }
            // 移除后将下一个面板接触暂停
            if(_panelsStack.Count > 0)
            {
                _panelsStack.Peek().OnResume();
            }
        }
    }
}