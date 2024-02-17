using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoblinMoveController : MoveController
{
    private Collider2D _collider;
    private GoblinMain _main;

    public override string CurrentMapName { get ; protected set ; }
    public override float WorkRange { get; protected set; }
    public override float AttackRange { get; protected set; }

    protected override void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _main = GetComponent<GoblinMain>();

        CurrentMapName = "MainMap";
        WorkRange = -1f;
        AttackRange = _main.AttackRange;
    }

}
