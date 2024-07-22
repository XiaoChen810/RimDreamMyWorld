using ChenChen_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimalMoveController))]
    public abstract class Animal : MonoBehaviour, IDetailView
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

        /// <summary>
        /// 动物定义
        /// </summary>
        public AnimalDef Def { get; protected set; }

        /// <summary>
        /// 动物信息
        /// </summary>
        [SerializeField] protected AnimalInfo _info;
        public AnimalInfo Info
        {
            get { return _info; }
            protected set { _info = value; }
        }

        protected DetailView _detailView;
        public DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Animal>();
                    }
                }
                return _detailView;
            }
        }

        public void Init(AnimalDef def, AnimalInfo info)
        {
            Def = def;
            Info = (AnimalInfo)info.Clone();
        }

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

        protected virtual void Update()
        {
            StateMachine.Update();
        }

        public void FlagTrade()
        {
            Info.IsFlagTrade = true;
        }

        public void CancelTrade()
        {
            Info.IsFlagTrade = false;
        }

        public void Trade()
        {
            Info.IsOnTrade = true;
        }

        public void StopTrade()
        {
            Info.IsOnTrade = false;
        }

        public void CompleteTrade()
        {
            Info.IsTrade = true;
            Info.IsFlagTrade= false;
            Info.IsOnTrade = false;
        }

        public bool WaitToTrade => Info.IsFlagTrade && !Info.IsOnTrade;
    }
}