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

            //逻辑
            if (!target.GameObject.TryGetComponent<Pawn>(out Pawn targetPawn))
            {
                DebugLogDescription = ("对象不是Pawn");
                return false;
            }

            // 设置人物目标点，前往目标，走过去
            if (!pawn.MoveController.GoToHere(targetPawn.gameObject, Urgency.Normal, 2))
            {
                DebugLogDescription = ("无法前往目标位置");
                return false;
            }

            // 设置人物接取工作
            pawn.JobToDo(targetPawn.gameObject);

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            // 判断是否到达目标点附近
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在社交
                pawn.JobDoing();

                string me = pawn.Def.PawnName;
                string him = target.GetComponent<Pawn>().Def.PawnName;
                string narrative = $"{me} 正在和 {him} 畅所欲言";
                ScenarioManager.Instance.Narrative(narrative, pawn.gameObject);

                // 移除焦虑
                pawn.EmotionController.RemoveEmotion(EmotionType.distressed);

                return StateType.Success;
            }

            //逻辑
            return StateType.Doing;
        }
    }
}