using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        public int Priority = 1;
        public string JobName = string.Empty;

        protected float intervalTime = 5;
        protected Action<GameObject> onGetJobSuccessly;

        private float giverTimer = 0;

        public JobGiver(Action<GameObject> onGetJobSuccessly, string jobName = null, float intervalTime = 5)
        {
            this.onGetJobSuccessly = onGetJobSuccessly;
            this.intervalTime = intervalTime;

            if (jobName != null )
            {
                JobName = jobName;
            }            
        }

        protected abstract GameObject TryGiveJob(Pawn pawn);

        public virtual GameObject TryIssueJobPackage(Pawn pawn)
        {
            giverTimer += Time.deltaTime;
            if (giverTimer <= intervalTime) return null;
            giverTimer = 0;

            GameObject job = TryGiveJob(pawn);
            if (job == null) return null;
            onGetJobSuccessly?.Invoke(job);
            return job;
        }
    }
}
