using ChenChen_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(MoveController))]
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

        public EmotionController EmotionController;

        [Header("��ǰ����")]
        public GameObject CurJobTarget;
        public List<string> CurrentStateList = new List<string>();

        [Header("�����߼�����")]
        public float WorkRange = 1;
        public float AttackRange = 1;
        public float AttackSpeedWait = 2.5f;
        public float AttackDamage = 0;

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

        /// <summary>
        /// ��ͣ��ǰ��������ֹͣ���ܹ���һ��ʱ��
        /// </summary>
        /// <param name="wait"></param>
        public void StopJob(float wait)
        {
            StateMachine.TryChangeState(null);
            StartCoroutine(StopJobCo(wait));

            IEnumerator StopJobCo(float wait)
            {
                _pawnKindDef.CanGetJob = false;
                yield return new WaitForSeconds(wait);
                _pawnKindDef.CanGetJob = true;
            }
        }

        #endregion

        #region Battle

        private bool canDamaged = true;

        public void GetDamage(GameObject enemy, float damage)
        {
            // �޵�֡����
            if (!canDamaged) return;

            // �жϵ�ǰ����,Ȼ��50���ʷ�����50��������
            StopJob(10);
            if (Random.value < 0.5f)
            {
                StateMachine.TryChangeState(new PawnJob_Attack(this, enemy));
            }
            else
            {
                StateMachine.TryChangeState(new PawnJob_Escape(this, enemy));
            }

            _pawnInfo.HP.CurValue -= damage;

            // Ѫ��Ϊ��������
            if (_pawnInfo.HP.IsSpace)
            {
                Info.IsDead = true;
                Destroy(gameObject);
                return;
            }

            // �����������޵�֡
            Animator.SetTrigger("IsHurted");
            StartCoroutine(AvoidDamage(2));
        }

        IEnumerator AvoidDamage(float time)
        {
            canDamaged = false;
            yield return new WaitForSeconds(time);
            canDamaged = true;
        }

        #endregion

        protected virtual void Start()
        {
            /* �������������ƶ���� */
            MoveController = GetComponent<PawnMoveController>();

            /* �����������Ķ������ */
            Animator = GetComponent<Animator>();

            /* ����״̬�� */
            StateMachine = new StateMachine(this.gameObject, new PawnJob_Idle(this));

            /* ����ͼ��Pawn�ͱ�ǩ */
            gameObject.layer = 7;
            gameObject.tag = "Pawn";
        }

        private void OnEnable()
        {
            GameManager.Instance.OnTimeAddOneMinute += UpdatePawnInfo;
            GameManager.Instance.OnGameStart += Instance_OnGameStart;
        }

        private void Instance_OnGameStart()
        {
            transform.Find("Emotion").gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            GameManager.Instance.OnTimeAddOneMinute -= UpdatePawnInfo;
            GameManager.Instance.OnGameStart -= Instance_OnGameStart;
        }

        private void UpdatePawnInfo()
        {
            if(StateMachine.CurStateType != typeof(PawnJob_Sleep))
            {
                Info.Sleepiness.CurValue -= 0.07f;
            }
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            �����б�Debug();
#endif
            if (!Def.StopUpdate)
            {
                StateMachine.Update();
                if (!Info.IsOnWork && Def.CanGetJob) TryToGetJob();
            }
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

        private void OnDestroy()
        {
            GameManager.Instance.PawnGeneratorTool.RemovePawn(this);
        }
    }
}