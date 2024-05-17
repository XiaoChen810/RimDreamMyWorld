using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class LoadingPanel : PanelBase
    {
        public static readonly string path = "UI/Component/LoadingPanel";
        private AnimatorTool _animatorTool;
        private bool _isStart;
        private Slider _slider;

        public LoadingPanel(bool isStart, AnimatorTool animatorTool) : base(new UIType(path))
        {
            _isStart = isStart;
            _animatorTool = animatorTool;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Image background = UITool.GetChildByName("Background").GetComponent<Image>();
            _slider = UITool.TryGetChildComponentByName<Slider>("ProgressSlider");
            _animatorTool.ProgressSilder = _slider;
            if (_isStart)
            {
                //background.color = new Color(0, 0, 0, 0);
                //background.DOFade(1, 1);
            }
            else
            {
                _slider.gameObject.SetActive(false);
                background.color = new Color(0, 0, 0, 1);
                background.DOFade(0, 2);
            }
            OnExit(3);
        }
    }
}