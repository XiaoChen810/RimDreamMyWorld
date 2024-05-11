using UnityEngine;

namespace ChenChen_AI
{
    public abstract class StateBase : IState
    {
        protected StateMachine _stateMachine { get; private set; }

        /// <summary>
        /// 这个状态的最大存在时间
        /// </summary>
        public float MaxTick;

        /// <summary>
        /// 这个状态是否是成功结束的
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// 错误日志
        /// </summary>
        public string DebugLogDescription;

        public StateBase(StateMachine machine, StateBase next = null)
        {
            _stateMachine = machine;
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
            Debug.Log("Interrupt" + ToString());
        }
    }
}