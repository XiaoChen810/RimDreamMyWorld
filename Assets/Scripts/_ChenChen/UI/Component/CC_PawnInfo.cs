using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class CC_PawnInfo : ObjectPtr
    {
        [SerializeField] private Button info_Btn;

        private Panel_PawnInfo _pawnInfoPanel = null;

        public override void Init(TargetPtr target)
        {
            base.Init(target);

            if(info_Btn != null && target.TryGetComponent<Pawn>(out Pawn pawn))
            {
                info_Btn.onClick.AddListener(() =>
                {
                    if(_pawnInfoPanel != null)
                    {
                        _pawnInfoPanel.OnExit();
                    }
                    _pawnInfoPanel = new Panel_PawnInfo(pawn);
                    PanelManager.Instance.AddPanel(_pawnInfoPanel, stopCurrentPanel: false, addToStack: false);
                });
            }
            else
            {
                Debug.LogError("找不到对应的组件");
            }
        }

        private void Update()
        {
            if(_pawnInfoPanel != null)
            {
                _pawnInfoPanel.UpdateSlider();
            }
        }

        private void OnDisable()
        {
            if (_pawnInfoPanel != null)
                _pawnInfoPanel.OnExit();
        }
    }
}