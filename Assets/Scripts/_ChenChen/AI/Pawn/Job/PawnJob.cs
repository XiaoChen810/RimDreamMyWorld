using ChenChen_Core;
using UnityEngine;

namespace ChenChen_AI
{
    public abstract class PawnJob : StateBase
    {
        /// <summary>
        /// ���������������
        /// </summary>
        protected Pawn pawn;

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        protected TargetPtr target;

        public PawnJob(Pawn pawn, float maxTick, TargetPtr target, StateBase next = null) : base(pawn.StateMachine, maxTick, next)
        {
            this.pawn = pawn;
            this.target = target;
        }

        /// <summary>
        /// ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
        /// </summary>
        /// <returns></returns>
        public override bool OnEnter()
        {
            if (target.IsGameObject && target.GameObject == null)
            {
                DebugLogDescription = ("Ŀ��Ϊ��");
                return false;
            }

            if (target.GameObject.TryGetComponent<IGrant>(out var grant))
            {
                grant.GetPermission(pawn);
                if(grant.UserPawn != pawn)
                {
                    DebugLogDescription = ("Ŀ���Ѿ������˱�ʹ��");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// �ж�Ŀ���Ƿ񻹴���
        /// </summary>
        /// <returns></returns>
        public override StateType OnUpdate()
        {
            if(target != null && target.IsGameObject && target.GameObject == null)
            {
                return StateType.Failed;
            }

            return StateType.Doing;
        }

        /// <summary>
        /// �黹Ŀ��ʹ��Ȩ�ޣ�����еĻ�
        /// ��������״̬
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            if (target != null && target.IsGameObject && target.GameObject != null)
            {
                if (target.GameObject.TryGetComponent<IGrant>(out var grant))
                {
                    grant.RevokePermission(pawn);
                }
            }

            target = null;
            pawn.JobDone();
        }
    }
}