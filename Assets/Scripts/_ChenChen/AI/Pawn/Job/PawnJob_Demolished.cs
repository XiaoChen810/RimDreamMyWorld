using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private readonly static float tick = 50;
        private Thing_Building targetComponent;
        private float _time;
        private float _timeOne;

        public PawnJob_Demolished(Pawn pawn, GameObject building) : base(pawn, tick,new TargetPtr(building))
        {
            float ability = pawn.Attribute.A_Construction.Value;
            float a = 1 - (ability / 20);
            _timeOne = Mathf.Lerp(1f, 10, a) / 2;
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            // 尝试获取蓝图
            targetComponent = target.GetComponent<Thing_Building>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            bool flag = pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物无法接取工作
            pawn.JobToDo(target.GameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 如果完成了拆除，状态机结束暂停，可以进入下一个状态
            if (targetComponent == null)
            {
                return StateType.Success;
            }

            if (targetComponent.MaxDurability <= 0)
            {
                return StateType.Success;
            }

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                // 执行工作
                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    targetComponent.OnDemolish(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            // 归还目标使用权限
            targetComponent.RevokePermission(pawn);

            OnExit();
        }
    }
}