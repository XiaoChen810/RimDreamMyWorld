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
    // ������һ��λ��
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

        // ��ʱˢ��
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

        // �ƶ�
        var speedFactor = !reachedEndOfPath ? speed : 0;
        speedFactor = !reachDestination ? speedFactor : 0;
        speedFactor = canMove ? speedFactor : 0;
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.position += dir * speedFactor * Time.deltaTime;

        // �����߼�
        Filp();
        UpdateUrgency(curUrgency);
    }

    protected virtual void OnTargetReached()
    {
        reachDestination = true;
        isStart = false;
    }

    /// <summary>
    /// ǰ����Ŀ���
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
    /// ����Ŀ��
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
    {
        // ����Ҫ׷�ٵ�Ŀ��
        targetDestination = target.transform;
        targetIsAObject = true;
        ReflashDestination(targetDestination.position, urgency, endReachedDistance);
    }
    private void ReflashDestination(Vector3 target, Urgency urgency, float endReachedDistance)
    {
        Debug.Log("Reflash Destination");
        // �½�·��
        ABPath newPath = ABPath.Construct(transform.position, target);
        // ��ʼ����·��
        _seeker.StartPath(newPath, callback: (Path p) =>
        {
            p.Claim(this);
            if (!p.error)
            {
                // �ж�·���Ƿ�ɴ�
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
            // ���ұ��ߣ�����
            if (lastTransPositon.x < transform.position.x)
            {
                transform.localScale = Vector3.one;
            }
            // ������ߣ�����
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
