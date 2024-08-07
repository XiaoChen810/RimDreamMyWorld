using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Move : PawnJob
    {
        private readonly static float tick = 500;

        public PawnJob_Move(Pawn pawn, Vector2 targetPos, Urgency urgency = Urgency.Normal) : base(pawn, tick, new TargetPtr(targetPos))
        {
            this.pawn = pawn;
            this.Description = " 移动";          
            this.urgency = urgency;
        }

        /// <summary>
        /// 移动到设定位置，并附带到达时的方法
        /// </summary>
        /// <param name="onReach"> 当到达时，在返回Success前，触发 </param>
        public PawnJob_Move(Pawn pawn, Vector2 targetPos, Action onReach,Urgency urgency = Urgency.Normal) : base(pawn, tick, new TargetPtr(targetPos))
        {
            this.pawn = pawn;
            this.Description = " 移动";
            this.onReach = onReach;
            this.urgency = urgency;
        }

        Urgency urgency = Urgency.Normal;
        Action onReach = null;

        public override bool OnEnter()
        {
            if (!pawn.MoveController.GoToHere(target.PositonA, urgency))
            {
                DebugLogDescription = ($"无法移动到目标点: {target.PositonA} ");
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
                if(onReach != null)
                {
                    onReach.Invoke();
                }
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
