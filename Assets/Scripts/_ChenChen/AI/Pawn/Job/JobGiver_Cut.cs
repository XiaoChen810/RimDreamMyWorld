using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class JobGiver_Cut : JobGiver
    {
        public JobGiver_Cut(Action<GameObject> onGetJobSuccessly) : base(onGetJobSuccessly)
        {
        }

        protected override GameObject TryGiveJob(Pawn pawn)
        {
            List<Thing_Tree> trees = ThingSystemManager.Instance.GetThingsInstance<Thing_Tree>();
            foreach (Thing_Tree tree in trees)
            {
                if (tree.IsMarkCut)
                {
                    return tree.gameObject;
                }
            }

            return null;
        }
    }
}
