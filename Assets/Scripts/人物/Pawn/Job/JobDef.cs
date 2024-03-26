using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class JobDef
    {
        public string jobName;
        public string jobDescription;
        public JobTag jobTag;

        public JobDef(string jobName, string jobDescription, JobTag jobType)
        {
            this.jobName = jobName;
            this.jobDescription = jobDescription;
            this.jobTag = jobType;
        }

        public static JobDef Idle = new JobDef("Idle", "站着", JobTag.Idle);
        public static JobDef Goto = new JobDef("Goto", "前往", JobTag.Misc);
    }


}
