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
         /* 使用样例
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;
         */
        public override bool OnEnter()
        {
            // 尝试取得权限，预定当前工作，标记目标被使用
            if (target.IsGameObject && target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
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
         /* 使用样例
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;
         */
        public override StateType OnUpdate()
        {
            if(target != null && target.IsGameObject && target.GameObject == null)
            {
                return StateType.Failed;
            }

            // 继续执行其他逻辑
            return StateType.Doing;
        }

        /// <summary>
        /// 归还目标使用权限，如果有的话
        /// 设置人物状态
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            // 检查目标是否为空以及目标的 GameObject 是否已被销毁
            if (target != null && target.IsGameObject && target.GameObject != null)
            {
                // 尝试获取 PermissionBase 组件并归还使用权限
                if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
                {
                    if (!per.RevokePermission(pawn))
                    {
                        Debug.Log($"归还目标使用权限失败：{pawn.name} to {per.name}");
                    }
                }
            }

            target = null;
            // 设置人物状态
            pawn.JobDone();
        }
    }
}