using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class LoadingPanel : PanelBase
    {
        public static readonly string path = "UI/Component/LoadingPanel";
        private bool _isStart;
        public LoadingPanel(bool isStart) : base(new UIType(path))
        {
            _isStart = isStart;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Image background = UITool.GetChildByName("Background").GetComponent<Image>();
            if (_isStart)
            {
                background.color = new Color(0, 0, 0, 0);
                background.DOFade(1, 1);
            }
            else
            {
                background.color = new Color(0, 0, 0, 1);
                background.DOFade(0, 2);
            }
        }
    }
}