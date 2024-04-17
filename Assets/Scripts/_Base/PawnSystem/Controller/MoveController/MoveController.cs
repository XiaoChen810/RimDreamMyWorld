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
    [SerializeField] protected bool isStart = false;
    [SerializeField] protected bool canMove = true;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float endReachedDistance = 0.2f;
    [SerializeField] protected float repathRate = 0.5f;
    [SerializeField] protected Vector3 destination;
    [SerializeField] protected Transform targetDestination;
    [SerializeField] protected bool targetIsAObject = false;
    [SerializeField] protected bool reachedEndOfPath = true;
    [SerializeField] protected bool reachDestination = true;
    [SerializeField] protected int currentWaypoint = 0;
    [SerializeField] protected Urgency curUrgency = Urgency.Normal;

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
            ReflashDestination(targetDestination != null ? targetDestination.position : destination, curUrgency, endReachedDistance);
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
        UpdateUrgency(curUrgency);
    }

    protected virtual void OnTargetReached()
    {
        reachDestination = true;
        isStart = false;
    }

    /// <summary>
    /// 前往到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        targetDestination = null;
        targetIsAObject = false;
        ReflashDestination(target, urgency, endReachedDistance);
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        // 设置要追踪的目标
        targetDestination = target.transform;
        targetIsAObject = true;
        ReflashDestination(targetDestination.position, urgency, endReachedDistance);
    }
    private void ReflashDestination(Vector3 target, Urgency urgency, float endReachedDistance)
    {
        Debug.Log("Reflash Destination");
        // 新建路径
        ABPath newPath = ABPath.Construct(transform.position, target);
        // 开始计算路径
        _seeker.StartPath(newPath, callback: (Path p) =>
        {
            p.Claim(this);
            if (!p.error)
            {
                // 判断路径是否可达
                Vector3 end = p.vectorPath[p.vectorPath.Count - 1];
                if (Vector2.Distance(end, target) < endReachedDistance)
                {
                    destination = target;
                    curUrgency = urgency;
                    this.endReachedDistance = endReachedDistance;
                    canMove = true;
                    reachDestination = false;
                    if (path != null) path.Release(this);
                    path = p;
                    currentWaypoint = 0;
                    isStart = true;
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

    private void UpdateUrgency(Urgency urgency)
    {
        if (!isStart)
        {
            speed = 0;
            _anim.SetBool("IsWalk", false);
            _anim.SetBool("IsRun", false);
            return;
        }
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
