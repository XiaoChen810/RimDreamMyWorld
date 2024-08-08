using ChenChen_Core;
using ChenChen_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenChen_Thing
{
    // 灶台   
    public class Thing_Stove : Thing_Tool, IOperate
    {
        public override IReadOnlyList<(string, int)> RequiredForMade
        {
            get
            {
                List<(string, int)> res = new List<(string, int)>();
                if (CurrentMadeDef != null)
                {
                    foreach (Need requiredMaterial in CurrentMadeDef.requiredMaterials)
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
                var res = XmlLoader.Instance.Get<FoodDef>(XmlLoader.Def_Food).Where(x => x.canMade).ToList();
                return res.Cast<StuffDef>().ToList();
            }
        }

        public override bool IsFullRequiredForMade
        {
            get
            {
                if (CurrentMadeDef != null && NumberOfWaitingToMade != 0)
                {
                    return RequiredForMade.Count == 0;
                }
                return true;
            }
        }

        public bool IsOpenMenu { get; set; }

        public override void OpenWorkMenu()
        {
            if (!IsOpenMenu)
            {
                IsOpenMenu = true;
                PanelManager.Instance.AddPanel(new Panel_Table(this, () => { IsOpenMenu = false; }));
            }
            DetailView.ClosePanel();
        }

        #region - IOperate -
        public bool IsWaitToOperate => NumberOfWaitingToMade > 0 && IsFullRequiredForMade;

        public bool IsCompleteOperate => NumberOfWaitingToMade == 0;

        public Vector3 OperationPosition => workPosition.position;

        public float OnceTime => CurrentMadeDef != null ? CurrentMadeDef.workload : 99;

        public void Operate()
        {
            if (NumberOfWaitingToMade <= 0)
            {
                throw new InvalidOperationException("要制作的数量小于等于0");
            }
            NumberOfWaitingToMade--;
            ThingSystemManager.Instance.GenerateItem(CurrentMadeDef, producePositon.position, 1);
        }
        #endregion
    }
}
