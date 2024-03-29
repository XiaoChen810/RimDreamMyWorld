using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Build : JobBase
    {
        private GameObject building;
        private Building currentWorkObject;
        private float _time;

        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public PawnJob_Build(Pawn pawn, GameObject building = null) : base(pawn)
        {
            this.building = building;
        }

        public override bool OnEnter()
        {
            if (building == null) return false;

            // 尝试获取蓝图
            currentWorkObject = building.GetComponent<Building>();
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
            pawn.MoveControl.GoToHere(building.transform.position);
            pawn.MoveControl.IsRun = true;
            // 设置人物无法接取工作
            pawn.JobToDo(building);

            return true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (Vector2.Distance(pawn.transform.position, building.transform.position) < pawn.WorkRange)
            {
                pawn.MoveControl.StopMove();

                // 设置人物正在工作
                pawn.JobDoing();

                // 执行工作
                _time += Time.deltaTime;
                if(_time > 2)
                {
                    currentWorkObject.Build(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                // 播放动画
                pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (currentWorkObject.NeedWorkload <= 0)
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
