using ChenChen_Core;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class PawnJob : StateBase
    {
        /// <summary>
        /// 做这个工作的棋子
        /// </summary>
        protected Pawn pawn;

        /// <summary>
        /// 工作目标
        /// </summary>
        protected TargetPtr target;

        public PawnJob(Pawn pawn, float maxTick, TargetPtr target, StateBase next = null) : base(pawn.StateMachine, maxTick, next)
        {
            this.pawn = pawn;
            this.target = target;
        }

        /// <summary>
        /// 尝试取得权限，预定当前工作，标记目标被使用
        /// </summary>
        /// <returns></returns>
        public override bool OnEnter()
        {
            if (target != null && target.TargetA != null)
            {
                if (target.TargetA.TryGetComponent<IGrant>(out var grantA))
                {
                    grantA.GetPermission(pawn);
                    if (grantA.UserPawn != pawn)
                    {
                        DebugLogDescription = ($"目标已经 {grantA.UserPawn.name} 被使用");
                        return false;
                    }
                }
            }

            if (target != null && target.TargetB != null)
            {
                if (target.TargetB.TryGetComponent<IGrant>(out var grantB))
                {
                    grantB.GetPermission(pawn);
                    if (grantB.UserPawn != pawn)
                    {
                        DebugLogDescription = ($"目标已经 {grantB.UserPawn.name} 被使用");
                        return false;
                    }
                }
            }


            return true;
        }

        public override StateType OnUpdate()
        {
            return StateType.Doing;
        }

        /// <summary>
        /// 归还目标使用权限，如果有的话
        /// 设置人物状态
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            if (target != null && target.TargetA != null)
            {
                if (target.TargetA.TryGetComponent<IGrant>(out var grantA))
                {
                    grantA.RevokePermission(pawn);
                }
            }

            if (target != null && target.TargetB != null)
            {
                if (target.TargetB.TryGetComponent<IGrant>(out var grantB))
                {
                    grantB.RevokePermission(pawn);
                }
            }

            target = null;
            pawn.JobDone();
        }
    }
}