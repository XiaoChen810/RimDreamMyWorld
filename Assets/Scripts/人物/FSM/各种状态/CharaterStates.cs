using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������������ܵ�����״̬
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
            Debug.Log("���ڽ���" + CLC.currentBuiltObject.name);
            // �ߵ��Ǳ�ȥ
            CLC.MoveControl.GoToHere(buildPos);
            _stateMachine.isStop = true;
        }

        public override void OnUpdate()
        {
            // ������Ŀ�꿪ʼ����
            if (CLC.MoveControl.isReach)
            {
                int builtSpeed = 1;
                CLC.currentBuiltObject.Build(builtSpeed * Time.deltaTime);
            }

            // �������˽��죬״̬��������ͣ�����Խ�����һ��״̬
            if(CLC.currentBuiltObject._workloadRemainder <= 0)
            {
                _stateMachine.isStop = false;
            }

        }

        public override void OnExit()
        {
            CLC.currentBuiltObject.Complete();
            Debug.Log("�Ѿ���ɱ��ν���" + CLC.currentBuiltObject.name);

            CLC.currentBuiltObject = null;
            CLC.IsGetTask = false;
        }
    }
}


