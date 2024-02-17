using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : IState
{
    protected StateMachine _stateMachine { get; private set; }

    public StateBase nextState {  get; private set; } 

    public StateBase(StateMachine machine,StateBase next)
    {
        this._stateMachine = machine;
        this.nextState = next;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual StateType OnUpdate()
    {
        // ����

        // ���ﵽĳ����ʱ��������ͣ
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
