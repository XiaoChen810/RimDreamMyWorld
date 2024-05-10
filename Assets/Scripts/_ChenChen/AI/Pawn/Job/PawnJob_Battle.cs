using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Battle : PawnJob
    {
        private readonly static float tick = 500;
        private GameObject target;
        private Pawn targetPawnComponent;

        public PawnJob_Battle(Pawn pawn, GameObject target) : base(pawn, tick)
        {
            this.target = target;
        }

        public override bool OnEnter()
        {
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null) return false;
            return _pawn.TryToEnterBattle(targetPawnComponent);
        }

        public override StateType OnUpdate()
        {
            _pawn.JobDoing();
            
            //返回成功
            //目标被杀死
            if (targetPawnComponent.Info.IsDead)
            {
                return StateType.Success;
            }

            //返回失败
            //超出攻击距离
            if (Vector2.Distance(target.transform.position, _pawn.transform.position) > _pawn.AttackRange)
            {
                return StateType.Failed;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            _pawn.TryToEndBattle();
            _pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }


    }
}
