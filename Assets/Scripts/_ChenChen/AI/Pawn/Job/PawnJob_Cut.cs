using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cut : PawnJob
    {
        private readonly static float tick = 500;

        private Thing_Tree tree;
        private float _timer = 0;
        private readonly float _timeOne = 0.76f;

        public PawnJob_Cut(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            tree = target.TargetA.GetComponent<Thing_Tree>();
        }

        public override bool OnEnter()
        {
            if (tree == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(tree.transform.position, endReachedDistance: 1))
            {
                DebugLogDescription = "前往目标失败";
                return false;
            }

            pawn.JobToDo(target);
            Description = "前往砍伐" + tree.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (tree.Durability <= 0)
            {
                return StateType.Success;
            }

            if (!tree.IsMarkCut)
            {
                return StateType.Interrupt;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                Description = "正在砍伐";

                _timer += Time.deltaTime;
                if (_timer > _timeOne)
                {
                    tree.OnCut(20);
                    _timer = 0;
                }
            }

            return StateType.Doing;
        }
    }
}