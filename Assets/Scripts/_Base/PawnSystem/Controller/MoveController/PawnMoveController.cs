using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
public class PawnMoveController : MoveController
{
    private Collider2D _collider;
    private Pawn _pawn;
    private LineRenderer _lineRenderer;

    // 紧迫程度
    [SerializeField] protected Urgency curUrgency = Urgency.Normal;

    protected override void Start()
    {
        base.Start();
        _collider = GetComponent<Collider2D>();
        _pawn = GetComponent<Pawn>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        // 选中情况下，按下R征兆
        if (_pawn.IsSelect && Input.GetKeyDown(KeyCode.R))
        {
            _pawn.StateMachine.TryChangeState(new PawnJob_Draft(_pawn, !_pawn.IsDrafted));
        }
        // 征兆情况下, 鼠标右击，移动到鼠标点
        if (_pawn.IsSelect && _pawn.IsDrafted && Input.GetMouseButtonDown(1))
        {
            _pawn.StateMachine.TryChangeState(
                new PawnJob_Move(_pawn, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        // 选中情况下会绘制路径
        DrawPathUpdate();
    }

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        switch (urgency)
        {
            case Urgency.Wander:
                speed = 1;
                break;
            case Urgency.Normal:
                speed = 2;
                break;
            case Urgency.Urge:
                speed = 3;
                break;
            default:
                speed = 2;
                break;
        }
        StartPath(target, speed, endReachedDistance);
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        switch (urgency)
        {
            case Urgency.Wander:
                speed = 1;
                break;
            case Urgency.Normal:
                speed = 2;
                break;
            case Urgency.Urge:
                speed = 3;
                break;
            default:
                speed = 2;
                break;
        }
        StartPath(target, speed, endReachedDistance);
    }

    #region DrawPath

    protected void DrawPathUpdate()
    {
        List<Vector3> pathDraw = new List<Vector3>();
        if (path != null && _pawn.IsSelect)
        {
            for (int i = currentWaypoint; i < path.vectorPath.Count; i++)
            {
                pathDraw.Add(path.vectorPath[i]);
            }
        }
        DrawPath(pathDraw);
    }

    protected void DrawPath(List<Vector3> points)
    {
        // 设置线的宽度等属性
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

        // 设置路径点
        Vector3[] draw = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            draw[i] = new Vector3(points[i].x, points[i].y);
        }

        // 绘制路径
        _lineRenderer.positionCount = draw.Length;
        _lineRenderer.SetPositions(draw);
    }

    #endregion
}
