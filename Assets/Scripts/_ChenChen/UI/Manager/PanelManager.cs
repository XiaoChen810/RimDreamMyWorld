using ChenChen_Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// ��������
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
        /// �������壬Ĭ����ͣԭ�ȶ������壬�������ѹ��ջ
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="stopCurrentPanel"> �Ƿ���ͣ������� </param>
        /// <param name="removeOldPanel"> �Ƿ��Ƴ��ɵ����</param>
        /// <param name="addToStack"> �Ƿ����ջ�������ջ��Ҫ�Լ���������</param>
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
        /// �Ƴ���壬Ȼ��ָ�����һ�����
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
        /// �л���壬�����ǰ�Ѿ���������͵���壬��رգ�����������½���ǰ���
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="doTopPanelRemove">��ʱ���½���ǰ���ʱ��رն���������ֻ����ͣ</param>
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
        /// ��ȡ�������
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
        /// ��������Ƿ�Ϊ��
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _panelsStack.Count == 0;
        }
    }
}