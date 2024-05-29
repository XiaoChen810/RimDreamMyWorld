using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Attack : PawnJob
    {
        private readonly static float tick = 50;

        public PawnJob_Attack(Pawn pawn, GameObject target) : base(pawn, tick,new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            pawn.JobDoing();

            //返回成功
            if (pawn.MoveController.ReachDestination)
            {
                pawn.Animator.SetTrigger("IsAttack");
                return StateType.Success;
            }
            //返回失败

            return StateType.Doing;
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }


    }
}
