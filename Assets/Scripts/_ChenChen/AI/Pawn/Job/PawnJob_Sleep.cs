using ChenChen_BuildingSystem;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Sleep : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;
        private Thing_Bed bed;
        public PawnJob_Sleep(Pawn pawn, GameObject bed) : base(pawn, tick, new TargetPtr(bed))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.GameObject.TryGetComponent<Thing_Bed>(out bed)) 
            {
                DebugLogDescription = ("没有Thing_Bed组件");
                return false;
            }

            // 设置人物目标点，前往目标，走过去
            if (!pawn.MoveController.GoToHere(bed.transform.position, Urgency.Normal, 0.01f))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            // 设置人物接取工作
            pawn.JobToDo(bed.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在睡觉
                pawn.JobDoing();

                // 播放动画
                pawn.Animator.SetBool("IsDie", true);

                // 睡觉
                pawn.Info.Sleepiness.CurValue += Time.deltaTime;
            }

            // 睡饱了
            if(pawn.Info.Sleepiness.IsMax)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // 结束动画
            pawn.Animator.SetBool("IsDie", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}