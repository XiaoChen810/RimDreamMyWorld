using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    public StateBase currentState;

    /// <summary>
    ///  状态队列，当有新的状态添加时，添加到状态队列里
    /// </summary>
    public Queue<StateBase> StateQueue;

    public StateMachine()
    {
        StateQueue = new Queue<StateBase>();
    }

    public void Update()
    {
        if(currentState == null && StateQueue.Count > 0)
        {
            ChangeState(StateQueue.Dequeue());
        }
        else
        {
            if(currentState != null)
            {
                switch (currentState.OnUpdate())
                {
                    case StateType.Success:
                        ChangeState(currentState.nextState);
                        break;
                    case StateType.Failed:
                        currentState = null;
                        break;
                    case StateType.Doing:
                        break;
                    case StateType.Interrupt:
                        InterruptState();
                        break;

                }
            }
        }
    }

    /// <summary>
    ///  添加新状态，记得禁止再状态机暂停时添加新状态
    /// </summary>
    /// <param name="state"></param>
    public void AddState(StateBase state)
    {
        StateQueue.Enqueue(state);
        // Debug.Log("已经添加状态" + state + "进入队列");
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    StateQueue.Enqueue(state);
        //    Debug.Log("已经添加状态" + state);
        //}
        //Debug.LogWarning("并没有按住Shift");
    }

    /// <summary>
    /// 改变当前状态变成 newState
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(StateBase newState)
    {
        // 退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // 切换到新状态
        currentState = newState;
        if(currentState != null) currentState.OnEnter();

        // Debug.Log("已经切换状态" + newState);
    }

    /// <summary>
    /// 中断当前状态
    /// </summary>
    /// <param name="newState"></param>
    public void InterruptState()
    {
        // 中断当前状态
        if (currentState != null)
        {
            currentState.OnInterrupt();
        }

        currentState = null;
    }
}


