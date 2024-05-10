using ChenChen_Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UISystem
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
        public void AddPanel(PanelBase nextPanel, bool stopCurrentPanel = true)
        {
            if(_panelsStack.Count > 0 && stopCurrentPanel)
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

        /// <summary>
        /// �Ƴ����( ���޶������ )��Ȼ��ָ���һ�����
        /// </summary>
        /// <param name="removedPanel"></param>
        public void RemovePanel(PanelBase removedPanel)
        {
            if (removedPanel == null) return;

            // �Ƴ��������
            if(_panelsStack.Count > 0)
            {
                if( _panelsStack.Peek() == removedPanel)
                {
                    _panelsStack.Pop().OnExit();
                }
                else
                {
                    Debug.Log($"��ǰ����UI����{removedPanel.UIType.Name}");
                }
            }
            // �Ƴ�����һ�����Ӵ���ͣ
            if(_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek().IsStopping)
                    _panelsStack.Peek().OnResume();
            }
        }

        /// <summary>
        /// �л���壬�����ǰ�Ѿ���������͵���壬��رգ�����������½���ǰ���
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="sceneType"> �޶������ֳ���������л������������ȫ�������� </param>
        public void TogglePanel(PanelBase nextPanel, SceneType sceneType = SceneType.None)
        {
            GetTopPanel(out PanelBase panel);
            if (panel == null || panel.GetType() != nextPanel.GetType())
            {
                if (sceneType == SceneType.None || SceneSystem.Instance.CurSceneType == sceneType)
                {
                    AddPanel(nextPanel);
                }
            }
            else
            {
                RemovePanel(GetTopPanel());
            }
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
        /// ��ȡ�������
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

        /// <summary>
        /// ��������Ƿ�Ϊ��
        /// </summary>
        /// <returns></returns>
        public bool PanelSpace()
        {
            return _panelsStack.Count == 0;
        }
    }
}