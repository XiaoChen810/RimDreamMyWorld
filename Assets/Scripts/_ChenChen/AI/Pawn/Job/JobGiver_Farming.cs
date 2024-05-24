using ChenChen_BuildingSystem;
using System;
using UnityEngine;
namespace ChenChen_AI
{
    public class JobGiver_Farming : JobGiver
    {
        private static readonly float interval_time = 0;
        public JobGiver_Farming(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if(!pawn.Def.CanPlant) return null;
            return GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Farm);
        }
    }
}
