using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        protected Action<GameObject> onGetJobSuccessly;

        public JobGiver(Action<GameObject> onGetJobSuccessly)
        {
            this.onGetJobSuccessly = onGetJobSuccessly;
        }

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
            onGetJobSuccessly?.Invoke(job);
            return job;
        }
    }
}
