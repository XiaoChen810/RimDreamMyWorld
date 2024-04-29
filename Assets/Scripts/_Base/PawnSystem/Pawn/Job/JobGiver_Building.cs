using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Building : JobGiver
    {
        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return BuildingSystemManager.Instance.GetThingGenerated(BuildingLifeStateType.WaitingBuilt, needFree: true);
        }
    }
}
