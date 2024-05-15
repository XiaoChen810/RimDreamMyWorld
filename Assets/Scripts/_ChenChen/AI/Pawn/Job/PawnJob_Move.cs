using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Move : PawnJob
    {
        private readonly static float tick = 50;
        private Vector2 targetPos;

        /// <summary>
        /// 改变移动目标，需设置目标点
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="targetPos"></param>
        public PawnJob_Move(Pawn pawn, Vector2 targetPos) : base(pawn, tick)
        {
            this._pawn = pawn;
            this.targetPos = targetPos;
        }

        public override bool OnEnter()
        {
            // 设置目标点
            if (!_pawn.MoveController.GoToHere(targetPos))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }
            // 设置人物状态 
            _pawn.JobToDo(null);
            _pawn.JobCanGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点
            if (_pawn.MoveController.ReachDestination)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 设置人物状态 
            _pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            _pawn.JobDone();
        }
    }
}
