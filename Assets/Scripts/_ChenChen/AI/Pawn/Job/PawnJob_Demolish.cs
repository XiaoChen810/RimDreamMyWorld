using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolish : PawnJob
    {
        private readonly static float tick = 50;
        private Building targetComponent;
        private float _timer;
        private float _timeOne;

        public PawnJob_Demolish(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            float ability = pawn.Attribute.A_Construction.Value;
            _timeOne = Mathf.Lerp(0.1f, 1f, (1 - ability / 20));
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            targetComponent = target.GetComponent<Building>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("���Ի�ȡ���ʧ��");
                return false;
            }

            bool flag = pawn.MoveController.GoToHere(target.PositonA, Urgency.Urge, pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("�޷��ƶ���Ŀ���");
                return false;
            }

            pawn.JobToDo(target);
            this.Description = "ǰ�����" + target.TargetA.name;

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

            if (targetComponent.Durability <= 0)
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
                this.Description = "���ڲ��" + target.TargetA.name;

                _timer += Time.deltaTime;
                if (_timer > _timeOne)
                {
                    targetComponent.OnDemolish(1);
                    _timer = 0;
                }
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}