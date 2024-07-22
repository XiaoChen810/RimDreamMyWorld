using ChenChen_Thing;
using ChenChen_UI;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Sleep : JobGiver
    {
        private static readonly float interval_time = 5;

        public JobGiver_Sleep(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, null, interval_time)
        {
        }

        private float sleepy_threshold = 0.3f;    // 困意的阈值 0 ~ 1。
        private bool noBed;

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            // 如果困了
            if (pawn.Info.Sleepiness.CurValue > sleepy_threshold * pawn.Info.Sleepiness.MaxValue) return null;

            return FindBed(pawn);
        }

        private GameObject FindBed(Pawn pawn)
        {
            //先找自己的床
            foreach (var bed in ThingSystemManager.Instance.GetThingsInstance<Thing_Bed>(BuildingLifeStateType.FinishedBuilding))
            {
                if (bed.Owner == pawn)
                {
                    noBed = false;
                    return bed.gameObject;
                }
            }

            // 再找空床
            foreach (var bed in ThingSystemManager.Instance.GetThingsInstance<Thing_Bed>(BuildingLifeStateType.FinishedBuilding))
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

                //添加焦虑
                pawn.EmotionController.AddEmotion(EmotionType.distressed);
            }

            return null;
        }
    }
}
