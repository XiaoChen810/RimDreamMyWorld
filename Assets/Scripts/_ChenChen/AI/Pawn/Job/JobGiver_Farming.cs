using ChenChen_BuildingSystem;
using UnityEngine;
namespace ChenChen_AI
{
    public class JobGiver_Farming : JobGiver
    {
        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return GameManager.Instance.WorkSpaceTool.GetAWorkSpace(WorkSpaceType.Farm);
        }
    }
}
