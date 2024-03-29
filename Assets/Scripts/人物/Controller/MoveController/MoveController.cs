using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_MapGenerator;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public abstract class MoveController : MonoBehaviour
{
    protected Animator _anim;
    protected Rigidbody2D _rb;
    protected LineRenderer lineRenderer;

    // 移动速度
    public float MoveSpeed;
    // 路径更新时间
    public float PathUpdateIntervalTime;
    // 能否移动
    public bool CanMove = false;
    // 是否在奔跑
    public bool IsRun;
    // 是否已经到达目标点
    public bool IsReach;
    // 是否已经进入工作范围
    public bool IsNearWorkRange;
    // 是否已经进入攻击范围
    public bool IsNearAttackRange;
    // 是否选择只靠近工作范围
    public bool JustApproachWorkRange;
    // 是否选择只靠近攻击范围
    public bool JustApproachAttackRange;
    // 当前所在的地图
    public abstract string CurrentMapName { get; protected set; }
    // 工作范围
    public abstract float WorkRange { get; protected set; }
    // 攻击范围
    public abstract float AttackRange {  get; protected set; }


    // 所要移动到的目标点
    protected Vector3 moveTargetPos;
    // 上一个所要移动的目标点
    protected Vector3 lastMoveTargetPos;
    // 路径列表
    protected List<Vector2> movePathList = null;
    // 当前路径点索引序号
    protected int currentWaypointIndex = -1;
    // 自身上一个位置
    protected Vector3 lastTransPositon;

    protected virtual void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // init
        moveTargetPos = transform.position;
        lastMoveTargetPos = transform.position;
        lastTransPositon = transform.position;

        MapManager.Instance.MapObstaclesChange += ResetPath;
    }

    protected void Update()
    {
        MoveLogic();
        DrawLineForCurrentPath();
        Filp();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected void ResetPath()
    {
        // 获取路径
        movePathList = MapManager.Instance.GetPath(transform.position, moveTargetPos, CurrentMapName);

        if (movePathList == null)
        {
            Debug.Log("无法找到路径：" + moveTargetPos);
            EndMove();
            return;
        }

        // 重置路径点索引
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        lastMoveTargetPos = moveTargetPos;
        return;
    }

    /// <summary>
    /// 重新获取路径
    /// </summary>
    protected bool ResetPath(Vector3 target)
    {
        // 获取路径
        movePathList = MapManager.Instance.GetPath(transform.position, target, CurrentMapName);

        if(movePathList == null)
        {
            Debug.Log("无法找到路径：" + target);
            EndMove();
            return false;
        }

        // 重置路径点索引
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        lastMoveTargetPos = moveTargetPos = target;
        return true;
    }

    #region Virtual

    protected virtual void MoveLogic()
    {
        // 如果目标点改变，重新获取路径
        if (lastMoveTargetPos != moveTargetPos)
        {
            ResetPath(moveTargetPos);
        }
    }

    #endregion

    #region Public

    public void StopMove()
    {
        EndMove();
    }

    public bool GoToHere(Vector3 target, bool findBetterPosIfFailed = false)
    {
        if(!ResetPath(target))
        {
            if(findBetterPosIfFailed)
            {
                if (TryFindBetterPos(target))
                {
                    StartMove();
                    return true;
                }
            }
            return false;
        }
        StartMove();
        return true;
    }

    public bool TryFindBetterPos(Vector3 target)
    {
        if (ResetPath(target + new Vector3(1, 0))) return true;
        if (ResetPath(target + new Vector3(-1, 0))) return true;
        if (ResetPath(target + new Vector3(0, 1))) return true;
        if (ResetPath(target + new Vector3(0, -1))) return true;
        return false;
    }

    /// <summary>
    /// 前往目标直至结束
    /// </summary>
    /// <param name="target"></param>
    public void GoToTargetObject(GameObject target)
    {
        StartCoroutine(UpdateResetPathCo(target));
    }

    private IEnumerator UpdateResetPathCo(GameObject target)
    {
        do
        {
            GoToHere(target.transform.position);
            yield return new WaitForSeconds(PathUpdateIntervalTime);

        } while (CanMove);
    }

    #endregion

    #region DrawPath

    protected void DrawLineForCurrentPath()
    {
        // 根据当前的路径绘制路线
        if (currentWaypointIndex != -1 && movePathList != null)
        {
            List<Vector2> currentPath = new List<Vector2>();
            for (int i = currentWaypointIndex; i < movePathList.Count; i++)
            {
                currentPath.Add(movePathList[i]);
            }
            DrawPath(currentPath);
        }
    }

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

    #region Move

    protected void Move()
    {
        if(!CanMove) return;

        // 判断路径是否存在
        if (movePathList == null || movePathList.Count == 0 || currentWaypointIndex == -1)
        {
            EndMove();
            return;
        }

        // 设置移动的方向和速度
        Vector3 currentWaypoint = movePathList[currentWaypointIndex];
        Vector3 dir = (currentWaypoint - transform.position).normalized;
        float speed = IsRun ? MoveSpeed * 2 : MoveSpeed;
        _rb.velocity = dir * speed * Time.deltaTime;
        _anim.SetBool("IsRun", IsRun);
        _anim.SetBool("IsWalk", !IsRun);
        // 自身与当前目标的距离
        float distanceToTarget = Vector2.Distance(transform.position, moveTargetPos);
        // 自身与当前路径点的距离
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint);
        // 这帧中将要移动的距离
        float distanceToMove = _rb.velocity.magnitude * Time.deltaTime;

        // 判断是否已经足够接近工作范围
        if(distanceToTarget < WorkRange)
        {
            IsNearWorkRange = true;
            if(JustApproachWorkRange) EndMove();
        }

        // 判断是否已经足够接近攻击范围
        if (distanceToTarget < AttackRange)
        {
            IsNearAttackRange = true;
            if (JustApproachAttackRange) EndMove();
        }

        // 判断是否已经完全到达目标点
        if (distanceToWaypoint < 0.01f || distanceToMove >= distanceToWaypoint)
        {
            // 如果非常接近当前路径点，或者本帧移动的距离将超过了到路径点的距离，切换到下一个路径点 
            currentWaypointIndex++;

            // 如果到达路径的末尾，结束
            if (currentWaypointIndex >= movePathList.Count)
            {
                EndMove();
            }

        }
    }

    protected void StartMove()
    {
        CanMove = true;
        IsReach = false;
        IsNearWorkRange = false;
        IsNearAttackRange = false;
    }

    protected void EndMove()
    {
        Debug.Log("停止移动");
        CanMove = false;
        IsReach = true;
        currentWaypointIndex = -1;
        _rb.velocity = new Vector2(0, 0);
        _anim.SetBool("IsWalk", false);
        _anim.SetBool("IsRun", false);
        lineRenderer.positionCount = 0;
        JustApproachAttackRange = false;
        JustApproachWorkRange = false;
        movePathList = null;
    }

    #endregion
}
