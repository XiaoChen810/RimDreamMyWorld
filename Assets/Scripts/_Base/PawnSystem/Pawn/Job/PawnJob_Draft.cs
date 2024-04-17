using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Draft : PawnJob
    {
        private readonly static float tick = 50;

        public PawnJob_Draft(Pawn pawn,bool openOrclose) : base(pawn, tick)
        {
            pawn.IsDrafted = openOrclose;
        }

        public override bool OnEnter()
        {
            pawn.JobDoing();
            pawn.JobCanGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.IsDrafted)
            {
                return StateType.Doing;
            }

            return StateType.Success;
        }

        public override void OnExit()
        {
            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            if (pawn.IsDrafted)
            {
                _stateMachine.StateQueue.Enqueue(new PawnJob_Draft(pawn, true));
            }
        }
    }
}