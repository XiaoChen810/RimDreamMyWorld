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
    /// ��ǰ״̬
    /// </summary>
    public StateBase CurState
    {
        get { return _currentState; }
        set { _currentState = value; }
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
                //״̬���
                case StateType.Success:
                    _currentState.IsSuccess = true;
                    TryChangeState();
                    break;
                //״̬ʧ�ܰѵ�ǰ״̬��Ϊ��
                case StateType.Failed:
                    Debug.Log("״̬ʧ�ܣ�" + _currentState.ToString());
                    _currentState = null;
                    break;
                //״̬���ڽ���ʲôҲ������
                case StateType.Doing:
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
        if (newState != null)
        {
            ChangeState(newState);
            return;
        }

        //�����һ��Ŀ��״̬��Ϊ�գ����л�����һ��״̬
        if (_nextState != null)
        {
            ChangeState(_nextState);
            _nextState = null;
            return;
        }

        //�����ǰ���в�Ϊ�գ���Ӷ����г�һ��״̬����
        if (_StateQueue.Count > 0)
        {
            ChangeState(_StateQueue.Dequeue());
            return;
        }

        //��Ϊ��������ΪĬ��
        ChangeState(defaultState);
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
                Debug.Log($"{Owner.name}�л���״̬: " + _currentState);
                return;
            }
            // δ�ɹ�����
            _currentState = null;
            Debug.Log($"{Owner.name}����״̬ {_currentState} ʧ�ܣ���ǰ״̬�Զ��л�Ϊ�գ�");
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


