using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    public IState currentState;

    /// <summary>
    ///  ״̬���У������µ�״̬���ʱ����ӵ�״̬������
    /// </summary>
    public Queue<IState> StateQueue;

    /// <summary>
    ///  ��ͣ��������Ϊ��ʱ��״̬��ֻ����е�ǰ״̬��OnUpdate��������仯״̬
    /// </summary>
    public bool isStop;

    // ��ӳ�Ա����
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
            // ִ�е�ǰ״̬��OnUpdate����
            currentState.OnUpdate();

            if (isStop) return;

            // ����״ִ̬����ɣ��������һ��״̬
            if(StateQueue.Count > 0)
            {
                if (currentState.IsLoop) StateQueue.Enqueue(currentState);
                ChangeState(StateQueue.Dequeue());
            }
            else
            {
                // ���û����һ��״̬����ֱ�ӽ�����ǰ״̬���ع�Ĭ��״̬
                currentState = defaultState;
            }
        }
    }

    public void AddState(IState state)
    {
        StateQueue.Enqueue(state);
        Debug.Log("�Ѿ����״̬" + state);
    }

    public void ChangeState(IState newState)
    {
        // �˳���ǰ״̬
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // �л�����״̬
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
        // Debug.Log("Ĭ��״̬��ʼ");
    }

    public override void OnExit()
    {
        // Debug.Log("Ĭ��״̬����");
    }
}

