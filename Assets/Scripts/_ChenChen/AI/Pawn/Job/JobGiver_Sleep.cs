using ChenChen_Thing;
using ChenChen_UI;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Sleep : JobGiver
    {
        private static readonly float interval_time = 5;

        public JobGiver_Sleep(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, null, interval_time)
        {
        }

        private float sleepy_threshold = 0.3f; 
        private bool noBed;

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if (pawn.Info.Sleepiness.CurValue > sleepy_threshold * pawn.Info.Sleepiness.MaxValue) return null;

            return FindBed(pawn);
        }

        private TargetPtr FindBed(Pawn pawn)
        {
            var beds = ThingSystemManager.Instance.GetThingsInstance<Thing_Bed>();

            foreach (var bed in beds)
            {
                if (bed.Owner == pawn)
                {
                    noBed = false;
                    return new TargetPtr(bed.gameObject);
                }
            }

            foreach (var bed in beds)
            {
                if (bed.Owner == null)
                {
                    bed.Owner = pawn;
                    noBed = false;
                    return new TargetPtr(bed.gameObject);
                }
            }

            if (!noBed)
            {
                noBed = true;

                pawn.EmotionController.AddEmotion(EmotionType.distressed);
            }

            return null;
        }
    }
}
