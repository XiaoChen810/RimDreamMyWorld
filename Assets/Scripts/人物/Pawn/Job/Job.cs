using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class Job 
    {
        public string jobName;
        public string jobDescription;
        public JobTag jobTag;

        /// <summary>
        /// �ƶ��㣬�����ص�
        /// </summary>
        public Vector3 targetA;

        public Job()
        {
        }

        public Job(JobDef jobDef)
        {
            this.jobName = jobDef.jobName;
            this.jobDescription = jobDef.jobDescription;
            this.jobTag = jobDef.jobTag;
        }

        public Job(JobDef jobDef,Vector3 targetA)
        {
            this.jobName = jobDef.jobName;
            this.jobDescription = jobDef.jobDescription + targetA.ToString();
            this.jobTag = jobDef.jobTag;
            this.targetA = targetA;
        }
    }
}