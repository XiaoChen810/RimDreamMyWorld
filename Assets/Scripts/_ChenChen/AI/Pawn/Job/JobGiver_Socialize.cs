using ChenChen_UISystem;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Socialize : JobGiver
    {
        private static readonly float interval_time = 20;
        public JobGiver_Socialize(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, interval_time)
        {
        }

        private bool isAlone = false;

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            List<GameObject> otherPawns = new();
            //找一个除了自己以外的Pawn
            foreach (var otherPawn in GameManager.Instance.PawnGeneratorTool.PawnsList)
            {
                if (otherPawn != pawn) otherPawns.Add(otherPawn);
            }

            //有无其他Pawn
            if(otherPawns.Count <= 1)
            {
                //输出需求，一次
                if (!isAlone)
                {
                    isAlone = true;
                    string narrative = $"{pawn.Def.PawnName} 想要交朋友，但只有它一个人...";
                    ScenarioManager.Instance.Narrative(narrative, pawn.gameObject);
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
