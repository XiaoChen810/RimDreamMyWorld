using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
/// 这里包括了人物可能的所有状态
/// </summary>
namespace CharacterStates
{
    public class IdleState : StateBase
    {    /// <summary>
         /// 站立状态，什么都没有
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
    /// 建造状态，移动到目标蓝图开始建筑
    /// </summary>
    public class BuildState : StateBase
    {
        private CharacterMain Main;
        private Vector2 buildPos;
        /// <summary>
        /// 创建一个新的建造任务，需要设置建筑坐标
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
            // 设置目标点
            Main.MoveControl.GoToHere(buildPos);
            Main.MoveControl.JustApproachWorkRange = true;
            Main.MoveControl.IsRun = true;
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (Main.MoveControl.IsNearWorkRange)
            {
                // 开始建造
                Main.CurrentBuiltObject.Build(Main.buildSpeed * Time.deltaTime);

                // 播放动画
                Main.Animator.SetBool("IsDoing", true);
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
            if (Main.CurrentBuiltObject._workloadAlready <= 0)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            Main.CurrentBuiltObject.Complete();
            // Debug.Log("已经完成本次建造" + CLC.currentBuiltObject.name);

            Main.CurrentBuiltObject = null;
            Main.IsOnWork = false;
            Main.MoveControl.JustApproachWorkRange = false;
            Main.MoveControl.IsRun = false;

            // 结束动画
            Main.Animator.SetBool("IsDoing", false);
        }
    }

    /// <summary>
    /// 移动状态，改变目标点，到达后退出状态
    /// </summary>
    public class MoveState : StateBase
    {
        private CharacterMain Main;
        private Vector2 targetPos;

        /// <summary>
        /// 改变移动目标，需设置目标点
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
            // 设置目标点
            Main.MoveControl.GoToHere(targetPos);
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点
            if (Main.MoveControl.IsReach)
            {
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// 浇水状态，前往到浇水的地方浇水
    /// </summary>
    public class WaterState : StateBase
    {
        private CharacterMain Main;
        private Vector2 waterPos;

        private float _time;

        /// <summary>
        /// 进入浇水状态，需设置浇水坐标
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
            // 设置目标点
            Main.MoveControl.GoToHere(waterPos);
        }

        public override StateType OnUpdate()
        {
            // 判断是否到达目标点附近
            if (Main.MoveControl.IsReach)
            {
                // 播放动画
                Main.Animator.SetBool("IsWatering", true);
                _time += Time.deltaTime;
            }

            // 如果完成了建造，状态机结束暂停，可以进入下一个状态
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
    /// 钓鱼状态，前往钓鱼点，随时间变化
    /// </summary>
    public class FishingState : StateBase
    {
        private CharacterMain Main;
        private Vector2 fishPos;

        private float _time;

        /// <summary>
        /// 开始钓鱼，需要设置钓鱼位置
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
            // 设置目标点
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


