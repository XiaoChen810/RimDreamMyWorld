using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    public IState currentState;

    /// <summary>
    ///  状态队列，当有新的状态添加时，添加到状态队列里
    /// </summary>
    public Queue<IState> StateQueue;

    /// <summary>
    ///  暂停变量，当为真时，状态机只会进行当前状态的OnUpdate，而不会变化状态
    /// </summary>
    public bool isStop;

    // 添加成员变量
    private DefaultState defaultState;  

    public StateMachine()
    {
        StateQueue = new Queue<IState>();
        defaultState = new DefaultState(this);
        currentState = defaultState;
    }

    public void Update()
    {
        if (currentState != null)
        {
            // 执行当前状态的OnUpdate方法
            currentState.OnUpdate();

            if (isStop) return;

            // 当该状态执行完成，会进入下一个状态
            if(StateQueue.Count > 0)
            {
                if (currentState.IsLoop) StateQueue.Enqueue(currentState);
                ChangeState(StateQueue.Dequeue());
            }
            else
            {
                // 如果没有下一个状态，则直接结束当前状态，回归默认状态
                currentState = defaultState;
            }
        }
    }

    public void AddState(IState state)
    {
        StateQueue.Enqueue(state);
        Debug.Log("已经添加状态" + state);
    }

    public void ChangeState(IState newState)
    {
        // 退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // 切换到新状态
        currentState = newState;
        currentState.OnEnter();
    }
}

public class DefaultState : StateBase
{
    public static readonly bool CanLoop = true;
    public DefaultState(StateMachine machine) : base(machine, CanLoop) { }

    public override void OnEnter()
    {
        // Debug.Log("默认状态开始");
    }

    public override void OnExit()
    {
        // Debug.Log("默认状态结束");
    }
}

