using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using ChenChen_Map;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Seeker))]
    public abstract class MoveController : MonoBehaviour
    {
        protected Seeker _seeker;

        [Header("Debug")]
        // 自身上一个位置
        protected Vector3 lastTransPositon;
        // 停止
        [SerializeField] protected bool isStop = false;
        // 开始移动
        [SerializeField] protected bool isStart = false;
        // 能否移动，isStart 和 canMove同时为 true 才会动
        [SerializeField] protected bool canMove = true;
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

        [Header("移动速度 ")]
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

        [Header("速度倍率")]
        [SerializeField] protected float speedMagnification = 1f;

        [Header("面向右边")]
        public bool IsFaceRight;

        /// <summary>
        /// 当前路径;
        /// </summary>
        protected Path path;

        /// <summary>
        /// 是否到达目标点
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

        public virtual void Init()
        {
            lastTransPositon = transform.position;
            destination = transform.position;
        }

        protected virtual void Start()
        {
            _seeker = GetComponent<Seeker>();
            Init();
        }

        protected virtual void Update()
        {
            // 停止则返回
            if (isStop) return;

            // 如果没有开始则返回
            if (!isStart) return;

            // 如果目标不是一个物体
            if (!targetIsAObject)
            {
                targetDestination = null;
            }

            // 定时刷新
            if (Time.time > lastRepath + repathRate && _seeker.IsDone())
            {
                lastRepath = Time.time;
                ReflashDestination(targetDestination != null ? targetDestination.position : destination, endReachedDistance);
            }

            // 路径为空时返回
            if (path == null) return;

            // 判断是否到达
            reachedEndOfPath = false;
            if (StaticFuction.CompareDistance(transform.position, path.vectorPath[currentWaypoint],endReachedDistance))
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    if (targetIsAObject)
                    {
                        if(!StaticFuction.CompareDistance(transform.position, targetDestination.position, endReachedDistance))
                        {
                            return;
                        }
                    }
                    reachedEndOfPath = true;
                    OnTargetReached();
                }
            }

            // 移动，通过直接改变Transform的位置
            var speedFactor = !reachedEndOfPath ? Speed : 0;
            speedFactor = !reachDestination ? speedFactor : 0;
            speedFactor = canMove ? speedFactor : 0;
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            transform.position += dir * speedFactor * Time.deltaTime;

            // 其他逻辑
            Filp();       
        }

        protected virtual void OnTargetReached()
        {
            reachDestination = true;
            isStart = false;
        }

        #region Move
        /// <summary>
        /// 暂停移动
        /// </summary>
        public void StopMove()
        {
            isStop = false;
        }

        /// <summary>
        /// 暂停移动一段时间
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
        /// 恢复移动
        /// </summary>
        public void RecoverMove()
        {
            StopCoroutine(StopMoveCoroutine);
            isStop = true;
        }
        #endregion

        #region Path

        /// <summary>
        /// 前往到目标点
        /// </summary>
        /// <param name="target">目标点</param>
        /// <returns></returns>
        protected bool StartPath(Vector3 target, float endReachedDistance = 0.2f)
        {
            targetDestination = null;
            targetIsAObject = false;
            if(!ReflashDestination(target, endReachedDistance))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
        /// <param name="target">目标GameObject</param>
        /// <returns></returns>
        protected bool StartPath(GameObject target, float endReachedDistance = 0.2f)
        {
            // 设置要追踪的目标
            targetDestination = target.transform;
            targetIsAObject = true;
            if (!ReflashDestination(targetDestination.position, endReachedDistance))
            {
                targetDestination = null;
                targetIsAObject = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刷新目标节点
        /// </summary>
        /// <returns></returns>
        private bool ReflashDestination(Vector3 destination, float endReachedDistance)
        {
            if (destination.x <= 0 || destination.x >= MapManager.Instance.CurMapWidth) return false;
            if (destination.y <= 0 || destination.y >= MapManager.Instance.CurMapHeight) return false;
            if (!_seeker.IsDone())
            {
                return false;
            }
            if(AstarPath.active == null)
            {
                return false;
            }
            // 转换目标点为格子中心
            Vector3Int toInt = StaticFuction.VectorTransToInt(destination);
            Vector3 to = new Vector3(toInt.x + 0.5f, toInt.y + 0.5f);
            // 新建路径
            ABPath newPath = ABPath.Construct(transform.position, to);
            // 开始计算路径
            _seeker.StartPath(newPath, callback: (p) =>
            {
                p.Claim(this);
                if (!p.error)
                {
                    // 判断路径是否可达
                    Vector3 end = p.vectorPath[p.vectorPath.Count - 1];
                    if (Vector2.Distance(end, to) < endReachedDistance)
                    {
                        this.destination = to;
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
                if (!IsFaceRight)
                {
                    // 向左边走，正面
                    if (lastTransPositon.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    // 向右边走，反面
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

        public void FilpIt(float x)
        {
            if(transform.position.x < x)
            {
                FilpRight();
            }
            else
            {
                FilpLeft();
            }
        }

        #endregion
    }
}