using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Attack : PawnJob
    {
        private readonly static float tick = 30;

        private float _lastAttackTime;
        private Pawn _targetPawn;

        public PawnJob_Attack(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            _targetPawn = target.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            if (_targetPawn == null)
            {
                DebugLogDescription = "攻击目标消失";
                return false;
            }

            if (Vector2.Distance(target.PositonA, pawn.transform.position) > pawn.AttackRange)
            {
                DebugLogDescription = "攻击距离不够";
                return false;
            }

            pawn.JobToDo(target);
            Description = "攻击" + target.TargetA.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            if (target.TargetA == null)
            {
                return StateType.Success;
            }

            if (_targetPawn.Info.IsDead)
            {
                return StateType.Success;
            }

            if (Vector2.Distance(target.PositonA, pawn.transform.position) > pawn.AttackRange)
            {
                return StateType.Failed;
            }

            pawn.JobDoing();
            
            if(Time.time > _lastAttackTime + pawn.AttackSpeedWait)
            {
                _lastAttackTime = Time.time;
                if(target.PositonA.x < pawn.transform.position.x)
                {
                    pawn.MoveController.FilpLeft();
                }
                else
                {
                    pawn.MoveController.FilpRight();
                }
                _targetPawn.GetDamage(pawn.gameObject, pawn.AttackDamage);
            }


            return StateType.Doing;
        }
    }
}
