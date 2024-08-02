using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Trade : JobGiver
    {
        private static readonly float INTERVAL_TIME = 5;
        private static readonly float PROBABILITY = 1;
        private static readonly string JOBNAME = "驯兽";
        public JobGiver_Trade(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, JOBNAME, INTERVAL_TIME, PROBABILITY)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            var animals = GameManager.Instance.AnimalGenerateTool.AnimalList;
            foreach (var animal in animals)
            {
                if (animal.WaitToTrade)
                {
                    return animal.gameObject;
                }
            }

            return null;
        }
    }
}
