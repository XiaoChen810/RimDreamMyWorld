using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Attack : PawnJob
    {
        private readonly static float tick = 30;

        private float _lastAttackTime;
        private Pawn _targetPawn;

        public PawnJob_Attack(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
            _targetPawn = target.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.TryGetComponent<Pawn>(out _targetPawn))
            {
                DebugLogDescription = "攻击目标不允许";
                return false;
            }

            if (Vector2.Distance(target.Positon, pawn.transform.position) > pawn.AttackRange)
            {
                DebugLogDescription = "攻击距离不够";
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "攻击" + target.GameObject.name;
            return true;
        }

        public override StateType OnUpdate()
        {
            if (target.GameObject == null)
            {
                return StateType.Success;
            }

            if (_targetPawn.Info.IsDead)
            {
                return StateType.Success;
            }

            if (Vector2.Distance(target.Positon, pawn.transform.position) > pawn.AttackRange)
            {
                return StateType.Failed;
            }

            pawn.JobDoing();
            
            if(Time.time > _lastAttackTime + pawn.AttackSpeedWait)
            {
                _lastAttackTime = Time.time;
                if(target.Positon.x < pawn.transform.position.x)
                {
                    pawn.MoveController.FilpLeft();
                }
                else
                {
                    pawn.MoveController.FilpRight();
                }
                pawn.Animator.SetTrigger("Attack");
                _targetPawn.GetDamage(pawn.gameObject, pawn.AttackDamage);
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
