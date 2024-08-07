using ChenChen_Thing;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private Pawn targetPawnComponent;

        public PawnJob_Chase(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {

        }

        public override bool OnEnter()
        {
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null)
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
            this.Description = "����׷��" + target.TargetA.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (targetPawnComponent.Info.IsDead)
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

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}