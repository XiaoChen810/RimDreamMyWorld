using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ��ͼ����;

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

    // �ƶ��ٶ�
    public float moveSpeed;
    // �Ƿ��ڱ���
    public bool isRun;
    // �Ƿ��Ѿ�����Ŀ���
    public bool isReach;
    // ��ǰ���ڵĵ�ͼ
    public string currentMapName;

    // ��Ҫ�ƶ���Ŀ���
    private Vector2 MoveTargetPos;
    // ��һ����Ҫ�ƶ���Ŀ���
    private Vector2 LastMoveTargetPos;
    // ·����ƫ��ֵ
    private Vector2 waypointOffset = new Vector2(0.5f, 0.5f);
    // ·���б�
    private List<Vector2> movePathList = null;
    // ��ǰ·�����������
    private int currentWaypointIndex = -1;

    // ������һ��λ��
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
        Debug.Log("�ı�Ŀ���");
        MoveTargetPos = target;
        ResetPath();
    }

    #endregion



    private void MovePathLogic()
    {
        // ѡ�������,���Ŀ���ı䣬���»�ȡ·��
        if (logicConrtroller.IsSelect && LastMoveTargetPos != MoveTargetPos)
        {
            ResetPath();
        }

        // ѡ�������, ����һ����ı�Ŀ���
        if (logicConrtroller.IsSelect && Input.GetMouseButtonDown(1))
        {
            MoveTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        isReach = currentWaypointIndex == -1;
    }

    private void ResetPath()
    {
        // ��ȡ·��
        movePathList = MapManager.Instance.GetPath(transform.position, MoveTargetPos, currentMapName);

        // ����·��������
        if (movePathList != null && movePathList.Count > 0) currentWaypointIndex = 0;
        LastMoveTargetPos = MoveTargetPos;
    }

    private void DrawLineForCurrentPath()
    {
        // ���ݵ�ǰ��·������·��
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
        // �����ߵĿ�ȵ�����
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // ����·����
        Vector3[] path = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            path[i] = new Vector3(points[i].x + waypointOffset.x, points[i].y + waypointOffset.y);
        }

        // ����·��
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);
    }

    private void Filp()
    {
        if(lastTransPositon != transform.position)
        {
            // ���ұ��ߣ�����
            if(lastTransPositon.x < transform.position.x)
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

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // ����·���ƶ�
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

        // ����ӽ���ǰ·���㣬�л�����һ��·����
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint);
        if (distanceToWaypoint < 0.1f)
        {
            currentWaypointIndex++;
            
            // �������·����ĩβ����ͣ
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
