using UnityEngine;

namespace ChenChen_AI
{
    public abstract class StateBase : IState
    {
        protected StateMachine _stateMachine { get; private set; }

        /// <summary>
        /// ���״̬Ĭ�ϵ���һ��״̬
        /// </summary>
        public StateBase NextStateDefault { get; private set; }

        /// <summary>
        /// ���״̬��������ʱ��
        /// </summary>
        public float MaxTick { get; private set; }

        /// <summary>
        /// ���״̬�Ƿ��ǳɹ�������
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// ������־
        /// </summary>
        public string DebugLogDescription { get; set; }

        private string description = string.Empty;
        /// <summary>
        /// ״̬����
        /// </summary>
        public string Description
        {
            get
            {
                if (description == string.Empty)
                {
                    description = GetType().Name;
                }
                return description;
            }
            set
            {
                description = value;
            }
        }


        public StateBase(StateMachine machine, StateBase next = null)
        {
            _stateMachine = machine;
            NextStateDefault = next;
        }

        public StateBase(StateMachine machine,float maxTick, StateBase next = null)
        {
            _stateMachine = machine;
            MaxTick = maxTick;
            NextStateDefault = next;
        }

        public virtual bool OnEnter()
        {
            return true;
        }

        public virtual void OnExit()
        {

        }

        public virtual StateType OnUpdate()
        {
            return StateType.Success;
        }

        public virtual StateType OnFixedUpdate()
        {
            return StateType.Success;
        }

        public virtual void OnPause()
        {

        }

        public virtual void OnResume()
        {

        }

        public virtual void OnInterrupt()
        {
            Debug.Log("Interrupt" + this.ToString());
            OnExit();
        }
    }
}