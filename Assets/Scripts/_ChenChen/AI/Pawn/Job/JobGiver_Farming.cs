using ChenChen_Thing;
using System;
using UnityEngine;
namespace ChenChen_AI
{
    public class JobGiver_Farming : JobGiver
    {
        private static readonly float interval_time = 0.1f;
        private static readonly string jobName = "种植";
        public JobGiver_Farming(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, interval_time)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            if(!pawn.Def.CanPlant) return null;
            GameObject obj = GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Farm);
            return obj == null ? null : new TargetPtr(obj);
        }
    }
}
