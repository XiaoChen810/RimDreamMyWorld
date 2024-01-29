using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : IState
{
    protected StateMachine _stateMachine { get; private set; }

    public bool IsLoop { get; }

    public StateBase(StateMachine machine,bool canLoop)
    {
        this._stateMachine = machine;
        this.IsLoop = canLoop;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void OnUpdate()
    {
        // 运行

        // 当达到某条件时结束或暂停
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
