using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Socialize : JobGiver
    {
        private static readonly float interval_time = 20;
        private static readonly float probability_threshold = 0.5f;

        public JobGiver_Socialize(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, interval_time, probability_threshold)
        {
        }

        private float mood_threshold = 0.4f;    // 不开心的阈值 0 ~ 1。
        private bool isAlone = false;

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            // 如果心情不好
            if (pawn.Info.Happiness.CurValue > mood_threshold * pawn.Info.Happiness.MaxValue) return null;

            return FindFriend(pawn);
        }

        private GameObject FindFriend(Pawn pawn)
        {
            List<GameObject> otherPawns = new();
            //找一个除了自己以外的Pawn
            foreach (var otherPawn in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                if (otherPawn != pawn) otherPawns.Add(otherPawn);
            }

            //有无其他Pawn
            if (otherPawns.Count <= 1)
            {
                //输出需求，一次
                if (!isAlone)
                {
                    isAlone = true;
                    string narrative = $"{pawn.Def.PawnName} 想要交朋友，但只有它一个人...";
                    ScenarioManager.Instance.Narrative(narrative, pawn.gameObject);

                    //添加焦虑
                    pawn.EmotionController.AddEmotion(EmotionType.distressed);
                }
                return null;
            }
            else
            {
                isAlone = false;
                return otherPawns[UnityEngine.Random.Range(0, otherPawns.Count)];
            }
        }
    }
}
