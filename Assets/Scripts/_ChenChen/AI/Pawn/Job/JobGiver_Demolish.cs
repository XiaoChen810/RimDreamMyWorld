using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Demolish : JobGiver
    {
        public JobGiver_Demolish(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.MarkDemolished, needFree: true);
        }
    }
}
