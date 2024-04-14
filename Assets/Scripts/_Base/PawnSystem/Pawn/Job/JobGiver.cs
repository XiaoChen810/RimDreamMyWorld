using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        public JobGiver() { }

        // ���Ի�ȡ����
        protected abstract GameObject TryGiveJob(Pawn pawn);

        // ���Է��������
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
