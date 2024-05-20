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
         /* ʹ������
            var baseResult = base.OnEnter();
            if (baseResult != true) return baseResult;
         */
        public override bool OnEnter()
        {
            // ����ȡ��Ȩ�ޣ�Ԥ����ǰ���������Ŀ�걻ʹ��
            if (target.IsGameObject && target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
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
         /* ʹ������
            var baseResult = base.OnUpdate();
            if (baseResult != StateType.Doing) return baseResult;
         */
        public override StateType OnUpdate()
        {
            if(target != null && target.IsGameObject && target.GameObject == null)
            {
                return StateType.Failed;
            }

            // ����ִ�������߼�
            return StateType.Doing;
        }

        /// <summary>
        /// �黹Ŀ��ʹ��Ȩ�ޣ�����еĻ�
        /// ��������״̬
        /// </summary>
        public override void OnExit()
        {
            base.OnExit();

            // ���Ŀ���Ƿ�Ϊ���Լ�Ŀ��� GameObject �Ƿ��ѱ�����
            if (target != null && target.IsGameObject && target.GameObject != null)
            {
                // ���Ի�ȡ PermissionBase ������黹ʹ��Ȩ��
                if (target.GameObject.TryGetComponent<PermissionBase>(out PermissionBase per))
                {
                    if (!per.RevokePermission(pawn))
                    {
                        Debug.Log($"�黹Ŀ��ʹ��Ȩ��ʧ�ܣ�{pawn.name} to {per.name}");
                    }
                }
            }

            target = null;
            // ��������״̬
            pawn.JobDone();
        }
    }
}