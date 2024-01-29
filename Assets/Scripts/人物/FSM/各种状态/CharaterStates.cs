using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这里包括了人物可能的所有状态
/// </summary>
public class CharacterStates
{

    public class BuildState : StateBase
    {
        public static readonly bool CanLoop = false;

        private CharacterLogicController CLC;
        private Vector2 buildPos;
        public BuildState(CharacterLogicController CLC, Vector2 buildPos) : base(CLC.StateMachine.SM, CanLoop)
        {
            this.CLC = CLC;
            this.buildPos = buildPos;
        }

        public override void OnEnter()
        {
            Debug.Log("正在建造" + CLC.currentBuiltObject.name);
            // 走到那边去
            CLC.MoveControl.GoToHere(buildPos);
            _stateMachine.isStop = true;
        }

        public override void OnUpdate()
        {
            // 当靠近目标开始建造
            if (CLC.MoveControl.isReach)
            {
                int builtSpeed = 1;
                CLC.currentBuiltObject.Build(builtSpeed * Time.deltaTime);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if(CLC.currentBuiltObject._workloadRemainder <= 0)
            {
                _stateMachine.isStop = false;
            }

        }

        public override void OnExit()
        {
            CLC.currentBuiltObject.Complete();
            Debug.Log("已经完成本次建造" + CLC.currentBuiltObject.name);

            CLC.currentBuiltObject = null;
            CLC.IsGetTask = false;
        }
    }
}


