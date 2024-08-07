using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Fishing : JobGiver
    {
        private static readonly float interval_time = 1f;
        private static readonly string jobName = "钓鱼";
        public JobGiver_Fishing(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if(!pawn.Def.CanForaging) return null;
            GameObject obj = ThingSystemManager.Instance.GetThingInstance("钓鱼点");
            return obj == null ? null : new TargetPtr(obj);
        }
    }
}
