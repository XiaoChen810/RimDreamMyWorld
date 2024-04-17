using ChenChen_AI;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : AILerp
{
    protected Animator _anim;
    protected Rigidbody2D _rb;
    protected LineRenderer lineRenderer;
    // 自身上一个位置
    protected Vector3 lastTransPositon;

    [SerializeField] private bool _isReach;
    public bool IsReach
    {
        get
        {
            _isReach = reachedDestination;
            return _isReach;
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
    protected override void Update()
    {
        base.Update();
        if (lastTransPositon == transform.position && !reachedDestination)
        {
            _time += Time.deltaTime;
        }
        if (lastTransPositon != transform.position)
        {
            _time = 0;
        }
        if (_time > 2)
        {
            Debug.Log("wait out time");
            _time = 0;
            destination = transform.position;
        }
        Filp();
        DrawPathUpdate();
    }

    public override void OnTargetReached()
    {  
        _anim.SetBool("IsWalk", false);
        _anim.SetBool("IsRun", false);
        speed = 1;
        canMove = false;
    }

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        ReflashDestination(target, urgency, endReachedDistance);
    }

    private void ReflashDestination(Vector3 target, Urgency urgency, float endReachedDistance)
    {
        // 新建路径
        ABPath newPath = ABPath.Construct(transform.position, target);
        // 开始计算路径
        seeker.StartPath(newPath, (Path p) =>
        {
            // 判断路径是否可达
            Vector3 end = p.vectorPath[p.vectorPath.Count - 1];
            if (Vector2.Distance(end, target) < endReachedDistance)
            {
                destination = target;
                InitUrgency(urgency);
                //this.endReachedDistance = endReachedDistance;
                canMove = true;
            }
            else
            {
                Debug.Log("This point don't has path can reach: " + target
                    + " the path end node is: " + end);
            }
        });
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        // 设置要追踪的目标
        if (TryGetComponent<AIDestinationSetter>(out AIDestinationSetter ai))
        {
            if (ai == null)
            {
                ai = this.gameObject.AddComponent<AIDestinationSetter>();
            }
            ai.target = target.transform;
            InitUrgency(urgency);
            //this.endReachedDistance = endReachedDistance;
        }
    }

    private void InitUrgency(Urgency urgency)
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
                speed = 1;
                break;
        }
        if (speed <= 1)
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
        if (path != null)
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
