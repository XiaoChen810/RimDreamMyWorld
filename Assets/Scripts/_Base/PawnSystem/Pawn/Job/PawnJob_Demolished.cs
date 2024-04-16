using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private GameObject building;
        private Thing_Building currentWorkObject;
        private float _time;

        public PawnJob_Demolished(Pawn pawn, GameObject building = null) : base(pawn)
        {
            this.building = building;
        }

        public override bool OnEnter()
        {
            if (building == null) return false;

            // 尝试获取蓝图
            currentWorkObject = building.GetComponent<Thing_Building>();
            if (currentWorkObject == null)
            {
                Debug.LogWarning("The Building Don't Have Building Component");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!currentWorkObject.GetPrivilege(pawn))
            {
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            pawn.MoveControl.GoToHere(building.transform.position, Urgency.Urge, pawn.WorkRange);
            // 设置人物无法接取工作
            pawn.JobToDo(building);

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
            if (pawn.MoveControl.reachedDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                _time += Time.deltaTime;
                if (_time > 2)
                {
                    currentWorkObject.Demolish(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 设置人物状态
            pawn.JobDone();

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            // 归还目标使用权限
            currentWorkObject.RevokePrivilege(pawn);

            OnExit();
        }
    }
}