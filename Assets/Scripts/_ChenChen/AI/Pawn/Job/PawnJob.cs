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
            if (target != null && target.TargetA != null)
            {
                if (target.TargetA.TryGetComponent<IGrant>(out var grantA))
                {
                    grantA.GetPermission(pawn);
                    if (grantA.UserPawn != pawn)
                    {
                        DebugLogDescription = ($"Ŀ���Ѿ� {grantA.UserPawn.name} ��ʹ��");
                        return false;
                    }
                }
            }

            if (target != null && target.TargetB != null)
            {
                if (target.TargetB.TryGetComponent<IGrant>(out var grantB))
                {
                    grantB.GetPermission(pawn);
                    if (grantB.UserPawn != pawn)
                    {
                        DebugLogDescription = ($"Ŀ���Ѿ� {grantB.UserPawn.name} ��ʹ��");
                        return false;
                    }
                }
            }


            return true;
        }

        public override StateType OnUpdate()
        {
            return StateType.Doing;
        }

        /// <summary>
        /// �黹Ŀ��ʹ��Ȩ�ޣ�����еĻ�
        /// ��������״̬
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            if (target != null && target.TargetA != null)
            {
                if (target.TargetA.TryGetComponent<IGrant>(out var grantA))
                {
                    grantA.RevokePermission(pawn);
                }
            }

            if (target != null && target.TargetB != null)
            {
                if (target.TargetB.TryGetComponent<IGrant>(out var grantB))
                {
                    grantB.RevokePermission(pawn);
                }
            }

            target = null;
            pawn.JobDone();
        }
    }
}