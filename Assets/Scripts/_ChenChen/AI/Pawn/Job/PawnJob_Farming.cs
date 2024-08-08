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

        public PawnJob_Farming(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            _farmingTime = 15 - base.pawn.Attribute.A_Survival.Value;
            _farmingTime = _farmingTime > 1 ? _farmingTime : 1;
        }

        public override bool OnEnter()
        {
            if (!target.TargetA.TryGetComponent<WorkSpace_Farm>(out _workSpace_Farm))
            {
                DebugLogDescription = ($"{pawn.name} No WorkSpace_Farm");
                return false;
            }
        
            if (!_workSpace_Farm.TryGetAFarmingPosition(out _farmingPosition))
            {
                DebugLogDescription = ($"{pawn.name} 无法从 {_workSpace_Farm.name} 中获取工作位置");
                return false;
            }

            if (!pawn.MoveController.GoToHere(_farmingPosition, Urgency.Urge))
            {
                DebugLogDescription = ($"{pawn.name} The position can't arrive");
                return false;
            }

            pawn.JobToDo(target);
            this.Description = "正在前往" + target.TargetA.name + "种植";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在" + target.TargetA.name + "种植";

                _farmingTime -= Time.deltaTime;
                if (_farmingTime <= 0)
                {
                    if (_workSpace_Farm.TrySetAPositionHadFarmed(_farmingPosition))
                    {
                        return StateType.Success;
                    }
                    return StateType.Failed;
                }
            }
            return StateType.Doing;
        }

        public override void OnInterrupt()
        {
            _workSpace_Farm.ReturnAPosition(_farmingPosition);
            OnExit();
        }
    }
}