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

        private PawnInfoPanel _pawnInfoPanel = null;

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
                    _pawnInfoPanel = new PawnInfoPanel(pawn);
                    PanelManager.Instance.AddPanel(_pawnInfoPanel, stopCurrentPanel: false, addToStack: false);
                });
            }
            else
            {
                Debug.LogError("�Ҳ�����Ӧ�����");
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