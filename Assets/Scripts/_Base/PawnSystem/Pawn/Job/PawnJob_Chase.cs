using ChenChen_BuildingSystem;
using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Chase : PawnJob
    {
        private readonly static float tick = 500;
        private GameObject target;
        private Pawn targetPawnComponent;
        public PawnJob_Chase(Pawn pawn, GameObject target) : base(pawn, tick)
        {
            this.target = target;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;
            targetPawnComponent = target.GetComponent<Pawn>();
            if (targetPawnComponent == null) return false;

            // ���������޷���ȡ����
            pawn.JobToDo(target);
            // ��������Ŀ��㣬ǰ��Ŀ�꣬�߹�ȥ
            pawn.MoveControl.GoToHere(target, Urgency.Normal, pawn.AttackRange);
            return true;
        }

        public override StateType OnUpdate()
        {
            // �ж�Ŀ���Ƿ񵽴﹥������
            if (pawn.MoveControl.ReachDestination)
            {
                // �����������ڹ���
                pawn.JobDoing();               

                // ������һ��״̬
                if (!targetPawnComponent.IsDead)
                {
                    IsSuccess = true;
                    pawn.StateMachine.NextState = new PawnJob_Battle(pawn, target);
                    return StateType.Success;
                }
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // ��������״̬
            pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}