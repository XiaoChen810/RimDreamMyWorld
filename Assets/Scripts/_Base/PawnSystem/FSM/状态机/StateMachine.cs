using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    protected StateBase _currentState = null;
    protected StateBase _nextState = null;
    protected Queue<StateBase> _StateQueue;
    protected StateBase defaultState;

    /// <summary>
    /// 当前状态
    /// </summary>
    public StateBase CurState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    /// <summary>
    /// 下一个状态
    /// </summary>
    public StateBase NextState
    {
        get { return _nextState; }
        set
        {
            _nextState = value;
        }
    }

    /// <summary>
    /// 状态队列，当有新的状态添加时，可以添加到状态队列里
    /// </summary>
    public Queue<StateBase> StateQueue
    {
        get { return _StateQueue; }
        set
        {
            _StateQueue = value;
        }
    }

    public Pawn Owner;

    public StateMachine(StateBase defaultState, Pawn owner)
    {
        _StateQueue = new Queue<StateBase>();
        this.Owner = owner;
        this.defaultState = defaultState;
    }

    public void Update()
    {
        if (_currentState != null)
        {
            switch (_currentState.OnUpdate())
            {
                //状态完成
                case StateType.Success:
                    _currentState.IsSuccess = true;
                    TryChangeState();
                    break;
                //状态失败把当前状态设为空
                case StateType.Failed:
                    Debug.Log("状态失败：" + _currentState.ToString());
                    _currentState = null;
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

    public void TryChangeState(StateBase newState = null)
    {
        if (newState != null)
        {
            ChangeState(newState);
            return;
        }

        //如果下一个目标状态不为空，则切换成下一个状态
        if (_nextState != null)
        {
            ChangeState(_nextState);
            _nextState = null;
            return;
        }

        //如果当前队列不为空，则从队列中抽一个状态出来
        if (_StateQueue.Count > 0)
        {
            ChangeState(_StateQueue.Dequeue());
            return;
        }

        //都为空则设置为默认
        ChangeState(defaultState);
    }

    /// <summary>
    /// 切换当前状态为 newState, 当前状态如果未完成会中断
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(StateBase newState)
    {
        // 如果当前状态未完成
        if (_currentState != null && !_currentState.IsSuccess)
        {
            InterruptState();
        }
        else if (_currentState != null)
        {
            // 退出当前状态
            _currentState.OnExit();
        }

        // 切换到新状态
        _currentState = newState;
        if (_currentState != null)
        {
            if (_currentState.OnEnter())
            {
                Debug.Log($"{Owner.name}切换成状态: " + _currentState);
                return;
            }
            // 未成功进入
            _currentState = null;
            Debug.Log($"{Owner.name}进入状态 {_currentState} 失败，当前状态自动切换为空：");
        }
    }

    /// <summary>
    /// 中断当前状态
    /// </summary>
    /// <param name="newState"></param>
    private void InterruptState()
    {
        // 中断当前状态
        if (_currentState != null)
        {
            _currentState.OnInterrupt();
        }

        _currentState = null;
    }
}


