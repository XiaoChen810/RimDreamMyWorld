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

        public bool IsIdle => CurState == null && NextState == null;

        /// <summary>
        /// ��ǰ״̬
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
        /// ��ǰ״̬����
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
        /// ״̬���У������µ�״̬���ʱ��������ӵ�״̬������
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

        public StateMachine(GameObject owner, StateBase defaultState)
        {
            _StateQueue = new Queue<StateBase>();
            _defaultState = defaultState;
            Owner = owner;

        }

        public void Update()
        {
            if (CurState != null)
            {
                switch (CurState.OnUpdate())
                {
                    case StateType.Success:
                        CurState.IsSuccess = true;
                        TryChangeState(CurState.NextStateDefault);
                        break;
                    case StateType.Failed:
                        CurState.OnExit();
                        CurState = null;
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
        /// �л���ǰ״̬Ϊ newState, ��ǰ״̬���δ��ɻ��ж�
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(StateBase newState)
        {
            if (CurState != null && !_currentState.IsSuccess)
            {
                InterruptState();
            }
            else if (CurState != null)
            {
                CurState.OnExit();
            }

            CurState = newState;
            if (CurState != null)
            {
                if (CurState.OnEnter())
                {
                    MaxTick = newState.MaxTick;
                    return;
                }
                if (CurState.DebugLogDescription != null)
                {
                    string log = CurState.DebugLogDescription;
                    Debug.Log($"{Owner.name}����״̬ {CurState} ʧ�ܣ���ǰ״̬�Զ��л�Ϊ�գ�\n" +
                        $"ʧ��ԭ��: {log}");
                }
                CurState = null;
            }
        }

        /// <summary>
        /// �жϵ�ǰ״̬
        /// </summary>
        /// <param name="newState"></param>
        private void InterruptState()
        {
            // �жϵ�ǰ״̬
            if (CurState != null)
            {
                CurState.OnInterrupt();
            }

            CurState = null;
        }
    }
}