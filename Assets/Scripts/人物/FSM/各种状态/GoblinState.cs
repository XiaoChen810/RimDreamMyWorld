using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoblinState
{
    /// <summary>
    /// 闲逛
    /// </summary>
    public class Idle : StateBase
    {
        private GoblinMain _goblinMain;
        public Idle(StateMachine machine, GoblinMain goblinMain) : base(machine, null)
        {
            this._goblinMain = goblinMain;
        }

        private float _time = 0f;
        private float maxIdleTime;

        public override void OnEnter()
        {
            maxIdleTime = Random.Range(0.5f, 1f);
        }

        public override StateType OnUpdate()
        {
            _time += Time.deltaTime;

            if (_time < maxIdleTime) return StateType.Doing;

            if (GameManager.Instance.CharactersList.Count > 0 && _goblinMain.attackTarget == null)
            {
                _goblinMain.attackTarget = GameManager.Instance.CharactersList[0];
                _stateMachine.AddState(new GoblinState.Near(_stateMachine, _goblinMain));
                return StateType.Success;
            }

            if(_goblinMain.attackTarget != null)
            {
                _stateMachine.AddState(new GoblinState.Near(_stateMachine, _goblinMain));
                return StateType.Success;
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// 靠近目标
    /// </summary>
    public class Near : StateBase
    {
        private GoblinMain _goblinMain;

        public Near(StateMachine machine,GoblinMain goblinMain) : base(machine, new GoblinState.Chase(machine, goblinMain))
        {
            this._goblinMain = goblinMain;
        }

        public override void OnEnter()
        {
            _goblinMain.GMC.GoToTargetObject(_goblinMain.attackTarget);
            _goblinMain.GMC.IsRun = false;
        }

        public override StateType OnUpdate()
        {
            if(_goblinMain.attackTarget != null)
            {
                float distance = Vector2.Distance(_goblinMain.transform.position, _goblinMain.attackTarget.transform.position);
                if(distance < _goblinMain.ChaseRange)
                {                   
                    return StateType.Success;
                }
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// 追逐目标
    /// </summary>
    public class Chase : StateBase
    {
        private GoblinMain _goblinMain;
        public Chase(StateMachine machine, GoblinMain goblinMain) : base(machine, new GoblinState.Battle(machine, goblinMain))
        {
            this._goblinMain = goblinMain;
        }

        public override void OnEnter()
        {
            _goblinMain.GMC.IsRun = true;
            _goblinMain.GMC.GoToTargetObject(_goblinMain.attackTarget);
            _goblinMain.GMC.JustApproachAttackRange = true;
        }

        public override StateType OnUpdate()
        {
            if (_goblinMain.GMC.IsNearAttackRange)
            {
                return StateType.Success;   
            }

            return StateType.Doing;
        }
    }

    /// <summary>
    /// 进入战斗
    /// </summary>
    public class Battle : StateBase
    {
        private GoblinMain _goblinMain;
        public Battle(StateMachine machine, GoblinMain goblinMain) : base(machine, null)
        {
            this._goblinMain = goblinMain;
        }

        private float _timeCast;
        private float _timeBackswing;

        public override void OnEnter()
        {
            _timeCast = 0;
            _timeBackswing = 0;
        }

        public override StateType OnUpdate()
        {
            _timeCast += Time.deltaTime;

            // 当攻击目标消失
            if(_goblinMain.attackTarget == null)                
            {
                return StateType.Success;
            }
            // 超出攻击距离,并至少播放了一次前摇时
            if (Vector2.Distance(_goblinMain.transform.position, _goblinMain.attackTarget.transform.position) > _goblinMain.AttackRange
                && _timeCast > _goblinMain.AttackCast)
            {
                return StateType.Interrupt;
            }
            // 前摇结束
            if(_timeCast > _goblinMain.AttackCast)
            {
                _goblinMain.Animator.SetBool("IsAttack", true);
                _timeBackswing += Time.deltaTime;
            }
            // 后摇结束
            if(_timeBackswing > _goblinMain.AttackBackswing)
            {
                _stateMachine.AddState(this);
                return StateType.Success;
            }

            return StateType.Doing;
        }

        public override void OnExit()
        {
            _goblinMain.Animator.SetBool("IsAttack", false);
        }

        public override void OnInterrupt()
        {
            _goblinMain.Animator.SetBool("IsAttack", false);
            _stateMachine.AddState(new Idle(_stateMachine, _goblinMain));
        }
    }

}
