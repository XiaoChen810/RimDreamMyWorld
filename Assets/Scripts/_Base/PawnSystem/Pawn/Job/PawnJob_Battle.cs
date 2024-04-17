using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Battle : PawnJob
    {
        private GameObject target;
        private Pawn targetPawnComponent;

        public PawnJob_Battle(Pawn pawn, GameObject target) : base(pawn)
        {
            this.target = target;
        }

        public override bool OnEnter()
        {
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null) return false;
            return pawn.TryToEnterBattle(targetPawnComponent);
        }

        public override StateType OnUpdate()
        {
            pawn.JobDoing();
            
            //返回成功
            //目标被杀死
            if (targetPawnComponent.IsDead)
            {
                return StateType.Success;
            }

            //返回失败
            //超出攻击距离
            if (Vector2.Distance(target.transform.position, pawn.transform.position) > pawn.AttackRange)
            {
                return StateType.Failed;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            pawn.TryToEndBattle();
            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }


    }
}
