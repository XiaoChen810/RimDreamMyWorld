using ChenChen_Thing;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Sleep : PawnJob
    {
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

            if (!pawn.MoveController.GoToHere(bed.transform.position, Urgency.Normal, 0.01f))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(bed.gameObject);
            this.Description = "回床上睡觉";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在睡觉";
                pawn.Animator.SetBool("IsDie", true);
                pawn.Info.Sleepiness.CurValue += Time.deltaTime;
                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);
            }

            if(pawn.Info.Sleepiness.IsMax)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.Animator.SetBool("IsDie", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}