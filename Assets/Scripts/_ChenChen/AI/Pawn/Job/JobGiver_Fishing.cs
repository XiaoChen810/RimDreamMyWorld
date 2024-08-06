using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Fishing : JobGiver
    {
        private static readonly float interval_time = 1f;
        private static readonly string jobName = "钓鱼";
        public JobGiver_Fishing(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if(!pawn.Def.CanForaging) return null;
            return ThingSystemManager.Instance.GetThingInstance("钓鱼点");
        }
    }
}
