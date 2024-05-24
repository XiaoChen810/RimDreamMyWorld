using System.Collections;
using UnityEngine;

namespace ChenChen_AI
{
    public class PawnJob_Attack : PawnJob
    {
        private readonly static float tick = 50;
        private Pawn targetComponent;
        private bool isAttacking;

        public PawnJob_Attack(Pawn pawn, GameObject target) : base(pawn, tick,new TargetPtr(target))
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

            return true;
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

            if (!isAttacking)
            {
                pawn.StartCoroutine(AttackCo());
            }

            return StateType.Doing;
        }

        IEnumerator AttackCo()
        {
            isAttacking = true;
            pawn.MoveController.enabled = false;
            yield return new WaitForSeconds(pawn.AttackSpeed);
            Debug.Log("发起攻击");
            pawn.Animator.SetTrigger("IsAttack");
            yield return new WaitForSeconds(pawn.AttackSpeedWait);
            isAttacking = false;
            pawn.MoveController.enabled = true;

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
