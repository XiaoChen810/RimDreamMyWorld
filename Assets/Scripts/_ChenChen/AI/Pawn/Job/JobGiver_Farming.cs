using ChenChen_Thing;
using System;
using UnityEngine;
namespace ChenChen_AI
{
    public class JobGiver_Farming : JobGiver
    {
        private static readonly float interval_time = 0;
        private static readonly string jobName = "种植";
        public JobGiver_Farming(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            if(!pawn.Def.CanPlant) return null;
            return GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Farm);
        }
    }
}
