using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UISystem
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
        ///  添加新面板，暂停原先顶层的面板，把新面板压入栈
        /// </summary>
        /// <param name="nextPanel"></param>
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

        /// <summary>
        /// 移除面板( 仅限顶层面板 )，然后恢复下一个面板
        /// </summary>
        /// <param name="removedPanel"></param>
        public void RemovePanel(PanelBase removedPanel)
        {
            if (removedPanel == null) return;

            // 移除顶点面板
            if(_panelsStack.Count > 0)
            {
                if( _panelsStack.Peek() == removedPanel)
                {
                    _panelsStack.Pop().OnExit();
                }
                else
                {
                    Debug.Log($"当前顶层UI不是{removedPanel.UIType.Name}");
                }
            }
            // 移除后将下一个面板接触暂停
            if(_panelsStack.Count > 0)
            {
                _panelsStack.Peek().OnResume();
            }
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
        /// 获取顶层面板
        /// </summary>
        public PanelBase GetTopPanel(out PanelBase top)
        {
            _panelsStack.TryPeek(out var panel);
            if (panel != null)
            {
                top = panel;
                return panel;
            }
            top = null;
            return null;
        }

    }
}