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
    ///  ״̬���У������µ�״̬���ʱ��������ӵ�״̬������
    /// </summary>
    protected Queue<StateBase> StateQueue;

    /// <summary>
    ///  ״̬�⣬���ĵ�״̬
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
                //״̬���
                case StateType.Success:
                    currentState.IsSuccess = true;
                    TryChangeState();
                    break;
                //״̬ʧ�ܰѵ�ǰ״̬��Ϊ��
                case StateType.Failed:
                    Debug.Log("״̬ʧ�ܣ�" + currentState.ToString());
                    currentState = null;
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
        if (nextState != null)
        {
            ChangeState(nextState);
            nextState = null;
            return;
        }

        //�����ǰ���в�Ϊ�գ���Ӷ����г�һ��״̬����
        if (StateQueue.Count > 0)
        {
            ChangeState(StateQueue.Dequeue());
            return;
        }

        //��Ϊ��������ΪĬ��
        ChangeState(defaultState);
    }

    /// <summary>
    /// �Ǽ�һ��״̬
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
        return StateQueue;
    }

    public bool SpaceStateQueue()
    {
        return StateQueue.Count == 0;
    }

    public void AddStateToQueue(StateBase state)
    {
        StateQueue.Enqueue(state);
        // Debug.Log("�Ѿ����״̬" + state + "�������");
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
    /// �л���ǰ״̬Ϊ newState
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(StateBase newState)
    {
        // �����ǰ״̬δ���
        if (currentState != null && !currentState.IsSuccess)
        {
            InterruptState();
        }
        else if (currentState != null)
        {
            // �˳���ǰ״̬
            currentState.OnExit();
        }

        // �л�����״̬
        currentState = newState;
        if (currentState != null)
        {
            if (currentState.OnEnter())
            {
                Debug.Log("�Ѿ��л���״̬: " + currentState);
                return;
            }
            // δ�ɹ�����
            Debug.Log("����״̬ʧ�ܣ�" + currentState.ToString());
            currentState = null;
        }
        Debug.Log("�л��ɿ�״̬");
    }

    /// <summary>
    /// �жϵ�ǰ״̬
    /// </summary>
    /// <param name="newState"></param>
    private void InterruptState()
    {
        // �жϵ�ǰ״̬
        if (currentState != null)
        {
            currentState.OnInterrupt();
        }

        currentState = null;
    }
}


