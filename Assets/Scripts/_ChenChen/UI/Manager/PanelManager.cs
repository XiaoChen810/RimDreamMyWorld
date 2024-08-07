using ChenChen_Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// 管理面板的
    /// </summary>
    public class PanelManager : Singleton<PanelManager>
    {
        private Stack<PanelBase> _panelsStack;

        private UIManager _manager;

        public PanelManager()
        {
            _panelsStack = new Stack<PanelBase>();
            _manager = new UIManager();
        }

        /// <summary>
        /// 添加新面板，默认暂停原先顶层的面板，把新面板压入栈
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="stopCurrentPanel"> 是否暂停顶层面板 </param>
        /// <param name="removeOldPanel"> 是否移除旧的面板</param>
        /// <param name="addToStack"> 是否加入栈里，不加入栈则要自己管理销毁</param>
        public void AddPanel(PanelBase nextPanel, bool stopCurrentPanel = true, bool removeOldPanel = true, bool addToStack = true)
        {
            if(_panelsStack.Count > 0 && stopCurrentPanel)
            {
                PanelBase currentPanel = _panelsStack.Peek();
                currentPanel.OnPause();
            }

            if(removeOldPanel)
            {
                RemovePanel(nextPanel);
            }

            if (addToStack)
            {
                _panelsStack.Push(nextPanel);
            }

            GameObject nextPanelObject = _manager.GetOrGenerateSingleUI(nextPanel.UIType);
            nextPanel.Init(new UITool(nextPanelObject));
            nextPanel.Init(this);
            nextPanel.Init(_manager);
            nextPanel.OnEnter();
        }

        /// <summary>
        /// 移除面板，然后恢复其下一个面板
        /// </summary>
        public void RemovePanel(PanelBase removedPanel)
        {
            if (_panelsStack.Count == 0) return;

            Stack<PanelBase> temp = new();
            while (_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek() == removedPanel)
                {
                    _panelsStack.Pop().OnExit();
                    if (_panelsStack.Count > 0)
                    {
                        if (_panelsStack.Peek().IsStopping)
                            _panelsStack.Peek().OnResume();
                    }
                    break;
                }
                else
                {
                    temp.Push(_panelsStack.Pop());
                }
            }
            while (temp.Count > 0)
            {
                _panelsStack.Push(temp.Pop());
            }
        }

        public void RemovePanel(UIType removedPanelType)
        {
            if (_panelsStack.Count == 0) return;

            Stack<PanelBase> temp = new();
            while (_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek().UIType == removedPanelType)
                {
                    _panelsStack.Pop().OnExit();
                    if (_panelsStack.Count > 0)
                    {
                        if (_panelsStack.Peek().IsStopping)
                            _panelsStack.Peek().OnResume();
                    }
                    break;
                }
                else
                {
                    temp.Push(_panelsStack.Pop());
                }
            }
            while (temp.Count > 0)
            {
                _panelsStack.Push(temp.Pop());
            }
        }

        /// <summary>
        /// 切换面板，如果当前已经是这个类型的面板，则关闭，如果不是则新建当前面板
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="doTopPanelRemove">打开时，新建当前面板时会关闭顶层面板而不只是暂停</param>
        public void TogglePanel(PanelBase nextPanel, bool doTopPanelRemove = false)
        {      
            if(_panelsStack.TryPeek(out PanelBase top))
            {
                if(top.UIType.Equals(nextPanel.UIType))
                {
                    RemovePanel(top);
                    return;
                }
                if (doTopPanelRemove)
                {
                    RemovePanel(top);
                }
            }
            AddPanel(nextPanel);
        }

        /// <summary>
        /// 获取顶层面板
        /// </summary>
        public PanelBase GetTopPanel()
        {
            _panelsStack.TryPeek(out var panel);
            if(panel != null )
            {
                return panel;
            }
            return null;
        }

        /// <summary>
        /// 面板数量是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _panelsStack.Count == 0;
        }
    }
}