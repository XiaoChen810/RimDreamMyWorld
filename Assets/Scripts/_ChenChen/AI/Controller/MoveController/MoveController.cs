using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using ChenChen_Map;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Seeker))]
    public abstract class MoveController : MonoBehaviour
    {
        protected Rigidbody2D _rb;
        protected Seeker _seeker;

        [Header("Debug")]
        // ������һ��λ��
        protected Vector3 lastTransPositon;
        // ֹͣ
        [SerializeField] protected bool isStop = false;
        // ��ʼ�ƶ�
        [SerializeField] protected bool isStart = false;
        // �ܷ��ƶ���isStart �� canMoveͬʱΪ true �Żᶯ
        [SerializeField] protected bool canMove = true;
        // �ƶ�����µľ��룬Ҳ���жϵ���Ŀ���ľ���
        [SerializeField] protected float endReachedDistance = 0.2f;
        // ������̬ˢ��ʱ��ÿ��ˢ�µļ��ʱ��
        [SerializeField] protected float repathRate = 0.5f;
        // Ŀ���
        [SerializeField] protected Vector3 destination;
        // Ŀ��Transform
        [SerializeField] protected Transform targetDestination;
        // Ŀ���Ƿ��Ǹ�����
        [SerializeField] protected bool targetIsAObject = false;
        // ����·��ĩβ
        [SerializeField] protected bool reachedEndOfPath = true;
        // ����Ŀ���
        [SerializeField] protected bool reachDestination = true;
        // ��ǰ��·�����ĸ���
        [SerializeField] protected int currentWaypoint = 0;

        [Header("�ƶ��ٶ� ")]
        [SerializeField] private float _speed = 2f;
        protected float Speed
        {
            get 
            {
                float result = _speed * speedMagnification;
                return result; 
            }
            set
            {
                if (value <= 0)
                {
                    Debug.Log("Speed seted 0");
                }
                _speed = value;
            }
        }

        [Header("�ٶȱ���")]
        [SerializeField] protected float speedMagnification = 1f;

        [Header("�����ұ�")]
        public bool IsFaceRight;

        /// <summary>
        /// ��ǰ·��;
        /// </summary>
        protected Path path;

        /// <summary>
        /// �Ƿ񵽴�Ŀ���
        /// </summary>
        public bool ReachDestination
        {
            get
            {
                return reachDestination;
            }
        }

        private float lastRepath = float.NegativeInfinity;

        private Coroutine StopMoveCoroutine;

        /// <summary>
        /// ��ͣ�ƶ�
        /// </summary>
        public void StopMove()
        {
            isStop = false;
        }

        /// <summary>
        /// ��ͣ�ƶ�һ��ʱ��
        /// </summary>
        /// <param name="time"></param>
        public void StopMove(float time)
        {
            StopCoroutine(StopMoveCoroutine);
            StopMoveCoroutine = StartCoroutine(StopMoveCo(time));
        }

        IEnumerator StopMoveCo(float time)
        {
            isStop = false;
            yield return new WaitForSeconds(time);
            isStop = true;
        }

        /// <summary>
        /// �ָ��ƶ�
        /// </summary>
        public void RecoverMove()
        {
            StopCoroutine(StopMoveCoroutine);
            isStop = true;
        }

        protected virtual void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _seeker = GetComponent<Seeker>();

            // init
            lastTransPositon = transform.position;
            destination = transform.position;
        }

        protected virtual void Update()
        {
            if (isStop) return;

            if (!isStart) return;

            if (!targetIsAObject)
            {
                targetDestination = null;
            }

            // ��ʱˢ��
            if (Time.time > lastRepath + repathRate && _seeker.IsDone())
            {
                lastRepath = Time.time;
                ReflashDestination(targetDestination != null ? targetDestination.position : destination, endReachedDistance);
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
                    if (targetIsAObject)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, targetDestination.position);
                        if (distanceToTarget > endReachedDistance)
                        {
                            return;
                        }
                    }
                    reachedEndOfPath = true;
                    OnTargetReached();
                }
            }

            // �ƶ�
            var speedFactor = !reachedEndOfPath ? Speed : 0;
            speedFactor = !reachDestination ? speedFactor : 0;
            speedFactor = canMove ? speedFactor : 0;
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            transform.position += dir * speedFactor * Time.deltaTime;

            // �����߼�
            Filp();       
        }

        protected virtual void OnTargetReached()
        {
            reachDestination = true;
            isStart = false;
        }

        #region Path

        /// <summary>
        /// ǰ����Ŀ���
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool StartPath(Vector3 destination, float endReachedDistance = 0.2f)
        {
            targetDestination = null;
            targetIsAObject = false;
            if(!ReflashDestination(destination, endReachedDistance))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool StartPath(GameObject destination, float endReachedDistance = 0.2f)
        {
            // ����Ҫ׷�ٵ�Ŀ��
            targetDestination = destination.transform;
            targetIsAObject = true;
            if (!ReflashDestination(targetDestination.position, endReachedDistance))
            {
                targetDestination = null;
                targetIsAObject = false;
                return false;
            }
            return true;
        }

        private bool ReflashDestination(Vector3 destination, float endReachedDistance)
        {
            if (destination.x <= 0 || destination.x >= MapManager.Instance.CurMapWidth) return false;
            if (destination.y <= 0 || destination.y >= MapManager.Instance.CurMapHeight) return false;
            if (!_seeker.IsDone())
            {
                return false;
            }
            // �½�·��
            ABPath newPath = ABPath.Construct(transform.position, destination);
            // ��ʼ����·��
            _seeker.StartPath(newPath, callback: (p) =>
            {
                p.Claim(this);
                if (!p.error)
                {
                    // �ж�·���Ƿ�ɴ�
                    Vector3 end = p.vectorPath[p.vectorPath.Count - 1];
                    if (Vector2.Distance(end, destination) < endReachedDistance)
                    {
                        this.destination = destination;
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

            return true;
        }

        #endregion

        #region Flip

        protected void Filp()
        {
            if (lastTransPositon != transform.position)
            {
                if (IsFaceRight)
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
                if (!IsFaceRight)
                {
                    // ������ߣ�����
                    if (lastTransPositon.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    // ���ұ��ߣ�����
                    if (lastTransPositon.x > transform.position.x)
                    {
                        transform.localScale = Vector3.one;
                    }
                    lastTransPositon = transform.position;
                }
            }
        }

        public void FilpLeft()
        {
            if (IsFaceRight)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            lastTransPositon = transform.position;

        }

        public void FilpRight()
        {
            if (IsFaceRight)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            lastTransPositon = transform.position;
        }

        #endregion
    }
}