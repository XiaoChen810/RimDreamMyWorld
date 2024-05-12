using ChenChen_BuildingSystem;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private readonly static float tick = 50;
        private GameObject building;
        private Thing_Building currentWorkObject;
        private float _time;
        private float _timeOne;

        public PawnJob_Demolished(Pawn pawn, GameObject building = null) : base(pawn, tick)
        {
            this.building = building;
            float ability = pawn.Attribute.A_Construction.Value;
            if (ability == 0) _timeOne = 10;
            _timeOne = 1f / ability;
        }

        public override bool OnEnter()
        {
            if (building == null) return false;

            // 尝试获取蓝图
            currentWorkObject = building.GetComponent<Thing_Building>();
            if (currentWorkObject == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!currentWorkObject.GetPermission(_pawn))
            {
                DebugLogDescription = ("目标已经其他人被使用");
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            bool flag = _pawn.MoveControl.GoToHere(building.transform.position, Urgency.Urge, _pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物无法接取工作
            _pawn.JobToDo(building);

            return true;
        }

        public override StateType OnUpdate()
        {
            // 如果完成了拆除，状态机结束暂停，可以进入下一个状态
            if (currentWorkObject == null)
            {
                return StateType.Success;
            }

            if (currentWorkObject.MaxDurability <= 0)
            {
                return StateType.Success;
            }

            // 判断是否到达目标点附近
            if (_pawn.MoveControl.ReachDestination)
            {
                // 设置人物正在工作
                _pawn.JobDoing();

                // 执行工作
                // 执行工作
                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    currentWorkObject.OnDemolish(_pawn.Attribute.A_Construction.Value);
                    _time = 0;
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
            // 归还目标使用权限
            currentWorkObject.RevokePermission(_pawn);

            OnExit();
        }
    }
}