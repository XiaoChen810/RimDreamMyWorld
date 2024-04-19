using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using ChenChen_AI;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class MoveController : MonoBehaviour
{
    protected Animator _anim;
    protected Rigidbody2D _rb;
    protected Seeker _seeker;

    [Header("Debug")]
    // 自身上一个位置
    protected Vector3 lastTransPositon;
    // 开始寻路
    [SerializeField] protected bool isStart = false;
    // 能否移动，isStart 和 canMove同时为 true 才会动
    [SerializeField] protected bool canMove = true;
    // 移动速度
    [SerializeField] protected float speed = 2f;
    // 移动点更新的距离，也是判断到达目标点的距离
    [SerializeField] protected float endReachedDistance = 0.2f;
    // 开启动态刷新时，每次刷新的间隔时间
    [SerializeField] protected float repathRate = 0.5f;
    // 目标点
    [SerializeField] protected Vector3 destination;
    // 目标Transform
    [SerializeField] protected Transform targetDestination;
    // 目标是否是个物体
    [SerializeField] protected bool targetIsAObject = false;
    // 到达路径末尾
    [SerializeField] protected bool reachedEndOfPath = true;
    // 到达目标点
    [SerializeField] protected bool reachDestination = true;
    // 当前在路径的哪个点
    [SerializeField] protected int currentWaypoint = 0;


    /// <summary>
    /// Current path;
    /// </summary>
    protected Path path;    

    public bool ReachDestination
    {
        get
        {
            return reachDestination;
        }
    }

    private float lastRepath = float.NegativeInfinity;


    protected virtual void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
        
        // init
        lastTransPositon = transform.position;
        destination = transform.position;
    }

    protected virtual void Update()
    {
        if (!isStart) return;

        if (!targetIsAObject)
        {
            targetDestination = null;
        }

        // 定时刷新
        if (Time.time > lastRepath + repathRate && _seeker.IsDone())
        {
            lastRepath = Time.time;
            ReflashDestination(targetDestination != null ? targetDestination.position : destination, speed, endReachedDistance);
        }
        //Debug.Log($"Time.time  {Time.time} > lastRepath + repathRate {lastRepath + repathRate} _seeker.IsDone() {_seeker.IsDone()}");
        

        if (path == null)
        {
            return;
        }

        reachedEndOfPath = false;

        float distanceToWaypoint;
        distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < endReachedDistance)
        {
            if (currentWaypoint + 1 < path.vectorPath.Count)
            {
                currentWaypoint++;
            }
            else
            {
                if(targetIsAObject)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetDestination.position);
                    if(distanceToTarget > endReachedDistance)
                    {
                        return;
                    }
                }
                reachedEndOfPath = true;
                OnTargetReached();
            }
        }

        // 移动
        var speedFactor = !reachedEndOfPath ? speed : 0;
        speedFactor = !reachDestination ? speedFactor : 0;
        speedFactor = canMove ? speedFactor : 0;
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.position += dir * speedFactor * Time.deltaTime;

        // 其他逻辑
        Filp();
        UpdateMoveAnimation();
    }

    protected virtual void OnTargetReached()
    {
        reachDestination = true;
        isStart = false;
    }

    private void UpdateMoveAnimation()
    {
        if (!isStart || !canMove)
        {
            speed = 0;
            _anim.SetBool("IsWalk", false);
            _anim.SetBool("IsRun", false);
            return;
        }
        if (speed > 0 && speed <= 1)
        {
            _anim.SetBool("IsWalk", true);
        }
        if (speed > 1)
        {
            _anim.SetBool("IsRun", true);
        }
    }

    #region Path

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected void StartPath(Vector3 destination, float speed, float endReachedDistance = 0.2f)
    {
        targetDestination = null;
        targetIsAObject = false;
        ReflashDestination(destination, speed, endReachedDistance);
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected void StartPath(GameObject destination, float speed, float endReachedDistance = 0.2f)
    {
        // 设置要追踪的目标
        targetDestination = destination.transform;
        targetIsAObject = true;
        ReflashDestination(targetDestination.position, speed, endReachedDistance);
    }

    private void ReflashDestination(Vector3 destination, float speed, float endReachedDistance)
    {
        //Debug.Log("Reflash Destination");
        // 新建路径
        ABPath newPath = ABPath.Construct(transform.position, destination);
        // 开始计算路径
        _seeker.StartPath(newPath, callback: (Path p) =>
        {
            p.Claim(this);
            if (!p.error)
            {
                // 判断路径是否可达
                Vector3 end = p.vectorPath[p.vectorPath.Count - 1];
                if (Vector2.Distance(end, destination) < endReachedDistance)
                {
                    this.destination = destination;
                    this.speed = speed;
                    this.endReachedDistance = endReachedDistance;
                    if (path != null) path.Release(this);
                    path = p;
                    isStart = true;
                    canMove = true;
                    reachDestination = false;
                    currentWaypoint = 0;
                }
                else
                {
                    //Debug.Log("This point don't has path can reach: " + target
                    //    + " the path end node is: " + end);
                    return;
                }
            }
            else
            {
                p.Release(this);
            }
        });
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
