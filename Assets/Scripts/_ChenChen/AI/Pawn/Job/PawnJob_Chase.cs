using ChenChen_Thing;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private Pawn chaseTarget;

        public PawnJob_Chase(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            chaseTarget = target.TargetA.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            if (chaseTarget == null)
            {
                DebugLogDescription = ("Ŀ����Pawn���������һ����׷����Ŀ��");
                return false;
            }

            if(!pawn.MoveController.GoToHere(target.TargetA, Urgency.Normal, pawn.AttackRange))
            {
                DebugLogDescription = ("�޷�ǰ��Ŀ��");
                return false;
            }

            pawn.JobToDo(target);
            Description = "����׷��" + chaseTarget.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            if (chaseTarget == null)
            {
                return StateType.Interrupt;
            }

            if (chaseTarget.Info.IsDead)
            {
                return StateType.Success;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();

                IsSuccess = true;
                pawn.StateMachine.NextState = new PawnJob_Attack(pawn, target);
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}