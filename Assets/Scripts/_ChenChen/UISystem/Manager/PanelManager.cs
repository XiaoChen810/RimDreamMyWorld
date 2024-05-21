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
        public void RemoveTopPanel(PanelBase removedPanel)
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
            // �Ƴ�����һ���������ͣ
            if(_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek().IsStopping)
                    _panelsStack.Peek().OnResume();
            }
        }

        public void RemovePanel(PanelBase removedPanel)
        {
            if (removedPanel == null) return;

            Stack<PanelBase> temp = new();
            // �Ƴ�ѡ������
            while (_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek() == removedPanel)
                {
                    _panelsStack.Pop().OnExit();
                    break;
                }
                else
                {
                    temp.Push(_panelsStack.Pop());
                    Debug.Log($"��ǰ����UI����{removedPanel.UIType.Name}");
                }
            }
            // ��ʣ�µ����Ż�
            while (temp.Count > 0)
            {
                _panelsStack.Push(temp.Pop());
            }
            // �������ϲ��������ͣ
            if (_panelsStack.Count > 0)
            {
                if (_panelsStack.Peek().IsStopping)
                    _panelsStack.Peek().OnResume();
            }
        }

        /// <summary>
        /// �л���壬�����ǰ�Ѿ���������͵���壬��رգ�����������½���ǰ���
        /// </summary>
        /// <param name="nextPanel"></param>
        /// <param name="sceneType">�޶������ֳ���������л������������ȫ��������</param>
        /// <param name="doTopPanelRemove">��ʱ���½���ǰ���ʱ��رն������</param>
        public void TogglePanel(PanelBase nextPanel, SceneType sceneType = SceneType.None, bool doTopPanelRemove = false)
        {
            if (!TryGetTopPanel(out PanelBase top) || top.GetType() != nextPanel.GetType())
            {
                if (sceneType == SceneType.None || SceneSystem.Instance.CurSceneType == sceneType)
                {
                    if (doTopPanelRemove)
                    {
                        RemoveTopPanel(GetTopPanel());
                    }
                    AddPanel(nextPanel);
                }
            }
            else
            {
                RemoveTopPanel(GetTopPanel());
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
        public bool TryGetTopPanel(out PanelBase top)
        {
            _panelsStack.TryPeek(out var panel);
            if (panel != null)
            {
                top = panel;
                return true;
            }
            top = null;
            return false;
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