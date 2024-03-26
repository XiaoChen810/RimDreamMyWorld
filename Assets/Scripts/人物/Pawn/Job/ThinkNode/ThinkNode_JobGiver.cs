using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    /// <summary>
    /// 工作的想法
    /// </summary>
    public abstract class ThinkNode_JobGiver : ThinkNode
    {
        protected ThinkNode_JobGiver() { }

        protected abstract Job TryGiveJob(Pawn pawn);

        public override ThinkResult TryIssueJobPackage(Pawn pawn)
        {
            Job job = this.TryGiveJob(pawn);
            if (job == null)
            {
                return ThinkResult.NoJob;
            }
            return new ThinkResult(job, this, null, false);
        }
    }
}