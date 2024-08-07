using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Building : JobGiver
    {
        private static readonly float interval_time = 0.1f;
        private static readonly string jobName = "НЈдь";

        public JobGiver_Building(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if (!pawn.Def.CanBuild) return null;
            var list = ThingSystemManager.Instance.GetThingsInstance<Building>();
            foreach (var building in list)
            {
                if (building.LifeState == BuildingLifeStateType.MarkBuilding && building.RequiredMaterialsLoadFull)
                {
                    return new TargetPtr(building.gameObject);
                }
            }
            return null;
        }
    }
}
