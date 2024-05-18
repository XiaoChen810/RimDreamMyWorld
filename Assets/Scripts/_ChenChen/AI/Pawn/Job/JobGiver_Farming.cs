using ChenChen_BuildingSystem;
using System;
using UnityEngine;
namespace ChenChen_AI
{
    public class JobGiver_Farming : JobGiver
    {
        public JobGiver_Farming(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Farm);
        }
    }
}
