using UnityEngine;

namespace ChenChen_AI
{
    public abstract class StateBase : IState
    {
        protected StateMachine _stateMachine { get; private set; }
        public float MaxTick;
        public bool IsSuccess;

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