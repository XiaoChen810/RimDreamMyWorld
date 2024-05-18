using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Building : PawnJob
    {
        private readonly static float tick = 50;
        private GameObject target;
        private Thing_Building curTargetComponent;
        private float _time;
        private float _timeOne;

        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public PawnJob_Building(Pawn pawn, GameObject target = null) : base(pawn, tick)
        {
            this.target = target;
            float ability = pawn.Attribute.A_Construction.Value;
            if (ability == 0) _timeOne = 10;
            _timeOne = 1f / ability;
        }

        public override bool OnEnter()
        {
            if (target == null) return false;

            // 尝试获取组件
            curTargetComponent = target.GetComponent<Thing_Building>();
            if (curTargetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!curTargetComponent.GetPermission(_pawn))
            {
                DebugLogDescription = ("目标已经其他人被使用");
                return false;
            }

            // 设置人物目标点，前往目标，跑过去
            if (!_pawn.MoveController.GoToHere(target.transform.position, Urgency.Urge, _pawn.WorkRange))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物接取工作
            _pawn.JobToDo(target);

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
                _time += Time.deltaTime;
                if(_time > _timeOne)
                {
                    curTargetComponent.OnBuild(1);
                    _time = 0;
                }

                // 播放动画
                _pawn.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (curTargetComponent.Workload <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 归还目标使用权限
            curTargetComponent.RevokePermission(_pawn);

            // 设置人物状态
            _pawn.JobDone();

            // 结束动画
            _pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            curTargetComponent.OnInterpret();
            OnExit();
        }
    }
}
