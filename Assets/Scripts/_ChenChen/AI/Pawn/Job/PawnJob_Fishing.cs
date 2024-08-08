using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Fishing : PawnJob
    {
        private readonly static float tick = 50;
        private Thing fishingComponent;

        private float _timer;
        private float _oneTime = 10f;

        /// <summary>
        /// 开始钓鱼，需要设置钓鱼位置
        /// </summary>
        public PawnJob_Fishing(Pawn pawn, TargetPtr target) : base(pawn, tick, target)
        {
            fishingComponent = target.TargetA.GetComponent<Thing>();
        }

        public override bool OnEnter()
        {
            if (fishingComponent == null)
            {
                return false;
            }

            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            if (!pawn.MoveController.GoToHere(fishingComponent.transform.position, Urgency.Normal))
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(target);
            Description = "正在前往钓鱼";

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
            }

            if (_timer > _oneTime)
            {
                Debug.Log("钓上一条鱼");
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }
}


