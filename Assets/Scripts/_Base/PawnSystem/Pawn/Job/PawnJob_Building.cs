using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Building : PawnJob
    {
        private GameObject target;
        private Thing_Building curTargetComponent;
        private float _time;

        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public PawnJob_Building(Pawn pawn, GameObject target = null) : base(pawn)
        {
            this.target = target;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;

            // 尝试获取蓝图
            curTargetComponent = target.GetComponent<Thing_Building>();
            if (curTargetComponent == null)
            {
                Debug.LogWarning("The Building Don't Have Building Component");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!curTargetComponent.GetPrivilege(pawn))
            {
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            if (!pawn.MoveControl.GoToHere(target.transform.position, Urgency.Urge, pawn.WorkRange))
            {
                Debug.LogWarning("The building can't arrive");
                return false;
            }

            // 设置人物无法接取工作
            pawn.JobToDo(target);

            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (pawn.MoveControl.reachedDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                _time += Time.deltaTime;
                if(_time > 2)
                {
                    curTargetComponent.Build(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (curTargetComponent.NeedWorkload <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Debug.Log($"{pawn.name} 已经完成建造: " + curTargetComponent.name);
            // 归还目标使用权限
            curTargetComponent.RevokePrivilege(pawn);

            // 设置人物状态
            pawn.JobDone();

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            curTargetComponent.Interpret();
            OnExit();
        }
    }
}
