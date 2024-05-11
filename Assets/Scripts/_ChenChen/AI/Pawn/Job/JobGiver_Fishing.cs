using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Fishing : JobGiver
    {
        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetThingGenerated("钓鱼点", needFree: true);
        }
    }
}
