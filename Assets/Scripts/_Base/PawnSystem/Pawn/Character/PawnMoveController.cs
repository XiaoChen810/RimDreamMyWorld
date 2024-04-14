using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PawnMoveController : MoveController
{
    private Collider2D _collider;
    private Pawn _pawn;

    public override string CurrentMapName { get; protected set; }
    public override float WorkRange { get; protected set; }
    public override float AttackRange { get; protected set; }

    protected override void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _pawn = GetComponent<Pawn>();

        CurrentMapName = "MainMap";
        WorkRange = _pawn.WorkRange;
        AttackRange = _pawn.AttackRange;
    }

    protected override void MoveLogic()
    {
        base.MoveLogic();

        // 征兆情况下, 鼠标右击，强制移动到目标点
        if (_pawn.IsSelect && _pawn.IsDrafted && Input.GetMouseButtonDown(1))
        {
            _pawn.StateMachine.TryChangeState(
                new PawnJob_Move(_pawn, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
}
