using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Building : JobGiver
    {
        public JobGiver_Building(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.MarkBuilding, needFree: true);
        }
    }
}
