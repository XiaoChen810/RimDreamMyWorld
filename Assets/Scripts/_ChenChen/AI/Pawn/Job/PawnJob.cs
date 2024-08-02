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

            if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
            {
                if (!per.GetPermission(pawn))
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
                if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
                {
                    if (!per.RevokePermission(pawn))
                    {
                        Debug.Log($"�黹Ŀ��ʹ��Ȩ��ʧ�ܣ�{pawn.name} to {per.name}");
                    }
                }
            }

            target = null;
            pawn.JobDone();
        }
    }
}