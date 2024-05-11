using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Building : JobGiver
    {
        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.MarkBuilding, needFree: true);
        }
    }
}
