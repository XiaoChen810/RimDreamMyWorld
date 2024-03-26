using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    public StateBase currentState = null;
    protected StateBase nextState = null;
    protected StateBase defaultState;

    /// <summary>
    ///  状态队列，当有新的状态添加时，可以添加到状态队列里
    /// </summary>
    protected Queue<StateBase> StateQueue;

    /// <summary>
    ///  状态库，订阅的状态
    /// </summary>
    public Dictionary<string, StateBase> StateDictionary;

    public StateMachine(StateBase defaultState)
    {
        StateQueue = new Queue<StateBase>();
        StateDictionary = new Dictionary<string, StateBase>();
        this.defaultState = defaultState;
    }

    public void Update()
    {
        if (currentState != null)
        {
            switch (currentState.OnUpdate())
            {
                //状态完成
                case StateType.Success:
                    currentState.IsSuccess = true;
                    TryChangeState();
                    break;
                //状态失败把当前状态设为空
                case StateType.Failed:
                    Debug.Log("状态失败：" + currentState.ToString());
                    currentState = null;
                    break;
                //状态正在进行什么也不处理
                case StateType.Doing:
                    break;
                //状态中断触发中断函数
                case StateType.Interrupt:
                    InterruptState();
                    break;
            }
            return;
        }

        TryChangeState();
    }

    private void TryChangeState()
    {
        //如果下一个目标状态不为空，则切换成下一个状态
        if (nextState != null)
        {
            ChangeState(nextState);
            nextState = null;
            return;
        }

        //如果当前队列不为空，则从队列中抽一个状态出来
        if (StateQueue.Count > 0)
        {
            ChangeState(StateQueue.Dequeue());
            return;
        }

        //都为空则设置为默认
        ChangeState(defaultState);
    }

    /// <summary>
    /// 登记一个状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="stateName"></param>
    public void RegisteredState(StateBase state,string stateName)
    {
        if (!StateDictionary.ContainsKey(stateName))
        {
            StateDictionary.Add(stateName, state);
        }
    }

    public Queue<StateBase> GetStateQueue()
    {
        StateQueue ??= new Queue<StateBase>();
        return StateQueue;
    }

    public bool SpaceStateQueue()
    {
        return StateQueue.Count == 0;
    }

    public void AddStateToQueue(StateBase state)
    {
        StateQueue.Enqueue(state);
        // Debug.Log("已经添加状态" + state + "进入队列");
    }

    public StateBase GetNextState()
    {
        return nextState;
    }

    public void SetNextState(StateBase next)
    {
        nextState = next;
    }

    /// <summary>
    /// 切换当前状态为 newState
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(StateBase newState)
    {
        // 如果当前状态未完成
        if (currentState != null && !currentState.IsSuccess)
        {
            currentState.OnInterrupt();
        }
        else if(currentState != null) 
        {
            // 退出当前状态
            currentState.OnExit();
        }

        // 切换到新状态
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter();
            Debug.Log("已经切换成状态: " + newState);
        }
        else
        {
            Debug.Log("切换成空状态");
        }

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


