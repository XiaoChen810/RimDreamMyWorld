﻿using UnityEngine;

namespace PawnStates
{
    public class PawnState_Move : StateBase
    {
        private Pawn pawn;
        private Vector2 targetPos;

        /// <summary>
        /// 改变移动目标，需设置目标点
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="targetPos"></param>
        public PawnState_Move(Pawn pawn, Vector2 targetPos) : base(pawn.StateMachine)
        {
            this.pawn = pawn;
            this.targetPos = targetPos;
        }

        public override void OnEnter()
        {
            // 设置目标点
            pawn.MoveControl.GoToHere(targetPos);

            // 设置人物状态 
            pawn.JobToDo(null);
            pawn.JobCanGet();
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点
            if (pawn.MoveControl.IsReach || Vector2.Distance(pawn.transform.position, targetPos) < 0.01)
            {
                return StateType.Success;
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
            Debug.Log("已中断 Move To" + targetPos);
            OnExit();
        }
    }
}
