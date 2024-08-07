using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Socialize : JobGiver
    {
        private static readonly float interval_time = 10;

        public JobGiver_Socialize(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, null, interval_time)
        {
        }

        private float mood_threshold = 0.4f;    // 不开心的阈值 0 ~ 1。
        private bool isAlone = false;

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            // 如果心情不好
            if (pawn.Info.Happiness.CurValue > mood_threshold * pawn.Info.Happiness.MaxValue) return null;

            return FindFriend(pawn);
        }

        private TargetPtr FindFriend(Pawn pawn)
        {
            List<GameObject> otherPawns = new();
            IReadOnlyList<Pawn> pawnsList = GameManager.Instance.PawnGeneratorTool.PawnList_Colony;

            foreach (var otherPawn in pawnsList)
            {
                if (otherPawn != pawn) otherPawns.Add(otherPawn.gameObject);
            }

            if (otherPawns.Count <= 1)
            {
                if (!isAlone)
                {
                    isAlone = true;
                    pawn.EmotionController.AddEmotion(EmotionType.distressed);
                }
                return null;
            }
            else
            {
                isAlone = false;
                GameObject obj = otherPawns[UnityEngine.Random.Range(0, otherPawns.Count)];
                return new TargetPtr(obj);
            }
        }
    }
}
