using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Escape : PawnJob
    {
        private readonly static float tick = 500;

        public PawnJob_Escape(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
            this.Description = "正在逃离" + target.name;
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

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
            if (pawn.MoveController.ReachDestination)
            {
                return StateType.Success;
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}