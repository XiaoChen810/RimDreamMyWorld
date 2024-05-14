using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimalMoveController))]
    public abstract class Animal : MonoBehaviour
    {
        /// <summary>
        /// 动物状态机
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// 动物移动的控制
        /// </summary>
        public AnimalMoveController MoveController { get; protected set; }

        /// <summary>
        /// 动物动画
        /// </summary>
        public Animator Animator { get; protected set; }

        protected virtual void Start()
        {
            /* 添加这个人物的移动组件 */
            MoveController = GetComponent<AnimalMoveController>();

            /* 添加这个人物的动画组件 */
            Animator = GetComponent<Animator>();

            /* 配置状态机 */
            StateMachine = new StateMachine(this.gameObject);

            /* 设置图层Pawn和标签 */
            gameObject.layer = 9;
            gameObject.tag = "Animal";
        }
    }
}