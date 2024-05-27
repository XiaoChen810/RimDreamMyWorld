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
                DebugLogDescription = ("Ŀ����Pawn���");
                return false;
            }
            // ���������޷���ȡ����
            pawn.JobToDo(target.GameObject);
            // ��������Ŀ��㣬ǰ��Ŀ�꣬�߹�ȥ
            pawn.MoveController.GoToHere(target.GameObject, Urgency.Normal, pawn.AttackRange);
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
            // �ж�Ŀ���Ƿ񵽴﹥������
            if (pawn.MoveController.ReachDestination)
            {
                // �����������ڹ���
                pawn.JobDoing();

                // ���빥��״̬
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