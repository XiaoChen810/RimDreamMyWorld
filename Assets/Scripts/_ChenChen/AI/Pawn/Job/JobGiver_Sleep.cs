using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Sleep : JobGiver
    {
        public JobGiver_Sleep(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        private bool noBed;

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if (pawn.Info.Sleepiness.CurValue > 0.3 * pawn.Info.Sleepiness.MaxValue) return null;

            //先找自己的床
            foreach(var bed in ThingSystemManager.Instance.GetThingsGenerated<Thing_Bed>())
            {
                if(bed.Owner == pawn)
                {
                    noBed = false;
                    return bed.gameObject;
                }
            }

            // 再找空床
            foreach (var bed in ThingSystemManager.Instance.GetThingsGenerated<Thing_Bed>())
            {
                if (bed.Owner == null)
                {
                    bed.Owner = pawn;
                    noBed = false;
                    return bed.gameObject;
                }
            }

            if (!noBed)
            {
                noBed = true;
                string narrative = $"{pawn.Def.PawnName} 想要睡觉，但没有床...";
                ScenarioManager.Instance.Narrative(narrative, pawn.gameObject);
            }

            return null;
        }
    }
}
