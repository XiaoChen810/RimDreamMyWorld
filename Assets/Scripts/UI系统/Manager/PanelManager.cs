using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIϵͳ
{
    /// <summary>
    /// ��������
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

            // ��ȡ�򴴽�nextPanel
            GameObject nextPanelObject = _manager.GetOrGenerateSingleUI(nextPanel.UIType);
            // ��ʼ��nextPanel��UITool
            nextPanel.Init(new UITool(nextPanelObject));
            // ��ʼ��nextPanel��PanelManagerΪ�Լ�
            nextPanel.Init(this);
            // ��ʼ��nextPanel��UIManager
            nextPanel.Init(_manager);
            // ��nextPanelѹ��ջ
            _panelsStack.Push(nextPanel);
            // ����nextPanel����ʱ�ķ���
            nextPanel.OnEnter();
        }

        public void RemovePanel(PanelBase nextPanel)
        {
            // �Ƴ��������
            if(_panelsStack.Count > 0)
            {
                if( _panelsStack.Peek() == nextPanel)
                {
                    _panelsStack.Peek().OnExit();
                }
                else
                {
                    Debug.Log($"��ǰ����UI����{nextPanel.UIType.Name}");
                }
            }
            // �Ƴ�����һ�����Ӵ���ͣ
            if(_panelsStack.Count > 0)
            {
                _panelsStack.Peek().OnResume();
            }
        }
    }
}