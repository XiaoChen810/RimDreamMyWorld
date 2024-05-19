using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Battle : PawnJob
    {
        private readonly static float tick = 500;
        private Pawn targetComponent;

        public PawnJob_Battle(Pawn pawn, GameObject target) : base(pawn, tick,new TargetPtr(target))
        {
        }

        public override bool OnEnter()
        {
            targetComponent = target.GetComponent<Pawn>();
            if (targetComponent == null)
            {
                DebugLogDescription = ("尝试获取Pawn组件失败");
                return false;
            }

            return pawn.TryToEnterBattle(targetComponent);
        }

        public override StateType OnUpdate()
        {
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;

            pawn.JobDoing();
            
            //返回成功
            //目标被杀死
            if (targetComponent.Info.IsDead)
            {
                return StateType.Success;
            }

            //返回失败
            //超出攻击距离
            if (Vector2.Distance(target.Positon, pawn.transform.position) > pawn.AttackRange)
            {
                return StateType.Failed;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            base.OnExit();

            pawn.TryToEndBattle();
        }

        public override void OnInterrupt()
        {
            OnExit();
        }


    }
}
