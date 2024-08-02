using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Draft : PawnJob
    {
        private readonly static float tick = 50;

        public PawnJob_Draft(Pawn pawn, bool openOrclose) : base(pawn, tick, null)
        {
            pawn.Info.IsDrafted = openOrclose;
            this.Description = "Õ÷Õ×ÖÐ";
        }

        public override bool OnEnter()
        {
            pawn.JobDoing();
            pawn.JobCanGet();
            return true;
        }

        public override StateType OnUpdate()
        {
            if (pawn.Info.IsDrafted)
            {
                return StateType.Doing;
            }
            return StateType.Success;
        }

        public override void OnInterrupt()
        {
            if (pawn.Info.IsDrafted)
            {
                _stateMachine.StateQueue.Enqueue(new PawnJob_Draft(pawn, true));
            }
        }
    }
}