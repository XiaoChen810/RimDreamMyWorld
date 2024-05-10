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
            _pawn.JobToDo(target);
            // ��������Ŀ��㣬ǰ��Ŀ�꣬�߹�ȥ
            _pawn.MoveControl.GoToHere(target, Urgency.Normal, _pawn.AttackRange);
            return true;
        }

        public override StateType OnUpdate()
        {
            // �ж�Ŀ���Ƿ񵽴﹥������
            if (_pawn.MoveControl.ReachDestination)
            {
                // �����������ڹ���
                _pawn.JobDoing();               

                // ������һ��״̬
                if (!targetPawnComponent.Info.IsDead)
                {
                    IsSuccess = true;
                    _pawn.StateMachine.NextState = new PawnJob_Battle(_pawn, target);
                    return StateType.Success;
                }
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // ��������״̬
            _pawn.JobDone();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}