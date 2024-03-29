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

    // �ƶ��ٶ�
    public float MoveSpeed;
    // ·������ʱ��
    public float PathUpdateIntervalTime;
    // �ܷ��ƶ�
    public bool CanMove = false;
    // �Ƿ��ڱ���
    public bool IsRun;
    // �Ƿ��Ѿ�����Ŀ���
    public bool IsReach;
    // �Ƿ��Ѿ����빤����Χ
    public bool IsNearWorkRange;
    // �Ƿ��Ѿ����빥����Χ
    public bool IsNearAttackRange;
    // �Ƿ�ѡ��ֻ����������Χ
    public bool JustApproachWorkRange;
    // �Ƿ�ѡ��ֻ����������Χ
    public bool JustApproachAttackRange;
    // ��ǰ���ڵĵ�ͼ
    public abstract string CurrentMapName { get; protected set; }
    // ������Χ
    public abstract float WorkRange { get; protected set; }
    // ������Χ
    public abstract float AttackRange {  get; protected set; }


    // ��Ҫ�ƶ�����Ŀ���
    protected Vector3 moveTargetPos;
    // ��һ����Ҫ�ƶ���Ŀ���
    protected Vector3 lastMoveTargetPos;
    // ·���б�
    protected List<Vector2> movePathList = null;
    // ��ǰ·�����������
    protected int currentWaypointIndex = -1;
    // ������һ��λ��
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
        // ��ȡ·��
        movePathList = MapManager.Instance.GetPath(transform.position, moveTargetPos, CurrentMapName);

        if (movePathList == null)
        {
            Debug.Log("�޷��ҵ�·����" + moveTargetPos);
            EndMove();
            return;
        }

        // ����·��������
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        lastMoveTargetPos = moveTargetPos;
        return;
    }

    /// <summary>
    /// ���»�ȡ·��
    /// </summary>
    protected bool ResetPath(Vector3 target)
    {
        // ��ȡ·��
        movePathList = MapManager.Instance.GetPath(transform.position, target, CurrentMapName);

        if(movePathList == null)
        {
            Debug.Log("�޷��ҵ�·����" + target);
            EndMove();
            return false;
        }

        // ����·��������
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        lastMoveTargetPos = moveTargetPos = target;
        return true;
    }

    #region Virtual

    protected virtual void MoveLogic()
    {
        // ���Ŀ���ı䣬���»�ȡ·��
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
    /// ǰ��Ŀ��ֱ������
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
        // ���ݵ�ǰ��·������·��
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
        // �����ߵĿ�ȵ�����
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // ����·����
        Vector3[] path = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            path[i] = new Vector3(points[i].x, points[i].y);
        }

        // ����·��
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
    }

    #endregion

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

    #region Move

    protected void Move()
    {
        if(!CanMove) return;

        // �ж�·���Ƿ����
        if (movePathList == null || movePathList.Count == 0 || currentWaypointIndex == -1)
        {
            EndMove();
            return;
        }

        // �����ƶ��ķ�����ٶ�
        Vector3 currentWaypoint = movePathList[currentWaypointIndex];
        Vector3 dir = (currentWaypoint - transform.position).normalized;
        float speed = IsRun ? MoveSpeed * 2 : MoveSpeed;
        _rb.velocity = dir * speed * Time.deltaTime;
        _anim.SetBool("IsRun", IsRun);
        _anim.SetBool("IsWalk", !IsRun);
        // �����뵱ǰĿ��ľ���
        float distanceToTarget = Vector2.Distance(transform.position, moveTargetPos);
        // �����뵱ǰ·����ľ���
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint);
        // ��֡�н�Ҫ�ƶ��ľ���
        float distanceToMove = _rb.velocity.magnitude * Time.deltaTime;

        // �ж��Ƿ��Ѿ��㹻�ӽ�������Χ
        if(distanceToTarget < WorkRange)
        {
            IsNearWorkRange = true;
            if(JustApproachWorkRange) EndMove();
        }

        // �ж��Ƿ��Ѿ��㹻�ӽ�������Χ
        if (distanceToTarget < AttackRange)
        {
            IsNearAttackRange = true;
            if (JustApproachAttackRange) EndMove();
        }

        // �ж��Ƿ��Ѿ���ȫ����Ŀ���
        if (distanceToWaypoint < 0.01f || distanceToMove >= distanceToWaypoint)
        {
            // ����ǳ��ӽ���ǰ·���㣬���߱�֡�ƶ��ľ��뽫�����˵�·����ľ��룬�л�����һ��·���� 
            currentWaypointIndex++;

            // �������·����ĩβ������
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
        Debug.Log("ֹͣ�ƶ�");
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
