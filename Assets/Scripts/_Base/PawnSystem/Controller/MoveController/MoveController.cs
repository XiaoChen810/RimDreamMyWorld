using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_MapGenerator;
using Pathfinding;
using ChenChen_AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public abstract class MoveController : AIPath
{
    protected Animator _anim;
    protected Rigidbody2D _rb;
    protected LineRenderer lineRenderer;
    protected AIPath aiPath;

    // 是否已经到达目标点
    public bool IsReach
    {
        get
        {
            return reachedDestination;
        }
    }
    // 自身上一个位置
    protected Vector3 lastTransPositon;

    protected override void Start()
    {
        base.Start();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        aiPath = GetComponent<AIPath>();

        // init
        lastTransPositon = transform.position;
    }

    protected virtual void Update()
    {
        Filp();
    }

    public override void OnTargetReached()
    {
        _anim.SetBool("IsWalk", false);
        _anim.SetBool("IsRun", false);
        aiPath.maxSpeed = 1;
    }

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        aiPath.destination = target;
        switch (urgency)
        {
            case Urgency.Wander:
                aiPath.maxSpeed = 1;
                break;
            case Urgency.Normal:
                aiPath.maxSpeed = 2;
                break;
            case Urgency.Urge:
                aiPath.maxSpeed = 3;
                break;
            default:
                aiPath.maxSpeed = 1;
                break;
        }
        if(aiPath.maxSpeed <= 1)
        {
            _anim.SetBool("IsWalk", true);
        }
        if(aiPath.maxSpeed >= 1)
        {
            _anim.SetBool("IsRun", true);
        }
        aiPath.endReachedDistance = endReachedDistance;

        return true;
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        aiPath.destination = target.transform.position;
        return true;
    }

    #region DrawPath

    protected void DrawPath(List<Vector2> points)
    {
        // 设置线的宽度等属性
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 设置路径点
        Vector3[] path = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            path[i] = new Vector3(points[i].x, points[i].y);
        }

        // 绘制路径
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
    }

    #endregion

    #region Flip

    protected void Filp()
    {
        if (lastTransPositon != transform.position)
        {
            // 向右边走，正面
            if (lastTransPositon.x < transform.position.x)
            {
                transform.localScale = Vector3.one;
            }
            // 向左边走，反面
            if (lastTransPositon.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            lastTransPositon = transform.position;
        }
    }

    public void FilpLeft()
    {
        lastTransPositon = transform.position;
        transform.localScale = new Vector3(-1, 1, 1);
    }

    public void FilpRight()
    {
        lastTransPositon = transform.position;
        transform.localScale = Vector3.one;
    }

    #endregion
}
