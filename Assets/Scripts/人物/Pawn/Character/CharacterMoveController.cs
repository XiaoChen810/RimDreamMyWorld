using ChenChen_AI;
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

        // ѡ�������, ����һ����ı�Ŀ���
        if (_characterMain.IsSelect && Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _characterMain.StateMachine.StateQueue.Enqueue(
                    new PawnJob_Move(_characterMain, Camera.main.ScreenToWorldPoint(Input.mousePosition)));

                _characterMain.IsSelect = false;
                return;
            }
            _characterMain.StateMachine.TryChangeState(
                new PawnJob_Move(_characterMain, Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            _characterMain.IsSelect = false;
        }
    }
}
