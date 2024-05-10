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

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!_workSpace.GetPrivilege(_pawn))
            {
                Debug.LogWarning($"{_pawn.name} No privilege");
                return false;
            }

            // 尝试获取工作位置            
            if (!_workSpace.TryGetAFarmingPosition(out _farmingPosition))
            {
                Debug.LogWarning($"{_pawn.name} No position");
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            if (!_pawn.MoveControl.GoToHere(_farmingPosition, Urgency.Urge))
            {
                Debug.LogWarning($"{_pawn.name} The position can't arrive");
                return false;
            }

            // 设置人物无法接取工作
            _pawn.JobToDo(_workSpace.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (_pawn.MoveControl.ReachDestination)
            {
                // 设置人物正在工作
                _pawn.JobDoing();

                // 执行工作
                _farmingTime -= Time.deltaTime;
                // 如果完成了种植，状态机结束暂停，可以进入下一个状态
                if (_farmingTime <= 0)
                {
                    return StateType.Success;
                }

                // 播放动画
                _pawn.Animator.SetBool("IsDoing", true);
            }
            return StateType.Doing;
        }

        public override void OnExit()
        {
            _workSpace.TrySetAPositionHadFarmed(_farmingPosition);
            Debug.Log($"_workSpace.TrySetAPositionHadFarmed({_farmingPosition})");

            // 设置人物状态
            _pawn.JobDone();

            // 结束动画
            _pawn.Animator.SetBool("IsDoing", false);

            // 归还目标使用权限
            _workSpace.RevokePrivilege(_pawn);
        }

        public override void OnInterrupt()
        {
            // 设置人物状态
            _pawn.JobDone();

            // 结束动画
            _pawn.Animator.SetBool("IsDoing", false);

            // 归还目标使用权限
            _workSpace.RevokePrivilege(_pawn);
        }
    }
}