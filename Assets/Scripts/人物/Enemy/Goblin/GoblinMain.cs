using GoblinState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GoblinMoveController))]
public class GoblinMain : MonoBehaviour
{
    public GoblinMoveController GMC { get; private set; }

    public StateMachine StateMachine { get; private set; }

    public Animator Animator { get; private set; }

    [Header("��ǰ����Ŀ��")]
    public GameObject attackTarget;

    [Header("��������")]
    public float AttackRange;

    [Header("׷������")]
    public float ChaseRange;

    [Header("ǰҡʱ��")]
    public float AttackCast = 0.1f;

    [Header("��ҡʱ��")]
    public float AttackBackswing = 0.1f;

    [Header("��ǰ����")]
    public List<string> CurrentTaskList = new List<string>();


    private void Start()
    {
        StateMachine = new StateMachine();
        GMC = GetComponent<GoblinMoveController>();
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(StateMachine.currentState == null && StateMachine.StateQueue.Count == 0)
        {
            StateMachine.AddState(new GoblinState.Idle(StateMachine, this));
        }
        StateMachine.Update();
        �����б�Debug();
    }

    private void �����б�Debug()
    {
        CurrentTaskList.Clear();
        CurrentTaskList.Add("���ڣ�" + StateMachine.currentState?.ToString());
        foreach (var task in StateMachine.StateQueue)
        {
            CurrentTaskList.Add("׼��" + task.ToString());
        }
    }
}
