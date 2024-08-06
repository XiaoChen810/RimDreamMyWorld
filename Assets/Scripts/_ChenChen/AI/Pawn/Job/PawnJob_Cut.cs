using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cut : PawnJob
    {
        private readonly static float tick = 500;

        private Thing_Tree tree;
        private float _time = 0;
        private float _timeOne = 0.76f;

        public PawnJob_Cut(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if(!target.TryGetComponent<Thing_Tree>(out tree))
            {
                DebugLogDescription = "获取组件失败";
                return false;
            }

            Vector3 position;
            if (pawn.transform.position.x < target.Positon.x)
            {               
                 position = target.Positon + new Vector3(-1, 0.3f); 
            }
            else
            {
                 position = target.Positon + new Vector3(2, 0.3f);
            }

            if (!pawn.MoveController.GoToHere(position))
            {
                DebugLogDescription = "前往目标点失败";
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "前往砍伐" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在砍伐" + target.GameObject.name;

                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    tree.OnCut(20);
                    _time = 0;
                }

                pawn.MoveController.FilpIt(target.Positon.x);
            }

            if (tree.Durability <= 0)
            {
                return StateType.Success;
            }

            if (!tree.IsMarkCut)
            {
                return StateType.Interrupt;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}