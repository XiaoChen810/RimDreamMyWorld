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

        public AnimalDef Def;
        public AnimalInfo Info;

        [Header("当前状态")]
        public List<string> CurrentStateList = new List<string>();

        protected virtual void Start()
        {
            /* 添加这个人物的移动组件 */
            MoveController = GetComponent<AnimalMoveController>();

            /* 添加这个人物的动画组件 */
            Animator = GetComponent<Animator>();

            /* 配置状态机 */
            StateMachine = new StateMachine(this.gameObject, new AnimalState_Idle(this));

            /* 设置图层Pawn和标签 */
            gameObject.layer = 9;
            gameObject.tag = "Animal";
        }

        public void Init(AnimalDef def,AnimalInfo info)
        {
            Def = def;
            Info = info;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            任务列表Debug();
#endif
            StateMachine.Update();
        }

        protected void 任务列表Debug()
        {
            CurrentStateList.Clear();
            CurrentStateList.Add("正在：" + StateMachine.CurState?.ToString());
            CurrentStateList.Add("下一个：" + StateMachine.NextState?.ToString());
            foreach (var task in StateMachine.StateQueue)
            {
                CurrentStateList.Add("准备" + task.ToString());
            }
        }
    }
}