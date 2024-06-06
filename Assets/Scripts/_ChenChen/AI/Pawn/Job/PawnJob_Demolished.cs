using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private readonly static float tick = 50;
        private ThingBase targetComponent;
        private float _time;
        private float _timeOne;

        public PawnJob_Demolished(Pawn pawn, GameObject building) : base(pawn, tick,new TargetPtr(building))
        {
            float ability = pawn.Attribute.A_Construction.Value;
            float a = 1 - (ability / 20);
            _timeOne = Mathf.Lerp(1f, 10, a) / 2;
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            // ���Ի�ȡ��ͼ
            targetComponent = target.GetComponent<ThingBase>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("���Ի�ȡ���ʧ��");
                return false;
            }

            // ��������Ŀ��㣬ǰ��Ŀ�꣬�ܹ�ȥ
            bool flag = pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("�޷��ƶ���Ŀ���");
                return false;
            }

            // ���������޷���ȡ����
            pawn.JobToDo(target.GameObject);
            this.Description = "ǰ�����" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // Ŀ��Ϊ�գ�����˲��
            if (targetComponent == null)
            {
                return StateType.Success;
            }
            // Ŀ���;ö�Ϊ0������˲��
            if (targetComponent.CurDurability <= 0)
            {
                return StateType.Success;
            }
            // Ŀ������ǲ��״̬���ж�
            if (targetComponent.LifeState != BuildingLifeStateType.MarkDemolished)
            {
                return StateType.Interrupt;
            }

            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (pawn.MoveController.ReachDestination)
            {
                // �����������ڹ���
                pawn.JobDoing();
                this.Description = "���ڲ��" + target.GameObject.name;

                // ִ�й���
                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    targetComponent.OnDemolish(pawn.Attribute.A_Construction.Value);
                    _time = 0;
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
            // �黹Ŀ��ʹ��Ȩ��
            targetComponent.RevokePermission(pawn);

            OnExit();
        }
    }
}