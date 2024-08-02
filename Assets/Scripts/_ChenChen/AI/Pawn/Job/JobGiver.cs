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
        protected float probability = 1;
        protected Action<GameObject> onGetJobSuccessly;

        private float lastGiverTime;

        public JobGiver(Action<GameObject> onGetJobSuccessly, string jobName = null, float intervalTime = 5, float probability = 1)
        {
            this.onGetJobSuccessly = onGetJobSuccessly;
            this.intervalTime = intervalTime;
            this.probability = probability;

            if (jobName != null )
            {
                JobName = jobName;
            }            
        }

        protected abstract GameObject TryGiveJob(Pawn pawn);

        public virtual GameObject TryIssueJobPackage(Pawn pawn)
        {
            if (Time.time < lastGiverTime + intervalTime) return null;
            if (UnityEngine.Random.value > probability) return null;
            lastGiverTime = Time.time;
            GameObject job = TryGiveJob(pawn);
            if (job == null)
            {
                return null;
            }
            onGetJobSuccessly?.Invoke(job);
            return job;
        }
    }
}
