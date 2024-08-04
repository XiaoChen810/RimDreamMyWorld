using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Demolish : JobGiver
    {
        private static readonly float interval_time = 0.1f;
        private static readonly string jobName = "拆除";

        public JobGiver_Demolish(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if (!pawn.Def.CanBuild) return null;
            return ThingSystemManager.Instance.GetThingInstance(BuildingLifeStateType.MarkDemolished, needFree: true);
        }
    }
}
