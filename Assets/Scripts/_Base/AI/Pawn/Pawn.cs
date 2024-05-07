using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    public abstract class Pawn : MonoBehaviour
    {
        /// <summary>
        /// �����״̬��
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// �����ƶ��Ŀ���
        /// </summary>
        public PawnMoveController MoveControl { get; protected set; }

        /// <summary>
        /// ���ﶯ��״̬����
        /// </summary>
        public Animator Animator { get; protected set; }

        [Header("��ǰ״̬")]
        public List<string> CurrentStateList = new List<string>();

        [Header("�����߼�����")]
        public float WorkRange = 1;
        public float AttackRange = 1;
        public float AttackSpeed = 0.76f;
        public float AttackSpeedWait = 0.5f;

        [Header("����״̬����")]
        public int Hp = 100;

        [Header("������������")]
        public PawnAttribute Attribute;

        [Header("������Ϣ")]
        public string PawnName;
        public string FactionName;
        public string Description;
        public string PrefabPath;

        [Header("����״̬ Can")]
        public bool StopUpdate = false;
        public bool CanSelect = true;
        public bool CanGetJob = true;
        public bool CanBattle = true;
        public bool CanDrafted = true;

        [Header("����״̬��Ϣ")]
        [SerializeField] private PawnInfo _pawnInfo = new();
        public PawnInfo PawnInfo
        {
            get
            {
                return _pawnInfo;
            }
            set
            {
                _pawnInfo = value;
            }
        }
        public bool IsDead
        {
            get
            {
                return _pawnInfo.IsDead;
            }
            set
            {
                _pawnInfo.IsDead = value;
            }
        }
        public bool IsSelect
        {
            get
            {
                return _pawnInfo.IsSelect;
            }
            set
            {
                if (_pawnInfo.IsSelect == value) return;
                if (!CanSelect) return;
                if (value)
                {
                    // ѡ�������
                    Indicator_DOFadeOne();
                }
                else
                {
                    // ѡ�������, ���������
                    Indicator_DOColorWhite();
                    Indicator_DOFadeZero();
                    _pawnInfo.IsDrafted = false;
                }
                _pawnInfo.IsSelect = value;
            }
        }
        public bool IsOnWork
        {
            get
            {
                return _pawnInfo.IsOnWork;
            }
            set
            {
                _pawnInfo.IsOnWork = value;
            }
        }
        public bool IsOnBattle
        {
            get
            {
                return _pawnInfo.IsOnBattle;
            }
            set
            {
                _pawnInfo.IsOnBattle = value;
            }
        }
        public bool IsDrafted
        {
            get
            {
                return _pawnInfo.IsDrafted;
            }
            set
            {
                if (_pawnInfo.IsDrafted == value) return;
                if (!CanDrafted) return;

                if (value)
                {
                    // ѡ�����
                    Indicator_DOColorRed();
                }
                else
                {
                    // ѡ���ԭ����ȡ��ѡ��
                    Indicator_DOColorWhite();
                    Indicator_DOFadeZero();
                    _pawnInfo.IsSelect = false;
                }
                _pawnInfo.IsDrafted = value;
            }
        }

        [Header("����ָ��")]
        public GameObject CurJobTarget;

        #region Job

        protected abstract void TryToGetJob();

        /// <summary>
        /// Go to work for job
        /// </summary>
        /// <param name="job"></param>
        public void JobToDo(GameObject job)
        {
            CanGetJob = false;
            IsOnWork = false;
            CurJobTarget = job;
        }

        /// <summary>
        /// IsOnWork => ture
        /// </summary>
        public void JobDoing()
        {
            IsOnWork = true;
        }

        /// <summary>
        /// Complete Job, CanGetJob => true, IsOnWork => false
        /// </summary>
        public void JobDone()
        {
            CanGetJob = true;
            IsOnWork = false;
            CurJobTarget = null;
        }

        /// <summary>
        /// CanGetJob => true
        /// </summary>
        public void JobCanGet()
        {
            CanGetJob = true;
        }

        /// <summary>
        /// CanGetJob => false;
        /// </summary>
        public void JobCannotGet()
        {
            CanGetJob = false;
        }

        #endregion

        #region Battle

        private Coroutine AttackCoroutine;
        private bool isTriggerAttack = false;

        public bool TryToEnterBattle(Pawn battleTarget)
        {
            if (isTriggerAttack) return false;
            IsOnBattle = true;
            CurJobTarget = battleTarget.gameObject;

            // ����λ��
            Vector3 me = transform.position;
            Vector3 him = battleTarget.gameObject.transform.position;
            if (me.x < him.x)
            {
                MoveControl.FilpRight();
            }
            else
            {
                MoveControl.FilpLeft();
            }

            if (AttackCoroutine != null) StopCoroutine(AttackAnimCo());
            AttackCoroutine = StartCoroutine(AttackAnimCo());
            return true;
        }

        public bool TryToEndBattle()
        {
            IsOnBattle = false;
            CurJobTarget = null;
            return true;
        }

        IEnumerator AttackAnimCo()
        {
            Debug.Log("Enter");
            while (IsOnBattle)
            {
                yield return null;
                if (!isTriggerAttack)
                {
                    isTriggerAttack = true;
                    Animator.SetTrigger("IsAttack");
                }
                yield return new WaitForSeconds(0.76f + AttackSpeedWait);
                isTriggerAttack = false;
            }
        }

        public void GetDamage(float damage)
        {
            Hp -= (int)damage;
            Hp = Hp <= 0 ? 0 : Hp;
            if (Hp <= 0)
            {
                IsDead = true;
                gameObject.SetActive(false);
                return;
            }
            Animator.SetTrigger("IsHurted");
        }

        #endregion

        #region Indicator

        // ��ȡ�����ָʾ��������������򴴽�
        private bool TryGetIndicator(out GameObject indicator)
        {
            indicator = null;
            if (transform.Find("SelectionBox"))
            {
                indicator = transform.Find("SelectionBox").gameObject;
                indicator.SetActive(true);
            }
            if (indicator == null)
            {
                indicator = Instantiate(Resources.Load<GameObject>("Views/SelectionBox"), gameObject.transform);
                indicator.name = "SelectionBox";
                indicator.SetActive(true);
            }
            if (indicator == null)
            {
                Debug.LogError("Failed to find or create the SelectionBox GameObject.");
                return false;
            }
            return true;
        }

        // Animation
        protected void Indicator_DOFadeOne()
        {
            if (TryGetIndicator(out GameObject indicator))
            {
                SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
                sr.DOFade(1, 1);
            }
        }
        protected void Indicator_DOFadeZero()
        {
            if (TryGetIndicator(out GameObject indicator))
            {
                SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
                sr.DOFade(0, 1);
            }
        }
        protected void Indicator_DOColorRed()
        {
            if (TryGetIndicator(out GameObject indicator))
            {
                SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
                sr.DOColor(Color.red, 1);
            }
        }
        protected void Indicator_DOColorWhite()
        {
            if (TryGetIndicator(out GameObject indicator))
            {
                SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
                sr.DOColor(Color.white, 1);
            }
        }

        #endregion

        protected virtual void Start()
        {
            /* ������������ƶ���� */
            MoveControl = GetComponent<PawnMoveController>();

            /* ����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(new PawnJob_Idle(this), this);

            /* ����ͼ��Pawn�ͱ�ǩ */
            gameObject.layer = 7;
            gameObject.tag = "Pawn";
        }

        protected virtual void Update()
        {
            if (StopUpdate) return;
            StateMachine.Update();
            TryToGetJob();

#if UNITY_EDITOR
            �����б�Debug();
#endif
        }

        protected void �����б�Debug()
        {
            CurrentStateList.Clear();
            CurrentStateList.Add("���ڣ�" + StateMachine.CurState?.ToString());
            CurrentStateList.Add("��һ����" + StateMachine.NextState?.ToString());
            foreach (var task in StateMachine.StateQueue)
            {
                CurrentStateList.Add("׼��" + task.ToString());
            }
        }
    }
}