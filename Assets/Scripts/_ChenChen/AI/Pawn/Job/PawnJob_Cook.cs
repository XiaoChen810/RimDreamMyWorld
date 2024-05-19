using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cook : PawnJob
    {
        //�������ʱ��
        private readonly static float tick = 500;

        private Thing_CookingStation _cookingStation;
        protected PawnJob_Cook(Pawn pawn,GameObject cookingStation) : base(pawn, tick, new TargetPtr(cookingStation))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.TryGetComponent<Thing_CookingStation>(out _cookingStation))
            {
                DebugLogDescription = "��ȡ���ʧ��";
                return false;
            }

            if(!pawn.MoveController.GoToHere(target.Positon,Urgency.Urge,pawn.WorkRange))
            {
                DebugLogDescription = "�޷��ƶ���Ŀ��λ�ã�" + target.Positon;
                return false;
            }

            pawn.JobToDo(target.GameObject);
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

                // ���Ŷ���
                pawn.Animator.SetBool("IsDoing", true);
            }

            // �������˽��죬״̬��������ͣ�����Խ�����һ��״̬
            if (_cookingStation.IsSuccess)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}