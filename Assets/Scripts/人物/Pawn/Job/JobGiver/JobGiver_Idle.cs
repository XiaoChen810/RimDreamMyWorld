using System;

namespace ChenChen_AI
{
    public class JobGiver_Idle : ThinkNode_JobGiver
    {
        public int ticks = 50;

        public JobGiver_Idle()
        {
        }

        //public override ThinkNode DeepCopy(bool resolve = true)
        //{
        //    JobGiver_Idle jobGiver_Idle = (JobGiver_Idle)base.DeepCopy(resolve);
        //    jobGiver_Idle.ticks = this.ticks;
        //    return jobGiver_Idle;
        //}

        protected override Job TryGiveJob(Pawn pawn)
        {
            return new Job(JobDef.Idle);
        }
    }
}
