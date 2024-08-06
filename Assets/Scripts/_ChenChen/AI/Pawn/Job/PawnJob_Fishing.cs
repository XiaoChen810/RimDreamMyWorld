using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Fishing : PawnJob
    {
        private readonly static float tick = 50;
        private Thing_Furniture curTargetComponent;

        private float _timer;
        private float _oneTime = 10f;

        /// <summary>
        /// 开始钓鱼，需要设置钓鱼位置
        /// </summary>
        public PawnJob_Fishing(Pawn pawn, GameObject target) : base(pawn, tick, new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            curTargetComponent = target.GetComponent<Thing_Furniture>();
            if (curTargetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            bool flag = pawn.MoveController.GoToHere(target.Positon - new Vector3(0, 0.4f), Urgency.Normal);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "正在前往钓鱼";

            return true;
        }   

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在钓鱼";
                _timer += Time.deltaTime;
                pawn.MoveController.FilpRight();
            }

            if (_timer > _oneTime)
            {
                Debug.Log("钓上一条鱼");
                return StateType.Success;
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


