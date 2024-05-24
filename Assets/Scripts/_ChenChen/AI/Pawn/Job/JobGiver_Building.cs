using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Building : JobGiver
    {
        private static readonly float interval_time = 0;

        public JobGiver_Building(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if (!pawn.Def.CanBuild) return null;
            return ThingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.MarkBuilding, needFree: true);
        }
    }
}
