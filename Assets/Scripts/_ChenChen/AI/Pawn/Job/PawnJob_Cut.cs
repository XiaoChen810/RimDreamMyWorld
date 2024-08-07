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

        public PawnJob_Cut(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if(!target.TryGetComponent<Thing_Tree>(out tree))
            {
                DebugLogDescription = "��ȡ���ʧ��";
                return false;
            }

            Vector3 position;
            if (pawn.transform.position.x < target.PositonA.x)
            {               
                 position = target.PositonA + new Vector3(-1, 0.3f); 
            }
            else
            {
                 position = target.PositonA + new Vector3(2, 0.3f);
            }

            if (!pawn.MoveController.GoToHere(position))
            {
                DebugLogDescription = "ǰ��Ŀ���ʧ��";
                return false;
            }

            pawn.JobToDo(target);
            this.Description = "ǰ������" + target.TargetA.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "���ڿ���" + target.TargetA.name;

                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    tree.OnCut(20);
                    _time = 0;
                }

                pawn.MoveController.FilpIt(target.PositonA.x);
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