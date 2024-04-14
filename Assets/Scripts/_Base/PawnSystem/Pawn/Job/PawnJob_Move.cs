using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Move : PawnJob
    {
        private Vector2 targetPos;

        /// <summary>
        /// 改变移动目标，需设置目标点
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="targetPos"></param>
        public PawnJob_Move(Pawn pawn, Vector2 targetPos) : base(pawn)
        {
            this.pawn = pawn;
            this.targetPos = targetPos;
        }

        public override bool OnEnter()
        {
            // 设置目标点
            pawn.MoveControl.GoToHere(targetPos);

            // 设置人物状态 
            pawn.JobToDo(null);
            pawn.JobCanGet();

            return true;
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
            Debug.Log($"{pawn.name} interrupt Move To" + targetPos);
            pawn.JobDone();
        }
    }
}
