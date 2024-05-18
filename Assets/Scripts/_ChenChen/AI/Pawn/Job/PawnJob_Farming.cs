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
            // 尝试获取工作位置            
            if (!_workSpace_Farm.TryGetAFarmingPosition(out _farmingPosition))
            {
                DebugLogDescription = ($"{_pawn.name} 无法从 {_workSpace_Farm.name} 中获取工作位置");
                return false;
            }
            if (!_pawn.MoveController.GoToHere(_farmingPosition, Urgency.Urge))
            {
                DebugLogDescription = ($"{_pawn.name} The position can't arrive");
                return false;
            }
            // 设置人物无法接取工作
            _pawn.JobToDo(_workSpace_Farm.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (_pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                _pawn.JobDoing();

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
                _pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 设置人物状态
            _pawn.JobDone();

            // 结束动画
            _pawn.Animator.SetBool("IsDoing", false);

        }

        public override void OnInterrupt()
        {
            _workSpace_Farm.ReturnAPosition(_farmingPosition);
            OnExit();
        }
    }
}