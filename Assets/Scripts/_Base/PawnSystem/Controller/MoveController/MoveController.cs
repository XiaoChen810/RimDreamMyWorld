using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // 自身上一个位置
    protected Vector3 lastTransPositon;

    public bool IsReach
    {
        get
        {
            return reachedDestination;
        }
    }

    protected override void Start()
    {
        base.Start();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // init
        lastTransPositon = transform.position;
        destination = transform.position;
    }

    private float _time = 0;
    protected virtual void Update()
    {
        if(lastTransPositon == transform.position && !reachedDestination)
        {
            _time += Time.deltaTime;
        }
        if(lastTransPositon != transform.position)
        {
            _time = 0;
        }
        if(_time > 2)
        {
            Debug.Log("wait out time");
            _time = 0;
            destination = transform.position;
            CancelCurrentPathRequest();
            // Release current path so that it can be pooled
            if (path != null) path.Release(this);
            path = null;
            interpolatorPath.SetPath(null);
            reachedEndOfPath = true;
        }
        Filp();
        DrawPathUpdate();
    }

    public override void OnTargetReached()
    {
        _anim.SetBool("IsWalk", false);
        _anim.SetBool("IsRun", false);
        maxSpeed = 1;
    }

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        InitUrgency(urgency);
        this.endReachedDistance = endReachedDistance;
        destination = target;
        return true;
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        GetComponent<AIDestinationSetter>().target = target.transform;  
        destination = target.transform.position;     
        return true;
    }

    private void InitUrgency(Urgency urgency)
    {
        switch (urgency)
        {
            case Urgency.Wander:
                maxSpeed = 1;
                break;
            case Urgency.Normal:
                maxSpeed = 2;
                break;
            case Urgency.Urge:
                maxSpeed = 3;
                break;
            default:
                maxSpeed = 1;
                break;
        }
        if (maxSpeed <= 1)
        {
            _anim.SetBool("IsWalk", true);
        }
        else
        {
            _anim.SetBool("IsRun", true);
        }
    }

    #region DrawPath

    protected void DrawPathUpdate()
    {
        List<Vector3> pathDraw = new List<Vector3>();
        if(path != null)
        {
            pathDraw = path.vectorPath;
            DrawPath(pathDraw);
        }
    }

    protected void DrawPath(List<Vector3> points)
    {
        // 设置线的宽度等属性
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 设置路径点
        Vector3[] path = new Vector3[points.Count - 1];
        for (int i = 0; i < points.Count - 1; i++)
        {
            path[i] = new Vector3(points[i + 1].x, points[i + 1].y);
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
