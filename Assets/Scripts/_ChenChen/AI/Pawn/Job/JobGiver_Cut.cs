using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Cut : JobGiver
    {
        public JobGiver_Cut(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly, intervalTime: 8)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            return ThingSystemManager.Instance.GetTreeToCut();
        }
    }
}
