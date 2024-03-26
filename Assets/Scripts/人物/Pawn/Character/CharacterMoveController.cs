using PawnStates;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(CharacterMain))]
public class CharacterMoveController : MoveController
{
    private Collider2D _collider;
    private CharacterMain _characterMain;

    public override string CurrentMapName { get; protected set; }
    public override float WorkRange { get; protected set; }
    public override float AttackRange { get; protected set; }

    protected override void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _characterMain = GetComponent<CharacterMain>();

        CurrentMapName = "MainMap";
        WorkRange = _characterMain.WorkRange;
        AttackRange = _characterMain.AttackRange;
    }

    protected override void MoveLogic()
    {
        base.MoveLogic();

        // 选中情况下, 鼠标右击，改变目标点
        if (_characterMain.IsSelect && Input.GetMouseButtonDown(1))
        {
            _characterMain.StateMachine.InterruptState();

            _characterMain.StateMachine.SetNextState(new PawnState_Move(
                _characterMain, Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            _characterMain.IsSelect = false;
        }        
    }
}
