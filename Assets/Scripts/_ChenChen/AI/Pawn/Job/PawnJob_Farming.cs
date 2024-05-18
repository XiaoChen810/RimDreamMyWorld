using ChenChen_BuildingSystem;
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

        public PawnJob_Farming(Pawn pawn, GameObject workSpace = null) : base(pawn, tick)
        {
            _workSpace_Farm = workSpace.GetComponent<WorkSpace_Farm>();
            _farmingTime = 15 - _pawn.Attribute.A_Survival.Value;
            _farmingTime = _farmingTime > 1 ? _farmingTime : 1;
        }

        public override bool OnEnter()
        {
            if (_workSpace_Farm == null)
            {
                DebugLogDescription = ($"{_pawn.name} No WorkSpace_Farm");
                return false;
            }
            // ���Ի�ȡ����λ��            
            if (!_workSpace_Farm.TryGetAFarmingPosition(out _farmingPosition))
            {
                DebugLogDescription = ($"{_pawn.name} �޷��� {_workSpace_Farm.name} �л�ȡ����λ��");
                return false;
            }
            if (!_pawn.MoveController.GoToHere(_farmingPosition, Urgency.Urge))
            {
                DebugLogDescription = ($"{_pawn.name} The position can't arrive");
                return false;
            }
            // ���������޷���ȡ����
            _pawn.JobToDo(_workSpace_Farm.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (_pawn.MoveController.ReachDestination)
            {
                // �����������ڹ���
                _pawn.JobDoing();

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
                _pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            // ��������״̬
            _pawn.JobDone();

            // ��������
            _pawn.Animator.SetBool("IsDoing", false);

        }

        public override void OnInterrupt()
        {
            _workSpace_Farm.ReturnAPosition(_farmingPosition);
            OnExit();
        }
    }
}