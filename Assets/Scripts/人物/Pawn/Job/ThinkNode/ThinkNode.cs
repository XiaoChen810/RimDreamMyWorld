using System;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class ThinkNode
    {
        /// <summary>
        /// 想法
        /// </summary>
        protected ThinkNode() { }

        protected float priority = -1f;

        public virtual float GetPriority(Pawn pawn)
        {
            if (this.priority < 0f)
            {
                Debug.Log("ThinkNode_PrioritySorter has child node which didn't give a priority: " + this);
                return 0f;
            }
            return this.priority;
        }

        /// <summary>
        /// 尝试发布工作包
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public abstract ThinkResult TryIssueJobPackage(Pawn pawn);

        //public virtual ThinkNode DeepCopy(bool resolve = true)
        //{
        //    ThinkNode thinkNode = (ThinkNode)Activator.CreateInstance(base.GetType());
        //    //for (int i = 0; i < this.subNodes.Count; i++)
        //    //{
        //    //    thinkNode.subNodes.Add(this.subNodes[i].DeepCopy(resolve));
        //    //}
        //    //thinkNode.priority = this.priority;
        //    //thinkNode.leaveJoinableLordIfIssuesJob = this.leaveJoinableLordIfIssuesJob;
        //    //thinkNode.uniqueSaveKeyInt = this.uniqueSaveKeyInt;
        //    //if (resolve)
        //    //{
        //    //    thinkNode.ResolveSubnodesAndRecur();
        //    //}
        //    //ThinkTreeKeyAssigner.AssignSingleKey(thinkNode, 0);
        //    return thinkNode;
        //}
    }
}
