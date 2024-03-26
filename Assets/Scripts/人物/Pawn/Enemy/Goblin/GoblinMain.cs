//using GoblinState;
//using PawnStates;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(GoblinMoveController))]
//public class GoblinMain : Pawn
//{
//    public GoblinMoveController GMC { get; private set; }

//    [Header("当前进攻目标")]
//    public GameObject attackTarget;

//    [Header("追击距离")]
//    public float ChaseRange;

//    [Header("前摇时间")]
//    public float AttackCast = 0.1f;

//    [Header("后摇时间")]
//    public float AttackBackswing = 0.1f;

//    private void Start()
//    {
//        StateMachine = new StateMachine(new PawnState_Idle(this));
//        GMC = GetComponent<GoblinMoveController>();
//        Animator = GetComponent<Animator>();
//    }

//    private new void Update()
//    {
//        if(StateMachine.currentState == null && StateMachine.SpaceStateQueue())
//        {
//            StateMachine.AddStateToQueue(new GoblinState.Idle(StateMachine, this));
//        }
//        StateMachine.Update();
//        任务列表Debug();
//    }
//}
