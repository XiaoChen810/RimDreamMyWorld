using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        protected Action<GameObject> onGetJobSuccessly;

        // 优先级
        public int Priority = 1;

        // 名字
        public string JobName = string.Empty;

        // 间隔时间
        protected float intervalTime = 5;

        // 概率触发
        protected float probability = 1;

        // 上一次分配时间
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

        // 尝试获取任务
        protected abstract GameObject TryGiveJob(Pawn pawn);

        // 尝试发布任务包
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
