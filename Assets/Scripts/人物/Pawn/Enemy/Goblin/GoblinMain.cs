//using GoblinState;
//using PawnStates;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(GoblinMoveController))]
//public class GoblinMain : Pawn
//{
//    public GoblinMoveController GMC { get; private set; }

//    [Header("��ǰ����Ŀ��")]
//    public GameObject attackTarget;

//    [Header("׷������")]
//    public float ChaseRange;

//    [Header("ǰҡʱ��")]
//    public float AttackCast = 0.1f;

//    [Header("��ҡʱ��")]
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
//        �����б�Debug();
//    }
//}
