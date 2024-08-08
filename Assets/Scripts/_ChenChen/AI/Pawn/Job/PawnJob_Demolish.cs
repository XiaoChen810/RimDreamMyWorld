using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolish : PawnJob
    {
        private readonly static float tick = 50;
        private Building building;
        private float _timer;
        private float _timeOne;

        public PawnJob_Demolish(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            building = target.TargetA.GetComponent<Building>();
            _timeOne = Mathf.Lerp(0.1f, 1f, (1 - pawn.Attribute.A_Construction.Value / 20));
        }

        public override bool OnEnter()
        {
            if (building == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(target.PositonA, Urgency.Urge, pawn.WorkRange))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(target);
            Description = "前往拆除" + building.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (building == null)
            {
                return StateType.Success;
            }

            if (building.Durability <= 0)
            {
                return StateType.Success;
            }

            if (building.LifeState != BuildingLifeStateType.MarkDemolished)
            {
                return StateType.Interrupt;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                Description = "正在拆除" + building.name;

                _timer += Time.deltaTime;
                if (_timer > _timeOne)
                {
                    building.OnDemolish(1);
                    _timer = 0;
                }
            }

            return StateType.Doing;
        }
    }
}