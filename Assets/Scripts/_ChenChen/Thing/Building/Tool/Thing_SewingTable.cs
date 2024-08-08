using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Core;
using System.Linq;

namespace ChenChen_Thing
{
    // 缝纫台
    public class Thing_SewingTable : Thing_Tool , IOperate
    {
        public ApparelDef CurrentApparelDef
        {
            get
            {
                return CurrentMadeDef as ApparelDef;
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

        public override List<StuffDef> AllCanDo
        {
            get
            {
                return XmlLoader.Instance.Get<ApparelDef>(XmlLoader.Def_Apparel).Cast<StuffDef>().ToList();
            }
        }

        public override bool IsFullRequiredForMade
        {
            get
            {
                if(CurrentApparelDef != null && NumberOfWaitingToMade != 0)
                {
                    return RequiredForMade.Count == 0;
                } 
                return true;
            }
        }

        public override void OpenWorkMenu()
        {
            if (!IsOpenMenu)
            {
                IsOpenMenu = true;
                PanelManager.Instance.AddPanel(new Panel_Table(this, () => { IsOpenMenu = false; }));
            }
            DetailView.ClosePanel();
        }

        public bool IsOpenMenu { get; set; }

        #region - IOperate -
        public bool IsWaitToOperate => NumberOfWaitingToMade > 0 && IsFullRequiredForMade;

        public bool IsCompleteOperate => NumberOfWaitingToMade == 0;

        public Vector3 OperationPosition => workPosition.position;

        public float OnceTime => CurrentApparelDef != null ? CurrentApparelDef.workload : 99;

        public void Operate()
        {
            if(NumberOfWaitingToMade <= 0)
            {
                throw new InvalidOperationException("要制作的数量小于等于0");
            }
            NumberOfWaitingToMade--;
            ThingSystemManager.Instance.GenerateItem(CurrentApparelDef, producePositon.position, 1);
        }
        #endregion
    }
}
