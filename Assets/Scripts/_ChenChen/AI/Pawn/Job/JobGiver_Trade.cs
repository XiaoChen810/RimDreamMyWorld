using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Trade : JobGiver
    {
        private static readonly float INTERVAL_TIME = 1f;
        private static readonly string JOBNAME = "驯兽";

        public JobGiver_Trade(Action<TargetPtr> onGetJobSuccessly) : base(onGetJobSuccessly, JOBNAME, INTERVAL_TIME)
        {
        }

        protected override TargetPtr TryGiveJob(Pawn pawn)
        {
            var animals = GameManager.Instance.AnimalGenerateTool.AnimalList;
            foreach (var animal in animals)
            {
                if (animal.WaitToTrade)
                {
                    return new TargetPtr(animal.gameObject);
                }
            }

            return null;
        }
    }
}
