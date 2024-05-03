using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class MoveController : MonoBehaviour
    {
        protected Rigidbody2D _rb;
        protected Seeker _seeker;

        [Header("Debug")]
        // ������һ��λ��
        protected Vector3 lastTransPositon;
        // ��ʼѰ·
        [SerializeField] protected bool isStart = false;
        // �ܷ��ƶ���isStart �� canMoveͬʱΪ true �Żᶯ
        [SerializeField] protected bool canMove = true;
        // �ƶ��ٶ�
        [SerializeField] protected float speed = 2f;
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
            var speedFactor = !reachedEndOfPath ? speed : 0;
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
        protected bool StartPath(Vector3 destination, float speed, float endReachedDistance = 0.2f)
        {
            targetDestination = null;
            targetIsAObject = false;
            return ReflashDestination(destination, speed, endReachedDistance);
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool StartPath(GameObject destination, float speed, float endReachedDistance = 0.2f)
        {
            // ����Ҫ׷�ٵ�Ŀ��
            targetDestination = destination.transform;
            targetIsAObject = true;
            return ReflashDestination(targetDestination.position, speed, endReachedDistance);
        }

        private bool ReflashDestination(Vector3 destination, float speed, float endReachedDistance)
        {
            //Debug.Log("Reflash Destination");
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

            return true;
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
    }
}