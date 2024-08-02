using ChenChen_Thing;
using System;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Demolished : PawnJob
    {
        private readonly static float tick = 50;
        private Thing targetComponent;
        private float _time;
        private float _timeOne;

        public PawnJob_Demolished(Pawn pawn, GameObject building) : base(pawn, tick,new TargetPtr(building))
        {
            float ability = pawn.Attribute.A_Construction.Value;
            float a = 1 - (ability / 20);
            _timeOne = Mathf.Lerp(1f, 10, a) / 2;
        }

        public override bool OnEnter()
        {
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;

            targetComponent = target.GetComponent<Thing>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("尝试获取组件失败");
                return false;
            }

            bool flag = pawn.MoveController.GoToHere(target.Positon, Urgency.Urge, pawn.WorkRange);
            if (!flag)
            {
                DebugLogDescription = ("无法移动到目标点");
                return false;
            }

            pawn.JobToDo(target.GameObject);
            this.Description = "前往拆除" + target.GameObject.name;

            return true;
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            if (targetComponent == null)
            {
                return StateType.Success;
            }

            if (targetComponent.CurDurability <= 0)
            {
                return StateType.Success;
            }

            if (targetComponent.LifeState != BuildingLifeStateType.MarkDemolished)
            {
                return StateType.Interrupt;
            }

            if (pawn.MoveController.ReachDestination)
            {
                pawn.JobDoing();
                this.Description = "正在拆除" + target.GameObject.name;

                _time += Time.deltaTime;
                if (_time > _timeOne)
                {
                    targetComponent.OnDemolish(pawn.Attribute.A_Construction.Value);
                    _time = 0;
                }

                pawn.Animator.SetBool("IsDoing", true);
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.Animator.SetBool("IsDoing", false);
        }

        public override void OnInterrupt()
        {
            // 归还目标使用权限
            targetComponent.RevokePermission(pawn);

            OnExit();
        }
    }
}