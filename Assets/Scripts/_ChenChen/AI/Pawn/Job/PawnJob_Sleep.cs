using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Sleep : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;
        private GameObject target;
        private Thing_Bed bed;
        public PawnJob_Sleep(Pawn pawn, GameObject bed) : base(pawn, tick, null)
        {
            target = bed;
        }

        public override bool OnEnter()
        {
            if(!target.TryGetComponent<Thing_Bed>(out bed)) 
            {
                DebugLogDescription = ("没有Thing_Bed组件");
                return false;
            }

            // 尝试取得权限，预定当前工作，标记目标被使用
            if (!bed.GetPermission(_pawn))
            {
                DebugLogDescription = ("目标已经有其他人被使用");
                return false;
            }

            // 设置人物目标点，前往目标，走过去
            if (!_pawn.MoveController.GoToHere(bed.transform.position, Urgency.Normal, 0.01f))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物接取工作
            _pawn.JobToDo(bed.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            if (target == null) return StateType.Failed;
            // 判断是否到达目标点附近
            if (_pawn.MoveController.ReachDestination)
            {
                // 设置人物正在睡觉
                _pawn.JobDoing();

                // 播放动画
                _pawn.Animator.SetBool("IsDie", true);

                // 睡觉
                _pawn.Info.Sleepiness.CurValue += Time.deltaTime;
            }

            if(_pawn.Info.Sleepiness.IsMax)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            // 归还目标使用权限
            bed.RevokePermission(_pawn);

            // 设置人物状态
            _pawn.JobDone();

            // 结束动画
            _pawn.Animator.SetBool("IsDie", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}