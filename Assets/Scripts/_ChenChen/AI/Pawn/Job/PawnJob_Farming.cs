using ChenChen_BuildingSystem;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ChenChen_AI
{
    public class PawnJob_Farming : PawnJob
    {
        private readonly static float tick = 50;

        private WorkSpace_Farm _workSpace;
        private Vector2 _farmingPosition;
        private float _farmingTime;

        public PawnJob_Farming(Pawn pawn, GameObject workSpace = null) : base(pawn, tick)
        {
            _workSpace = workSpace.GetComponent<WorkSpace_Farm>();
            _farmingTime = 20 - _pawn.Attribute.A_Survival.Value;
            _farmingTime = _farmingTime > 1 ? _farmingTime : 1;
        }

        public override bool OnEnter()
        {
            if (_workSpace == null)
            {
                Debug.LogWarning($"{_pawn.name} No WorkSpace_Farm");
                return false;
            }

            // ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
            if (!_workSpace.GetPrivilege(_pawn))
            {
                Debug.LogWarning($"{_pawn.name} No privilege");
                return false;
            }

            // ���Ի�ȡ����λ��            
            if (!_workSpace.TryGetAFarmingPosition(out _farmingPosition))
            {
                Debug.LogWarning($"{_pawn.name} No position");
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�ܹ�ȥ
            if (!_pawn.MoveControl.GoToHere(_farmingPosition, Urgency.Urge))
            {
                Debug.LogWarning($"{_pawn.name} The position can't arrive");
                return false;
            }

            // ���������޷���ȡ����
            _pawn.JobToDo(_workSpace.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (_pawn.MoveControl.ReachDestination)
            {
                // �����������ڹ���
                _pawn.JobDoing();

                // ִ�й���
                _farmingTime -= Time.deltaTime;
                // ����������ֲ��״̬��������ͣ�����Խ�����һ��״̬
                if (_farmingTime <= 0)
                {
                    return StateType.Success;
                }

                // ���Ŷ���
                _pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            _workSpace.TrySetAPositionHadFarmed(_farmingPosition);
            Debug.Log($"_workSpace.TrySetAPositionHadFarmed({_farmingPosition})");

            // ��������״̬
            _pawn.JobDone();

            // ��������
            _pawn.Animator.SetBool("IsDoing", false);

            // �黹Ŀ��ʹ��Ȩ��
            _workSpace.RevokePrivilege(_pawn);
        }

        public override void OnInterrupt()
        {
            // ��������״̬
            _pawn.JobDone();

            // ��������
            _pawn.Animator.SetBool("IsDoing", false);

            // �黹Ŀ��ʹ��Ȩ��
            _workSpace.RevokePrivilege(_pawn);
        }
    }
}