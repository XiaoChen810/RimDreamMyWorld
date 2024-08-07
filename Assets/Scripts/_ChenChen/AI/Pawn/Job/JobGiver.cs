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
        protected Action<TargetPtr> onGetJobSuccessly;

        private float giverTimer = 0;

        public JobGiver(Action<TargetPtr> onGetJobSuccessly, string jobName = null, float intervalTime = 5)
        {
            this.onGetJobSuccessly = onGetJobSuccessly;
            this.intervalTime = intervalTime;

            if (jobName != null )
            {
                JobName = jobName;
            }            
        }

        protected abstract TargetPtr TryGiveJob(Pawn pawn);

        public virtual TargetPtr TryIssueJobPackage(Pawn pawn)
        {
            giverTimer += Time.deltaTime;
            if (giverTimer <= intervalTime) return null;
            giverTimer = 0;

            TargetPtr targetPtr = TryGiveJob(pawn);
            if (targetPtr == null) return null;

            onGetJobSuccessly?.Invoke(targetPtr);
            return targetPtr;
        }
    }
}
