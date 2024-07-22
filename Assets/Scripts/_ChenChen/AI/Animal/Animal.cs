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
        /// ����״̬��
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// �����ƶ��Ŀ���
        /// </summary>
        public AnimalMoveController MoveController { get; protected set; }

        /// <summary>
        /// ���ﶯ��
        /// </summary>
        public Animator Animator { get; protected set; }

        /// <summary>
        /// ���ﶨ��
        /// </summary>
        public AnimalDef Def { get; protected set; }

        /// <summary>
        /// ������Ϣ
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
            /* ������������ƶ���� */
            MoveController = GetComponent<AnimalMoveController>();

            /* ����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(this.gameObject, new AnimalState_Idle(this));

            /* ����ͼ��Pawn�ͱ�ǩ */
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