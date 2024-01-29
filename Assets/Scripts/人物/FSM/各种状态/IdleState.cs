using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class IdleState : StateBase
{
    static readonly bool canLoop = true;
    public IdleState(StateMachine machine) : base(machine, canLoop) { }

    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
      
    }

}
