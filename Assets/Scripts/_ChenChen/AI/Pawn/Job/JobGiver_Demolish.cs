using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Demolish : JobGiver
    {
        private static readonly float interval_time = 0.1f;
        private static readonly string jobName = "拆除";

        public JobGiver_Demolish(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if (!pawn.Def.CanBuild) return null;
            var list = ThingSystemManager.Instance.GetThingsInstance<Building>();
            foreach (var building in list)
            {
                if (building.LifeState == BuildingLifeStateType.MarkDemolished)
                {
                    return new TargetPtr(building.gameObject);
                }
            }
            return null;
        }
    }
}
