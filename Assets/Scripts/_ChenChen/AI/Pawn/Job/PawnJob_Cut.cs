using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Cut : PawnJob
    {
        //持续最大时间
        private readonly static float tick = 500;

        private Thing_Tree tree;
        private float _time = 0;
        private float _timeOne = 0.76f; // 一次动画的时间

        public PawnJob_Cut(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            //逻辑
            if(!target.TryGetComponent<Thing_Tree>(out tree))
            {
                DebugLogDescription = "获取组件失败";
                return false;
            }

            // 判断左边近还是右边近
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

            // 设置人物接取工作
            pawn.JobToDo(target.GameObject);
            this.Description = "前往砍伐" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            //逻辑
            if (pawn.MoveController.ReachDestination)
            {
                // 设置人物正在工作
                pawn.JobDoing();
                this.Description = "正在砍伐" + target.GameObject.name;

                // 执行工作
                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    tree.OnCut(20);
                    _time = 0;
                }

                // 播放动画
                pawn.MoveController.FilpIt(target.Positon.x);
                pawn.Animator.SetBool("IsLumbering", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (tree.CurDurability <= 0)
            {
                return StateType.Success;
            }

            // 如果取消
            if (!tree.IsMarkCut)
            {
                return StateType.Interrupt;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            // 结束动画
            pawn.Animator.SetBool("IsLumbering", false);
        }

        public override void OnInterrupt()
        {
            OnExit();
        }
    }
}