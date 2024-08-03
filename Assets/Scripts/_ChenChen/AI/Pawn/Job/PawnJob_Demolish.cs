using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolish : PawnJob
    {
        private readonly static float tick = 50;
        private Thing targetComponent;
        private float _timer;
        private float _timeOne;

        public PawnJob_Demolish(Pawn pawn, GameObject building) : base(pawn, tick,new TargetPtr(building))
        {
            float ability = pawn.Attribute.A_Construction.Value;
            _timeOne = Mathf.Lerp(0.1f, 1f, (1 - ability / 20));
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            targetComponent = target.GetComponent<Thing>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("���Ի�ȡ���ʧ��");
                return false;
            }

            bool flag = pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("�޷��ƶ���Ŀ���");
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "ǰ�����" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (targetComponent == null)
            {
                return StateType.Success;
            }

            if (targetComponent.CurDurability <= 0)
            {
                return StateType.Success;
            }

            if (targetComponent.LifeState != BuildingLifeStateType.MarkDemolished)
            {
                return StateType.Interrupt;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "���ڲ��" + target.GameObject.name;

                _timer += Time.deltaTime;
                if (_timer > _timeOne)
                {
                    targetComponent.OnDemolish(1);
                    _timer = 0;
                }

                pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

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