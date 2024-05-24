using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        protected Action<GameObject> onGetJobSuccessly;
        protected float intervalTime = 5;
        private float lastGiverTime;

        public JobGiver(Action<GameObject> onGetJobSuccessly, float intervalTime)
        {
            this.onGetJobSuccessly = onGetJobSuccessly;
            this.intervalTime = intervalTime;
        }

        // ���Ի�ȡ����
        protected abstract GameObject TryGiveJob(Pawn pawn);

        // ���Է��������
        public virtual GameObject TryIssueJobPackage(Pawn pawn)
        {
            if (Time.time < lastGiverTime + intervalTime) return null;

            GameObject job = this.TryGiveJob(pawn);
            if (job == null)
            {
                return null;
            }
            onGetJobSuccessly?.Invoke(job);
            lastGiverTime = Time.time;
            return job;
        }
    }
}
