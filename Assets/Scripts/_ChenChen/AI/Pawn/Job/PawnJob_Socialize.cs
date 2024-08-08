using ChenChen_UI;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Socialize : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;
        private readonly Pawn targetPawn = null;

        public PawnJob_Socialize(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            targetPawn = target.TargetA.GetComponent<Pawn>();
        }

        public override bool OnEnter()
        {
            if(targetPawn == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("无法前往目标位置");
                return false;
            }

            pawn.JobToDo(target);
            Description = $"准备和{targetPawn.name}社交";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();

                Description = $"正在和{targetPawn.Def.PawnName}社交";

                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //逻辑
            return StateType.Doing;
        }
    }
}