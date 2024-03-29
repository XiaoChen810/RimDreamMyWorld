using UnityEngine;

public abstract class StateBase : IState
{
    protected StateMachine _stateMachine { get; private set; }

    public StateBase nextState {  get; private set; }

    public bool IsSuccess;

    public StateBase(StateMachine machine,StateBase next = null)
    {
        this._stateMachine = machine;
        this.nextState = next;
    }

    public virtual bool OnEnter()
    {
        IsSuccess = true;
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

    }
}
