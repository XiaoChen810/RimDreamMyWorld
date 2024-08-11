using UnityEngine;

namespace ChenChen_AI
{
    public abstract class StateBase : IState
    {
        protected StateMachine _stateMachine { get; private set; }

        /// <summary>
        /// 这个状态默认的下一个状态
        /// </summary>
        public StateBase NextStateDefault { get; private set; }

        /// <summary>
        /// 这个状态的最大存在时间
        /// </summary>
        public float MaxTick { get; private set; }

        /// <summary>
        /// 这个状态是否是成功结束的
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误日志
        /// </summary>
        public string DebugLogDescription { get; set; }

        private string description = string.Empty;
        /// <summary>
        /// 状态描述
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