using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StateMachine 
{
    public StateBase currentState;

    /// <summary>
    ///  ״̬���У������µ�״̬���ʱ����ӵ�״̬������
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
    ///  �����״̬���ǵý�ֹ��״̬����ͣʱ�����״̬
    /// </summary>
    /// <param name="state"></param>
    public void AddState(StateBase state)
    {
        StateQueue.Enqueue(state);
        // Debug.Log("�Ѿ����״̬" + state + "�������");
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    StateQueue.Enqueue(state);
        //    Debug.Log("�Ѿ����״̬" + state);
        //}
        //Debug.LogWarning("��û�а�סShift");
    }

    /// <summary>
    /// �ı䵱ǰ״̬��� newState
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(StateBase newState)
    {
        // �˳���ǰ״̬
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // �л�����״̬
        currentState = newState;
        if(currentState != null) currentState.OnEnter();

        // Debug.Log("�Ѿ��л�״̬" + newState);
    }

    /// <summary>
    /// �жϵ�ǰ״̬
    /// </summary>
    /// <param name="newState"></param>
    public void InterruptState()
    {
        // �жϵ�ǰ״̬
        if (currentState != null)
        {
            currentState.OnInterrupt();
        }

        currentState = null;
    }
}


