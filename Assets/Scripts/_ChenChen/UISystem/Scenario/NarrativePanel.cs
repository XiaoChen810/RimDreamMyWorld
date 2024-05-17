using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    /// <summary>
    /// ����������ڵ����
    /// </summary>
    public class NarrativePanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/NarrativePanel";

        private ScenarioManager _scenarioManager;
        private RectTransform _rectTransformMyself;
        private Button _openBtn;
        private Button _closeBtn;
        private float moveSpeed = 1;
        private float moveDistance = 450;
        private GameObject _content; 
        private NarrativePool _pool;

        public NarrativePanel(ScenarioManager scenarioManager) : base(new UIType(path))
        {
            _scenarioManager = scenarioManager;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _openBtn = UITool.TryGetChildComponentByName<Button>("OpenBtn");
            _closeBtn = UITool.TryGetChildComponentByName<Button>("CloseBtn");
            _closeBtn.gameObject.SetActive(false);

            _content = UITool.GetChildByName("Content");
            _pool = new NarrativePool(_content, 50);

            _rectTransformMyself = UITool.MyGameObject.GetComponent<RectTransform>();
            _openBtn.onClick.AddListener(() =>
            {               
                float end = _rectTransformMyself.position.y - moveDistance;
                _rectTransformMyself.DOMoveY(end, moveSpeed).OnComplete(() =>
                {
                    _openBtn.gameObject.SetActive(false);
                    _closeBtn.gameObject.SetActive(true);
                });
            });
            _closeBtn.onClick.AddListener(() =>
            {
                float end = _rectTransformMyself.position.y + moveDistance;
                _rectTransformMyself.DOMoveY(end, moveSpeed).OnComplete(() =>
                {
                    _openBtn.gameObject.SetActive(true);
                    _closeBtn.gameObject.SetActive(false);
                });
            });
        }

        /// <summary>
        /// ���һ��Ҫ�����������
        /// </summary>
        /// <param name="text"> �ı����� </param>
        /// <param name="target"> ������ָ��˭ </param>
        public void AddOneNarrativeLog(string text, GameObject target)
        {
            NarrativeLog newNarrativeLog = _pool.Get();
            newNarrativeLog.Init(text, target);
        }
    }
}