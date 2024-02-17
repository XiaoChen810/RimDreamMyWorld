using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
/// ���������������ܵ�����״̬
/// </summary>
namespace CharacterStates
{
    public class IdleState : StateBase
    {    /// <summary>
         /// վ��״̬��ʲô��û��
         /// </summary>
        public IdleState(StateMachine machine) : base(machine, null) { }

        public override StateType OnUpdate()
        {
            if(_stateMachine.StateQueue.Count > 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// ����״̬���ƶ���Ŀ����ͼ��ʼ����
    /// </summary>
    public class BuildState : StateBase
    {
        private CharacterMain Main;
        private Vector2 buildPos;
        /// <summary>
        /// ����һ���µĽ���������Ҫ���ý�������
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="buildPos"></param>
        public BuildState(CharacterMain characterMain, Vector2 buildPos) : base(characterMain.StateMachine, null)
        {
            this.Main = characterMain;
            this.buildPos = buildPos;
        }

        public override void OnEnter()
        {
            // ����Ŀ���
            Main.MoveControl.GoToHere(buildPos);
            Main.MoveControl.JustApproachWorkRange = true;
            Main.MoveControl.IsRun = true;
        }

        public override StateType OnUpdate()
        {
            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (Main.MoveControl.IsNearWorkRange)
            {
                // ��ʼ����
                Main.CurrentBuiltObject.Build(Main.buildSpeed * Time.deltaTime);

                // ���Ŷ���
                Main.Animator.SetBool("IsDoing", true);
            }

            // �������˽��죬״̬��������ͣ�����Խ�����һ��״̬
            if (Main.CurrentBuiltObject._workloadAlready <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Main.CurrentBuiltObject.Complete();
            // Debug.Log("�Ѿ���ɱ��ν���" + CLC.currentBuiltObject.name);

            Main.CurrentBuiltObject = null;
            Main.IsOnWork = false;
            Main.MoveControl.JustApproachWorkRange = false;
            Main.MoveControl.IsRun = false;

            // ��������
            Main.Animator.SetBool("IsDoing", false);
        }
    }

    /// <summary>
    /// �ƶ�״̬���ı�Ŀ��㣬������˳�״̬
    /// </summary>
    public class MoveState : StateBase
    {
        private CharacterMain Main;
        private Vector2 targetPos;

        /// <summary>
        /// �ı��ƶ�Ŀ�꣬������Ŀ���
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="targetPos"></param>
        public MoveState(CharacterMain characterMain, Vector2 targetPos) : base(characterMain.StateMachine, null)
        {
            this.Main = characterMain;
            this.targetPos = targetPos;
        }

        public override void OnEnter()
        {
            // ����Ŀ���
            Main.MoveControl.GoToHere(targetPos);
        }

        public override StateType OnUpdate()
        {
            // �ж��Ƿ񵽴�Ŀ���
            if (Main.MoveControl.IsReach)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// ��ˮ״̬��ǰ������ˮ�ĵط���ˮ
    /// </summary>
    public class WaterState : StateBase
    {
        private CharacterMain Main;
        private Vector2 waterPos;

        private float _time;

        /// <summary>
        /// ���뽽ˮ״̬�������ý�ˮ����
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="waterPos"></param>
        public WaterState(CharacterMain characterMain, Vector2 waterPos) : base(characterMain.StateMachine, null)
        {
            this.Main = characterMain;
            this.waterPos = waterPos;
        }

        public override void OnEnter()
        {
            // ����Ŀ���
            Main.MoveControl.GoToHere(waterPos);
        }

        public override StateType OnUpdate()
        {
            // �ж��Ƿ񵽴�Ŀ��㸽��
            if (Main.MoveControl.IsReach)
            {
                // ���Ŷ���
                Main.Animator.SetBool("IsWatering", true);
                _time += Time.deltaTime;
            }

            // �������˽��죬״̬��������ͣ�����Խ�����һ��״̬
            if (_time >= 1)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Main.Animator.SetBool("IsWatering", false);
        }
    }

    /// <summary>
    /// ����״̬��ǰ������㣬��ʱ��仯
    /// </summary>
    public class FishingState : StateBase
    {
        private CharacterMain Main;
        private Vector2 fishPos;

        private float _time;

        /// <summary>
        /// ��ʼ���㣬��Ҫ���õ���λ��
        /// </summary>
        /// <param name="characterMain"></param>
        /// <param name="fishPos"></param>
        public FishingState(CharacterMain characterMain, Vector2 fishPos) : base(characterMain.StateMachine, null)
        {
            this.Main = characterMain;
            this.fishPos = fishPos;
        }

        public override void OnEnter()
        {
            // ����Ŀ���
            Main.MoveControl.GoToHere(fishPos);
        }

        public override StateType OnUpdate()
        {
            if (Main.MoveControl.IsReach)
            {
                _time += Time.deltaTime;
            }

            if (_time >= 1) Main.Animator.SetInteger("IsFishing", 1);
            if (_time >= 7) Main.Animator.SetInteger("IsFishing", 2);

            if (_time >= 10)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Main.IsOnWork = false;
            Main.Animator.SetInteger("IsFishing", 0);
        }
    }
}


