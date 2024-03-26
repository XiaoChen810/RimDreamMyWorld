using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChenChen_AI
{
    public struct ThinkResult : IEquatable<ThinkResult>
    {
        private Job jobInt;

        private ThinkNode sourceNodeInt;

        private JobTag? tag;

        private bool fromQueue;

        /// <summary>
        /// 想法结果
        /// </summary>
        /// <param name="job"></param>
        /// <param name="sourceNode"></param>
        /// <param name="tag"></param>
        /// <param name="fromQueue">是否从队列中取来的工作</param>
        public ThinkResult(Job job, ThinkNode sourceNode, JobTag? tag = null, bool fromQueue = false)
        {
            this.jobInt = job;
            this.sourceNodeInt = sourceNode;
            this.tag = tag;
            this.fromQueue = fromQueue;
        }
        public Job Job
        {
            get
            {
                return this.jobInt;
            }
        }

        public ThinkNode SourceNode
        {
            get
            {
                return this.sourceNodeInt;
            }
        }

        public JobTag? Tag
        {
            get
            {
                return this.tag;
            }
        }

        public bool FromQueue
        {
            get
            {
                return this.fromQueue;
            }
        }


        public static ThinkResult NoJob
        {
            get
            {
                return new ThinkResult(null, null, null, false);
            }
        }

        public bool Equals(ThinkResult other)
        {
            if (this.jobInt == other.jobInt && this.sourceNodeInt == other.sourceNodeInt)
            {
                JobTag? jobTag = this.tag;
                JobTag valueOrDefault = jobTag.GetValueOrDefault();
                JobTag? jobTag2 = other.tag;
                if (valueOrDefault == jobTag2.GetValueOrDefault() && jobTag != null == (jobTag2 != null))
                {
                    return this.fromQueue == other.fromQueue;
                }
            }
            return false;
        }
    }
}
