using ChenChen_BuildingSystem;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private GameObject target;
        private Pawn targetPawnComponent;
        public PawnJob_Chase(Pawn pawn, GameObject target) : base(pawn, tick)
        {
            this.target = target;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null) return false;

            // 设置人物无法接取工作
            pawn.JobToDo(target);
            // 设置人物目标点，前往目标，走过去
            pawn.MoveControl.GoToHere(target, Urgency.Normal, pawn.AttackRange);
            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断目标是否到达攻击距离
            if (pawn.MoveControl.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();               

                // 进入下一个状态
                if (!targetPawnComponent.IsDead)
                {
                    IsSuccess = true;
                    pawn.StateMachine.NextState = new PawnJob_Battle(pawn, target);
                    return StateType.Success;
                }
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 设置人物状态
            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}