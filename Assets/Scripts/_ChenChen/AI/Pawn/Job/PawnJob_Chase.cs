using ChenChen_Thing;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private Pawn targetPawnComponent;

        public PawnJob_Chase(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {

        }

        public override bool OnEnter()
        {
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null)
            {
                DebugLogDescription = ("目标无Pawn组件，不是一个可追击的目标");
                return false;
            }

            if(!pawn.MoveController.GoToHere(target.GameObject, Urgency.Normal, pawn.AttackRange))
            {
                DebugLogDescription = ("无法前往目标");
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "正在追击" + target.GameObject.name;

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
                pawn.StateMachine.NextState = new PawnJob_Attack(pawn, target.GameObject);
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