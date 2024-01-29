using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 地图生成;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class CharacterMoveController : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private LineRenderer lineRenderer;
    private CharacterLogicController logicConrtroller;

    // 移动速度
    public float moveSpeed;
    // 是否在奔跑
    public bool isRun;
    // 是否已经到达目标点
    public bool isReach;
    // 当前所在的地图
    public string currentMapName;

    // 所要移动的目标点
    private Vector2 MoveTargetPos;
    // 上一个所要移动的目标点
    private Vector2 LastMoveTargetPos;
    // 路径点偏移值
    private Vector2 waypointOffset = new Vector2(0.5f, 0.5f);
    // 路径列表
    private List<Vector2> movePathList = null;
    // 当前路径点索引序号
    private int currentWaypointIndex = -1;

    // 自身上一个位置
    private Vector3 lastTransPositon;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        logicConrtroller = GetComponent<CharacterLogicController>();

        MoveTargetPos = transform.position;
        LastMoveTargetPos = transform.position;
        lastTransPositon = transform.position;
        currentMapName = "MainMap";
    }

    private void Update()
    {
        MovePathLogic();
        DrawLineForCurrentPath();
        Filp();
    }

    #region Public

    public void GoToHere(Vector2 target)
    {
        Debug.Log("改变目标点");
        MoveTargetPos = target;
        ResetPath();
    }

    #endregion



    private void MovePathLogic()
    {
        // 选中情况下,如果目标点改变，重新获取路径
        if (logicConrtroller.IsSelect && LastMoveTargetPos != MoveTargetPos)
        {
            ResetPath();
        }

        // 选中情况下, 鼠标右击，改变目标点
        if (logicConrtroller.IsSelect && Input.GetMouseButtonDown(1))
        {
            MoveTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        isReach = currentWaypointIndex == -1;
    }

    private void ResetPath()
    {
        // 获取路径
        movePathList = MapManager.Instance.GetPath(transform.position, MoveTargetPos, currentMapName);

        // 重置路径点索引
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        LastMoveTargetPos = MoveTargetPos;
    }

    private void DrawLineForCurrentPath()
    {
        // 根据当前的路径绘制路线
        if (currentWaypointIndex != -1)
        {
            List<Vector2> currentPath = new List<Vector2>();
            for (int i = currentWaypointIndex; i < movePathList.Count; i++)
            {
                currentPath.Add(movePathList[i]);
            }
            DrawPath(currentPath);
        }
    }

    private void DrawPath(List<Vector2> points)
    {
        // 设置线的宽度等属性
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 设置路径点
        Vector3[] path = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            path[i] = new Vector3(points[i].x + waypointOffset.x, points[i].y + waypointOffset.y);
        }

        // 绘制路径
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
    }

    private void Filp()
    {
        if(lastTransPositon != transform.position)
        {
            // 向右边走，正面
            if(lastTransPositon.x < transform.position.x)
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

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 根据路径移动
        if (movePathList == null || movePathList.Count == 0 || currentWaypointIndex == -1)
            return;

        Vector2 currentWaypoint = new Vector2(movePathList[currentWaypointIndex].x, movePathList[currentWaypointIndex].y) + waypointOffset;
        if (isRun)
        {
            Run(currentWaypoint, moveSpeed * 2);
        }
        else
        {
            Walk(currentWaypoint, moveSpeed);
        }

        // 如果接近当前路径点，切换到下一个路径点
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint);
        if (distanceToWaypoint < 0.1f)
        {
            currentWaypointIndex++;
            
            // 如果到达路径的末尾，暂停
            if (currentWaypointIndex >= movePathList.Count)
                currentWaypointIndex = -1;
        }
    }

    private void Walk(Vector3 targetPos, float speed)
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        if (distance < 0.1f)
        {
            _rb.velocity = new Vector2(0, 0);
            _anim.SetBool("IsWalk", false);
            return;
        }

        speed = speed < 0 ? 0 : speed;

        Vector3 dir = targetPos - transform.position;
        dir.Normalize();
        _rb.velocity = dir * speed * Time.deltaTime;

        _anim.SetBool("IsRun", false);
        _anim.SetBool("IsWalk", true);

    }

    private void Run(Vector3 targetPos, float speed)
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        if (distance < 0.1f)
        {
            _rb.velocity = new Vector2(0, 0);
            _anim.SetBool("IsRun", false);
            return;
        }

        speed = speed < 0 ? 0 : speed;

        Vector3 dir = targetPos - transform.position;
        dir.Normalize();
        _rb.velocity = dir * speed * Time.deltaTime;

        _anim.SetBool("IsWalk", false);
        _anim.SetBool("IsRun", true);

    }
}
