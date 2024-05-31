using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Escape : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;

        public PawnJob_Escape(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //逻辑
            Vector3 dir = pawn.transform.position - target.Positon;
            dir.Normalize();
            dir *= 10;
            Vector2 pos = pawn.transform.position + dir;
            if (!pawn.MoveController.GoToHere(pos, Urgency.Urge))
            {
                DebugLogDescription = "目标点不可达" + pos;
                return false;
            }

            pawn.JobCannotGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //逻辑
            if (pawn.MoveController.ReachDestination)
            {
                return StateType.Success;
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            //逻辑
            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}