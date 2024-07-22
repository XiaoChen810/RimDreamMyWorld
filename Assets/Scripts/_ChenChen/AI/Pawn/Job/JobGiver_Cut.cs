using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Cut : JobGiver
    {
        private static readonly string jobName = "砍伐";

        public JobGiver_Cut(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, jobName, intervalTime: 8)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetTreeToCut();
        }
    }
}
