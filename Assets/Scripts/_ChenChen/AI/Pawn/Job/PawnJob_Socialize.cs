using ChenChen_UI;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Socialize : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;

        public PawnJob_Socialize(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!target.GameObject.TryGetComponent<Pawn>(out Pawn targetPawn))
            {
                DebugLogDescription = ("对象不是Pawn");
                return false;
            }

            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("无法前往目标位置");
                return false;
            }

            pawn.JobToDo(targetPawn.gameObject);
            this.Description = $"准备和{targetPawn.name}一起玩";

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();

                string him = target.GetComponent<Pawn>().Def.PawnName;
                this.Description = $"正在和{him}一起玩";

                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //逻辑
            return StateType.Doing;
        }
    }
}