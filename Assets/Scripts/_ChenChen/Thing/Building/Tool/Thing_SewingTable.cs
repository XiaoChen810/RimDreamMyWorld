using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Core;

namespace ChenChen_Thing
{
    // 缝纫台
    public class Thing_SewingTable : Thing_Tool , IOperate
    {
        [SerializeField] private ApparelDef currentApparelDef = null;   // 当前制作中的衣服
        [SerializeField] private int numberOfWaitingToMade = 0;         // 还需要制作的数量
        [SerializeField] private Transform workPosition;                // 工作地点
        [SerializeField] private float workOnceTime = 5;                // 工作一次所需时间 
        [SerializeField] private Transform producePositon;              // 产出地点

        public ApparelDef CurrentApparelDef
        {
            get
            {
                return currentApparelDef;
            }
            set
            {
                currentApparelDef = value;
            }
        }

        public int NumberOfWaitingToMade
        {
            get
            {
                return numberOfWaitingToMade;
            }
            set
            {         
                numberOfWaitingToMade = Mathf.Clamp(value, 0, 99);
                Debug.Log("设置了要制作的数量" + numberOfWaitingToMade);
            }
        }

        public override IReadOnlyList<(string, int)> RequiredForMade
        {
            get
            {
                List<(string, int)> res = new List<(string, int)>();
                if(CurrentApparelDef != null)
                {
                    foreach (Need requiredMaterial in CurrentApparelDef.requiredMaterials)
                    {
                        int had = Bag.ContainsKey(requiredMaterial.label) ? Bag[requiredMaterial.label] : 0;
                        int required = requiredMaterial.numbers;
                        int need = required - had;
                        if (need != 0)
                        {
                            res.Add((requiredMaterial.label, need));
                        }
                    }
                }
                return res;
            }
        }

        public override bool IsFullRequiredForMade => base.IsFullRequiredForMade;

        public bool IsOpenMenu { get; set; }

        public override void OpenWorkMenu()
        {
            if (!IsOpenMenu)
            {
                IsOpenMenu = true;
                PanelManager.Instance.AddPanel(new Panel_SewingTable(this));
            }
            DetailView.ClosePanel();
        }

        #region - IOperate -
        public bool IsWaitToOperate => NumberOfWaitingToMade > 0 && IsFullRequiredForMade;

        public Vector3 OperationPosition => workPosition.position;

        public float OnceTime => workOnceTime;

        public void Operate()
        {
            if(NumberOfWaitingToMade <= 0)
            {
                throw new InvalidOperationException("要制作的数量小于等于0");
            }
            NumberOfWaitingToMade--;
            ThingSystemManager.Instance.TryGenerateItem(CurrentApparelDef, producePositon.position, 1);
        }
        #endregion
    }
}
