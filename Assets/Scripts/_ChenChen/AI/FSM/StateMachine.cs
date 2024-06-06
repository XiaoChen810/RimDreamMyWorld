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
        /// ��ǰ״̬
        /// </summary>
        public StateBase CurState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
            }
        }

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
        /// ��һ��״̬
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
        /// ��ǰ״̬��Ĭ��״̬
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
        /// ״̬���У������µ�״̬����ʱ���������ӵ�״̬������
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
        /// ��ǰ״̬���������ʱ��
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
                    //״̬���
                    case StateType.Success:
                        _currentState.IsSuccess = true;
                        TryChangeState(_currentState.NextStateDefault);
                        break;
                    //״̬ʧ�ܰѵ�ǰ״̬��Ϊ��
                    case StateType.Failed:
                        _currentState.OnExit();
                        _currentState = null;
                        break;
                    //״̬���ڽ���,��ʱ,��ʱ�л�
                    case StateType.Doing:
                        if (_tickTime > Time.time + _maxTick)
                        {
                            _tickTime = Time.time;
                            TryChangeState();
                        }
                        break;
                    //״̬�жϴ����жϺ���
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
            // ��ǰ״̬��Ĭ��
            if (newState != null)
            {
                ChangeState(newState);
                return;
            }

            // ״̬����һ��Ŀ��״̬
            if (_nextState != null)
            {
                ChangeState(_nextState);
                _nextState = null;
                return;
            }

            // �����ǰ���в�Ϊ�գ���Ӷ����г�һ��״̬����
            if (_StateQueue.Count > 0)
            {
                ChangeState(_StateQueue.Dequeue());
                return;
            }

            // ��Ϊ��������ΪĬ��
            ChangeState(_defaultState);
        }

        /// <summary>
        /// �л���ǰ״̬Ϊ newState, ��ǰ״̬���δ��ɻ��ж�
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(StateBase newState)
        {
            // �����ǰ״̬δ���
            if (_currentState != null && !_currentState.IsSuccess)
            {
                InterruptState();
            }
            else if (_currentState != null)
            {
                // �˳���ǰ״̬
                _currentState.OnExit();
            }

            // �л�����״̬
            _currentState = newState;
            if (_currentState != null)
            {
                if (_currentState.OnEnter())
                {
                    //Debug.Log($"{Owner.name}�л���״̬: " + _currentState);
                    MaxTick = newState.MaxTick;
                    return;
                }
                // δ�ɹ�����
                if(_currentState.DebugLogDescription != null)
                {
                    string log = _currentState.DebugLogDescription;
                    Debug.Log($"{Owner.name}����״̬ {_currentState} ʧ�ܣ���ǰ״̬�Զ��л�Ϊ�գ�\n" +
                        $"ʧ��ԭ��: {log}");
                }
                _currentState = null;
            }
        }

        /// <summary>
        /// �жϵ�ǰ״̬
        /// </summary>
        /// <param name="newState"></param>
        private void InterruptState()
        {
            // �жϵ�ǰ״̬
            if (_currentState != null)
            {
                _currentState.OnInterrupt();
            }

            _currentState = null;
        }
    }
}