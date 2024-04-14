using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        public JobGiver() { }

        // 尝试获取任务
        protected abstract GameObject TryGiveJob(Pawn pawn);

        // 尝试发布任务包
        public virtual GameObject TryIssueJobPackage(Pawn pawn)
        {
            GameObject job = this.TryGiveJob(pawn);
            if (job == null)
            {
                return null;
            }
            return job;
        }
    }
}
