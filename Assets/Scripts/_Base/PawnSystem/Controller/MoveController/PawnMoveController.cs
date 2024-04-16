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

    protected override void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _pawn = GetComponent<Pawn>();
    }

    protected override void Update()
    {
        base.Update();
        // ���������, ����һ���ǿ���ƶ���Ŀ���,&& _pawn.IsDrafted
        if (_pawn.IsSelect && Input.GetMouseButtonDown(1))
        {
            _pawn.StateMachine.TryChangeState(
                new PawnJob_Move(_pawn, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
}
