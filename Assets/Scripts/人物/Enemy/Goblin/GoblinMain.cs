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

    [Header("当前进攻目标")]
    public GameObject attackTarget;

    [Header("攻击距离")]
    public float AttackRange;

    [Header("追击距离")]
    public float ChaseRange;

    [Header("前摇时间")]
    public float AttackCast = 0.1f;

    [Header("后摇时间")]
    public float AttackBackswing = 0.1f;

    [Header("当前任务")]
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
        任务列表Debug();
    }

    private void 任务列表Debug()
    {
        CurrentTaskList.Clear();
        CurrentTaskList.Add("正在：" + StateMachine.currentState?.ToString());
        foreach (var task in StateMachine.StateQueue)
        {
            CurrentTaskList.Add("准备" + task.ToString());
        }
    }
}
