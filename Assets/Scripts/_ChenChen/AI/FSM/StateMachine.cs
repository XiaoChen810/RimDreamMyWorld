using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [System.Serializable]
    public class StateMachine
    {
        protected StateBase _currentState = null;
        protected StateBase _nextState = null;
        protected StateBase _defaultState = null;
        protected Queue<StateBase> _StateQueue;
        protected float _maxTick = -1;

        /// <summary>
        /// 当前状态
        /// </summary>
        public StateBase CurState
        {
            get
            { 
                return _currentState; 
            }
            set
            {
                _currentState = value;
            }
        }

        /// <summary>
        /// 当前状态类型
        /// </summary>
        public Type CurStateType
        {
            get
            {
                if (CurState == null)
                {
                    return null;
                }
                return CurState.GetType();
            }
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
        /// 当前状态的默认状态
        /// </summary>
        public StateBase DefaultState
        {
            get { return _defaultState; }
            set
            {
                _defaultState = value;
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

        /// <summary>
        /// 当前状态的最大运行时间
        /// </summary>
        public float MaxTick
        {
            get { return _maxTick; }
            set
            {
                _tickTime = Time.time;
                _maxTick = value;
            }
        }

        public GameObject Owner;
        private float _tickTime;

        public StateMachine(GameObject owner)
        {
            _StateQueue = new Queue<StateBase>();
            Owner = owner;
        }

        public StateMachine(GameObject owner, StateBase defaultState)
        {
            _StateQueue = new Queue<StateBase>();
            _defaultState = defaultState;
            Owner = owner;

        }

        public void Update()
        {
            if (_currentState != null)
            {
                switch (_currentState.OnUpdate())
                {
                    case StateType.Success:
                        _currentState.IsSuccess = true;
                        TryChangeState(_currentState.NextStateDefault);
                        break;
                    case StateType.Failed:
                        _currentState.OnExit();
                        _currentState = null;
                        break;
                    case StateType.Doing:
                        if (_tickTime > Time.time + _maxTick)
                        {
                            _tickTime = Time.time;
                            TryChangeState();
                        }
                        break;
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

            if (_nextState != null)
            {
                ChangeState(_nextState);
                _nextState = null;
                return;
            }

            if (_StateQueue.Count > 0)
            {
                ChangeState(_StateQueue.Dequeue());
                return;
            }

            ChangeState(_defaultState);
        }

        /// <summary>
        /// 切换当前状态为 newState, 当前状态如果未完成会中断
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(StateBase newState)
        {
            if (_currentState != null && !_currentState.IsSuccess)
            {
                InterruptState();
            }
            else if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = newState;
            if (_currentState != null)
            {
                if (_currentState.OnEnter())
                {
                    MaxTick = newState.MaxTick;
                    return;
                }
                if (_currentState.DebugLogDescription != null)
                {
                    string log = _currentState.DebugLogDescription;
                    Debug.Log($"{Owner.name}进入状态 {_currentState} 失败，当前状态自动切换为空：\n" +
                        $"失败原因: {log}");
                }
                _currentState = null;
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
}