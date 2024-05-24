using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PawnMoveController))]
    public abstract class Pawn : MonoBehaviour, IDetailView
    {
        /// <summary>
        /// �����״̬��
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// �����ƶ��Ŀ���
        /// </summary>
        public PawnMoveController MoveController { get; protected set; }

        /// <summary>
        /// ���ﶯ��״̬����
        /// </summary>
        public Animator Animator { get; protected set; }

        [Header("��ǰ����")]
        public GameObject CurJobTarget;
        public List<string> CurrentStateList = new List<string>();

        [Header("�����߼�����")]
        public float WorkRange = 1;
        public float AttackRange = 1;
        public float AttackSpeed = 0.76f;
        public float AttackSpeedWait = 0.5f;

        [Header("���ﶨ��")]
        [SerializeField] private PawnKindDef _pawnKindDef;
        public PawnKindDef Def
        {
            get 
            { 
                if(_pawnKindDef == null)
                {
                    _pawnKindDef = new PawnKindDef();
                }
                return _pawnKindDef; 
            }
            set 
            { 
                _pawnKindDef = value;
            }
        }

        [Header("������������")]
        [SerializeField] private PawnAttribute _pawnAttribute;
        public PawnAttribute Attribute
        {
            get
            {
                if (_pawnAttribute == null)
                {
                    _pawnAttribute = new PawnAttribute();
                }
                return _pawnAttribute;
            }
            set
            {
                _pawnAttribute = value;
            }
        }
        [Header("����״̬��Ϣ")]
        [SerializeField] private PawnInfo _pawnInfo;
        public PawnInfo Info
        {
            get
            {
                if (_pawnInfo == null)
                {
                    _pawnInfo = new PawnInfo();
                }
                return _pawnInfo;
            }
            set
            {
                _pawnInfo = value;
            }
        }

        // ϸ����ͼ
        protected DetailView _detailView;
        public DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Pawn>();
                    }
                }
                return _detailView;
            }
        }

        #region Job

        protected abstract void TryToGetJob();

        /// <summary>
        /// Go to work for job
        /// </summary>
        /// <param name="job"></param>
        public void JobToDo(GameObject job)
        {
            _pawnKindDef.CanGetJob = false;
            _pawnInfo.IsOnWork = false;
            CurJobTarget = job;
        }

        /// <summary>
        /// IsOnWork => ture
        /// </summary>
        public void JobDoing()
        {
            _pawnInfo.IsOnWork = true;
        }

        /// <summary>
        /// Complete Job, CanGetJob => true, IsOnWork => false
        /// </summary>
        public void JobDone()
        {
            _pawnKindDef.CanGetJob = true;
            _pawnInfo.IsOnWork = false;
            CurJobTarget = null;
        }

        /// <summary>
        /// CanGetJob => true
        /// </summary>
        public void JobCanGet()
        {
            _pawnKindDef.CanGetJob = true;
        }

        /// <summary>
        /// CanGetJob => false;
        /// </summary>
        public void JobCannotGet()
        {
            _pawnKindDef.CanGetJob = false;
        }

        #endregion

        #region Battle

        //private Coroutine AttackCoroutine;
        //private bool isTriggerAttack = false;

        //public bool TryToEnterBattle(Pawn battleTarget)
        //{
        //    if (isTriggerAttack) return false;
        //    _pawnInfo.IsOnBattle = true;
        //    CurJobTarget = battleTarget.gameObject;

        //    // ����λ��
        //    Vector3 me = transform.position;
        //    Vector3 him = battleTarget.gameObject.transform.position;
        //    if (me.x < him.x)
        //    {
        //        MoveController.FilpRight();
        //    }
        //    else
        //    {
        //        MoveController.FilpLeft();
        //    }

        //    if (AttackCoroutine != null) StopCoroutine(AttackAnimCo());
        //    AttackCoroutine = StartCoroutine(AttackAnimCo());
        //    return true;
        //}

        //public bool TryToEndBattle()
        //{
        //    _pawnInfo.IsOnBattle = false;
        //    CurJobTarget = null;
        //    return true;
        //}

        //IEnumerator AttackAnimCo()
        //{
        //    Debug.Log("Enter");
        //    while (_pawnInfo.IsOnBattle)
        //    {
        //        yield return null;
        //        if (!isTriggerAttack)
        //        {
        //            isTriggerAttack = true;
        //            Animator.SetTrigger("IsAttack");
        //        }
        //        yield return new WaitForSeconds(0.76f + AttackSpeedWait);
        //        isTriggerAttack = false;
        //    }
        //}

        public void GetDamage(float damage)
        {
            _pawnInfo.HP.CurValue -= (int)damage;
            if (_pawnInfo.HP.IsSpace)
            {
                Info.IsDead = true;
                gameObject.SetActive(false);
                return;
            }
            Animator.SetTrigger("IsHurted");
        }

        #endregion

        protected virtual void Start()
        {
            /* ������������ƶ���� */
            MoveController = GetComponent<PawnMoveController>();

            /* ����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(this.gameObject, new PawnJob_Idle(this));

            /* ����ͼ��Pawn�ͱ�ǩ */
            gameObject.layer = 7;
            gameObject.tag = "Pawn";
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            �����б�Debug();
#endif
            if (Def.StopUpdate) return;
            StateMachine.Update();
            if (!Info.IsOnWork && Def.CanGetJob) TryToGetJob();
        }

        protected void �����б�Debug()
        {
            CurrentStateList.Clear();
            if (Def.StopUpdate)
            {
                CurrentStateList.Add("StopUpdate");
                return;
            }
            CurrentStateList.Add("���ڣ�" + StateMachine.CurState?.ToString());
            CurrentStateList.Add("��һ����" + StateMachine.NextState?.ToString());
            foreach (var task in StateMachine.StateQueue)
            {
                CurrentStateList.Add("׼��" + task.ToString());
            }
        }
    }
}