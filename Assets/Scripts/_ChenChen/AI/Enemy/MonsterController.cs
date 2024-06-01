using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    public class MonsterController : MoveController
    {
        protected Monster _monster;

        /// <summary>
        /// 前往到目标点
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public bool GoToHere(Vector3 targetPos, float endReachedDistance = 0.2f)
        {
            return StartPath(targetPos, endReachedDistance);
        }

        protected override void Start()
        {
            base.Start();
            _monster = GetComponent<Monster>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateMoveAnimation();
        }

        private void UpdateMoveAnimation()
        {
            if (!isStart || !canMove)
            {
                _monster.Animator.SetBool("IsWalk", false);
                _monster.Animator.SetBool("IsRun", false);
                return;
            }
            if (Speed > 0 && Speed <= 1)
            {
                SetAnimation("IsWalk", true);
            }
            if (Speed > 1)
            {
                SetAnimation("IsRun", true);
            }

            void SetAnimation(string name, bool value)
            {
                if (value == _monster.Animator.GetBool(name))
                {
                    return;
                }
                _monster.Animator.SetBool("IsWalk", false);
                _monster.Animator.SetBool("IsRun", false);
                _monster.Animator.SetBool(name, value);
            }
        }
    }
}