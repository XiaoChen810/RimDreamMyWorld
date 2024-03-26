using System;
using System.Collections.Generic;

namespace ChenChen_AI
{
    public class JobGiver_MoveToThere : ThinkNode_JobGiver
    {
        public JobGiver_MoveToThere() { }

        protected override Job TryGiveJob(Pawn pawn)
        {
            //检查角色是否可以移动
            if (!pawn.MoveControl.CanMove)
            {
                return null;
            }
            return new Job(JobDef.Goto, pawn.transform.position);
        }
    }
}
