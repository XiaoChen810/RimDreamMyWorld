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
            // 尝试获取工作位置            
            if (!_workSpace_Farm.TryGetAFarmingPosition(out _farmingPosition))
            {
                DebugLogDescription = ($"{pawn.name} 无法从 {_workSpace_Farm.name} 中获取工作位置");
                return false;
            }
            // 尝试前往工作位置
            if (!pawn.MoveController.GoToHere(_farmingPosition, Urgency.Urge))
            {
                DebugLogDescription = ($"{pawn.name} The position can't arrive");
                return false;
            }
            // 设置人物无法接取工作
            pawn.JobToDo(_workSpace_Farm.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                _farmingTime -= Time.deltaTime;
                // 如果完成了种植，状态机结束暂停，可以进入下一个状态
                if (_farmingTime <= 0)
                {
                    // 在这个位置种一颗植物
                    if (_workSpace_Farm.TrySetAPositionHadFarmed(_farmingPosition))
                    {
                        return StateType.Success;
                    }
                    return StateType.Failed;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            _workSpace_Farm.ReturnAPosition(_farmingPosition);
            OnExit();
        }
    }
}