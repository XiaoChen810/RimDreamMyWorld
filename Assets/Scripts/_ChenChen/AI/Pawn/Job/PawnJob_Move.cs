using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Move : PawnJob
    {
        private readonly static float tick = 50;

        /// <summary>
        /// 改变移动目标，需设置目标点
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="targetPos"></param>
        public PawnJob_Move(Pawn pawn, Vector2 targetPos) : base(pawn, tick, new TargetPtr(targetPos))
        {
            this.pawn = pawn;
            this.Description = "移动";
        }

        public override bool OnEnter()
        {
            if (!pawn.MoveController.GoToHere(target.Positon))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(null);
            pawn.JobCanGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.MoveController.ReachDestination)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }


        public override void OnInterrupt()
        {
            pawn.JobDone();
        }
    }
}
