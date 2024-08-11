using ChenChen_Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class PawnMoveController : MoveController
    {
        private Pawn _pawn;
        private Collider2D _collider;
        private LineRenderer _lineRenderer;

        [Header("紧迫程度")]
        [SerializeField] protected Urgency curUrgency = Urgency.Normal;

        private Vector3 lastPosition;
        private float accumulatedDistance;
        private float timeElapsed;

        protected override void Start()
        {
            base.Start();
            _collider = GetComponent<Collider2D>();
            _pawn = GetComponent<Pawn>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        protected override void Update()
        {
            base.Update();
            if (_pawn.Info.IsDead) return;
            // 选中情况下，按下R征兆
            if (_pawn.Info.IsSelect && Input.GetKeyDown(KeyCode.R))
            {
                _pawn.StateMachine.TryChangeState(new PawnJob_Draft(_pawn, !_pawn.Info.IsDrafted));
            }
            // 征兆情况下, 鼠标右击
            if (_pawn.Info.IsSelect && _pawn.Info.IsDrafted && Input.GetMouseButtonDown(1))
            {
                Vector2 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D coll = Physics2D.OverlapPoint(mouseInput);

                // 移动到鼠标点
                _pawn.StateMachine.TryChangeState(
                    new PawnJob_Move(_pawn, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }
            // 选中情况下会绘制路径
            DrawPathUpdate();
            // 更新动画
            UpdateMoveAnimation();
        }

        private void UpdateMoveAnimation()
        {
            float distance = (transform.position - lastPosition).magnitude;
            accumulatedDistance += distance;
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= 0.1f) // 每0.1秒更新一次
            {
                float speed = accumulatedDistance / timeElapsed;
                _pawn.Anim.SetFloat("Speed", speed);
                accumulatedDistance = 0f;
                timeElapsed = 0f;
            }

            lastPosition = transform.position;
        }

        /// <summary>
        /// 前往到目标点
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 targetPos, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
            switch (urgency)
            {
                case Urgency.Wander:
                    Speed = 1;
                    break;
                case Urgency.Normal:
                    Speed = 2;
                    break;
                case Urgency.Urge:
                    Speed = 3;
                    break;
                default:
                    Speed = 2;
                    break;
            }
            curUrgency = urgency;
            return StartPath(targetPos, endReachedDistance);
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public bool GoToHere(GameObject targetObj, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
            switch (urgency)
            {
                case Urgency.Wander:
                    Speed = 1;
                    break;
                case Urgency.Normal:
                    Speed = 2;
                    break;
                case Urgency.Urge:
                    Speed = 3;
                    break;
                default:
                    Speed = 2;
                    break;
            }
            curUrgency = urgency;
            return StartPath(targetObj, endReachedDistance);
        }

        #region DrawPath

        protected void DrawPathUpdate()
        {
            if (_pawn.Faction != GameManager.PLAYER_FACTION) return;
            List<Vector3> pathDraw = new List<Vector3>();
            if (path != null && _pawn.Info.IsSelect)
            {
                for (int i = currentWaypoint; i < path.vectorPath.Count; i++)
                {
                    pathDraw.Add(path.vectorPath[i]);
                }
            }
            DrawPath(pathDraw);
        }

        protected void DrawPath(List<Vector3> points)
        {
            // 设置线的宽度等属性
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;

            // 设置路径点
            Vector3[] draw = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                draw[i] = new Vector3(points[i].x, points[i].y);
            }

            // 绘制路径
            _lineRenderer.positionCount = draw.Length;
            _lineRenderer.SetPositions(draw);
        }

        #endregion
    }
}