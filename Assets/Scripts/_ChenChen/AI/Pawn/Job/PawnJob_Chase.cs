using ChenChen_BuildingSystem;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private Pawn targetPawnComponent;
        public PawnJob_Chase(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null)
            {
                DebugLogDescription = ("目标无Pawn组件");
                return false;
            }
            // 设置人物无法接取工作
            pawn.JobToDo(target.GameObject);
            // 设置人物目标点，前往目标，走过去
            pawn.MoveController.GoToHere(target.Positon, Urgency.Normal, pawn.AttackRange);
            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断目标是否到达攻击距离
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();               

                // 进入下一个状态
                if (!targetPawnComponent.Info.IsDead)
                {
                    IsSuccess = true;
                    pawn.StateMachine.NextState = new PawnJob_Battle(pawn, target.GameObject);
                    return StateType.Success;
                }
            }

            return StateType.Doing;
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}