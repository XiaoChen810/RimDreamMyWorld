using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class PawnMoveController : MoveController
    {
        private Pawn _pawn;
        private Collider2D _collider;
        private LineRenderer _lineRenderer;

        // ���ȳ̶�
        [SerializeField] protected Urgency curUrgency = Urgency.Normal;

        private bool _isSwimming = false;

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
            // ѡ������£�����R����
            if (_pawn.IsSelect && Input.GetKeyDown(KeyCode.R))
            {
                _pawn.StateMachine.TryChangeState(new PawnJob_Draft(_pawn, !_pawn.IsDrafted));
            }
            // ���������, ����һ����ƶ�������
            if (_pawn.IsSelect && _pawn.IsDrafted && Input.GetMouseButtonDown(1))
            {
                _pawn.StateMachine.TryChangeState(
                    new PawnJob_Move(_pawn, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }
            // ѡ������»����·��
            DrawPathUpdate();
            // ���¶���
            UpdateMoveAnimation();
        }

        private void UpdateMoveAnimation()
        {
            if (!isStart || !canMove)
            {
                speed = 0;
                _pawn.Animator.SetBool("IsWalk", false);
                _pawn.Animator.SetBool("IsRun", false);
                _pawn.Animator.SetBool("IsSwimming", false);
                return;
            }
            if (!_isSwimming)
            {
                if (speed > 0 && speed <= 1)
                {
                    SetAnimation("IsWalk", true);
                }
                if (speed > 1)
                {
                    SetAnimation("IsRun", true);
                }
            }
            else
            {
                SetAnimation("IsSwimming", true);
            }
            void SetAnimation(string name,bool value)
            {
                if(value == _pawn.Animator.GetBool(name))
                {
                    return;
                }
                _pawn.Animator.SetBool("IsWalk", false);
                _pawn.Animator.SetBool("IsRun", false);
                _pawn.Animator.SetBool("IsSwimming", false);
                _pawn.Animator.SetBool(name, value);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision != null && collision.CompareTag("Water"))
            {
                _isSwimming = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision != null && collision.CompareTag("Water"))
            {
                _isSwimming = false;
            }
        }
        //private void OnCollisionStay2D(Collision2D collision)
        //{
        //    if (collision != null && collision.collider.CompareTag("Water"))
        //    {
        //        _isSwimming = true;
        //    }
        //}
        //private void OnCollisionExit2D(Collision2D collision)
        //{
        //    if (collision != null && collision.collider.CompareTag("Water"))
        //    {
        //        _isSwimming = false;
        //    }
        //}

        /// <summary>
        /// ǰ����Ŀ���
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
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
                    speed = 2;
                    break;
            }
            return StartPath(target, speed, endReachedDistance);
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public void GoToHere(GameObject target, Urgency urgency = Urgency.Normal, float endReachedDistance = 0.2f)
        {
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
                    speed = 2;
                    break;
            }
            StartPath(target, speed, endReachedDistance);
        }

        #region DrawPath

        protected void DrawPathUpdate()
        {
            List<Vector3> pathDraw = new List<Vector3>();
            if (path != null && _pawn.IsSelect)
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
            // �����ߵĿ�ȵ�����
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;

            // ����·����
            Vector3[] draw = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                draw[i] = new Vector3(points[i].x, points[i].y);
            }

            // ����·��
            _lineRenderer.positionCount = draw.Length;
            _lineRenderer.SetPositions(draw);
        }

        #endregion
    }
}