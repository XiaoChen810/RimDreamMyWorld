using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class JobGiver
    {
        protected Action<GameObject> onGetJobSuccessly;

        // ���ȼ�
        public int Priority = 1;

        // ����
        public string JobName = string.Empty;

        // ���ʱ��
        protected float intervalTime = 5;

        // ���ʴ���
        protected float probability = 1;

        // ��һ�η���ʱ��
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

        // ���Ի�ȡ����
        protected abstract GameObject TryGiveJob(Pawn pawn);

        // ���Է��������
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
