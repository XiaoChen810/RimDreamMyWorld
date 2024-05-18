using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Fishing : JobGiver
    {
        public JobGiver_Fishing(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetThingGenerated("钓鱼点", needFree: true);
        }
    }
}
