using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Loading : PanelBase
    {
        public static readonly string path = "UI/Component/LoadingPanel";
        private AnimatorTool _animatorTool;
        private bool _isStart;

        public Panel_Loading(bool isStart, AnimatorTool animatorTool) : base(new UIType(path))
        {
            _isStart = isStart;
            _animatorTool = animatorTool;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Image background = UITool.GetChildByName("Background").GetComponent<Image>();
            if (_isStart)
            {
                //background.color = new Color(0, 0, 0, 0);
                //background.DOFade(1, 1);
            }
            else
            {
                PanelManager.Instance.RemovePanel(this);
            }
        }
    }
}