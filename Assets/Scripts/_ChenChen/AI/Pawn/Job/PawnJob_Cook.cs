using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cook : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;

        private Thing_CookingStation _cookingStation;
        protected PawnJob_Cook(Pawn pawn,GameObject cookingStation) : base(pawn, tick, new TargetPtr(cookingStation))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.TryGetComponent<Thing_CookingStation>(out _cookingStation))
            {
                DebugLogDescription = "获取组件失败";
                return false;
            }

            if(!pawn.MoveController.GoToHere(target.Positon,Urgency.Urge,pawn.WorkRange))
            {
                DebugLogDescription = "无法移动到目标位置：" + target.Positon;
                return false;
            }

            pawn.JobToDo(target.GameObject);
            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (_cookingStation.IsSuccess)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}