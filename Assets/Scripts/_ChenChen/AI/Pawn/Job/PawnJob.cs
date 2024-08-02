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
            if (target.IsGameObject && target.GameObject == null)
            {
                DebugLogDescription = ("目标为空");
                return false;
            }

            if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
            {
                if (!per.GetPermission(pawn))
                {
                    DebugLogDescription = ("目标已经其他人被使用");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断目标是否还存在
        /// </summary>
        /// <returns></returns>
        public override StateType OnUpdate()
        {
            if(target != null && target.IsGameObject && target.GameObject == null)
            {
                return StateType.Failed;
            }

            return StateType.Doing;
        }

        /// <summary>
        /// 归还目标使用权限，如果有的话
        /// 设置人物状态
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            if (target != null && target.IsGameObject && target.GameObject != null)
            {
                if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
                {
                    if (!per.RevokePermission(pawn))
                    {
                        Debug.Log($"归还目标使用权限失败：{pawn.name} to {per.name}");
                    }
                }
            }

            target = null;
            pawn.JobDone();
        }
    }
}