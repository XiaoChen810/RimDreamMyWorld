using ChenChen_Thing;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ChenChen_AI
{
    public class PawnJob_Farming : PawnJob
    {
        private readonly static float tick = 50;

        private WorkSpace_Farm _workSpace_Farm;
        private Vector2 _farmingPosition;
        private float _farmingTime;

        public PawnJob_Farming(Pawn pawn, GameObject workSpace) : base(pawn, tick, new TargetPtr(workSpace))
        {
            _farmingTime = 15 - base.pawn.Attribute.A_Survival.Value;
            _farmingTime = _farmingTime > 1 ? _farmingTime : 1;
        }

        public override bool OnEnter()
        {
            if (!target.GameObject.TryGetComponent<WorkSpace_Farm>(out _workSpace_Farm))
            {
                DebugLogDescription = ($"{pawn.name} No WorkSpace_Farm");
                return false;
            }
            // ���Ի�ȡ����λ��            
            if (!_workSpace_Farm.TryGetAFarmingPosition(out _farmingPosition))
            {
                DebugLogDescription = ($"{pawn.name} �޷��� {_workSpace_Farm.name} �л�ȡ����λ��");
                return false;
            }
            // ����ǰ������λ��
            if (!pawn.MoveController.GoToHere(_farmingPosition, Urgency.Urge))
            {
                DebugLogDescription = ($"{pawn.name} The position can't arrive");
                return false;
            }
            // ���������޷���ȡ����
            pawn.JobToDo(_workSpace_Farm.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (pawn.MoveController.ReachDestination)
            {
                // �����������ڹ���
                pawn.JobDoing();

                // ִ�й���
                _farmingTime -= Time.deltaTime;
                // ����������ֲ��״̬��������ͣ�����Խ�����һ��״̬
                if (_farmingTime <= 0)
                {
                    // �����λ����һ��ֲ��
                    if (_workSpace_Farm.TrySetAPositionHadFarmed(_farmingPosition))
                    {
                        return StateType.Success;
                    }
                    return StateType.Failed;
                }

                // ���Ŷ���
                pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // ��������
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            _workSpace_Farm.ReturnAPosition(_farmingPosition);
            OnExit();
        }
    }
}