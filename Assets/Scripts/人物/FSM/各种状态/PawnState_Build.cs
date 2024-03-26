
using ChenChen_BuildingSystem;
using UnityEngine;

namespace PawnStates
{
    public class PawnState_Build : StateBase
    {
        private Pawn pawn;
        private GameObject building;
        private Building currentWorkObject;

        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public PawnState_Build(Pawn pawn, GameObject building = null) : base(pawn.StateMachine)
        {
            this.pawn = pawn;
            this.building = building;
        }

        public override void OnEnter()
        {
            if (building != null)
            {
                // 尝试获取蓝图
                currentWorkObject = building.GetComponent<Building>();
                if (currentWorkObject != null)
                {
                    // 尝试取得权限，预定当前工作，标记目标被使用
                    if (!currentWorkObject.GetPrivilege(pawn))
                    {
                        //未取得权限则退出
                        IsError = true;
                        return;
                    }

                    // 设置人物目标点，前往目标，跑过去
                    pawn.MoveControl.GoToHere(building.transform.position);
                    pawn.MoveControl.IsRun = true;

                    // 设置人物无法接取工作
                    pawn.JobToDo(building);
                }
                else
                {
                    Debug.LogWarning("The Building Don't Have Building Component");
                }
            }

        }

        public override StateType OnUpdate()
        {
            if (IsError) return StateType.Failed;

            // 判断是否到达目标点附近
            if (Vector2.Distance(pawn.transform.position, building.transform.position) < pawn.WorkRange)
            {
                pawn.MoveControl.StopMove();

                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                currentWorkObject.Build(pawn.Attribute.A_Construction.Value * Time.deltaTime);

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (currentWorkObject.WorkLoad <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Debug.Log($"已经完成建造" + currentWorkObject.name);
            // 归还目标使用权限
            currentWorkObject.RevokePrivilege(pawn);

            // 设置人物状态
            pawn.JobDone();
            pawn.MoveControl.IsRun = false;

            // 结束动画
            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            currentWorkObject.Interpret();
            OnExit();
        }
    }
}
