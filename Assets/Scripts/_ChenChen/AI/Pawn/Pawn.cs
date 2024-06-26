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
        /// 人物的状态机
        /// </summary>
        public StateMachine StateMachine { get; protected set; }

        /// <summary>
        /// 人物移动的控制
        /// </summary>
        public PawnMoveController MoveController { get; protected set; }

        /// <summary>
        /// 人物动画状态控制
        /// </summary>
        public Animator Animator { get; protected set; }

        public EmotionController EmotionController;

        [Header("当前任务")]
        public GameObject CurJobTarget;
        public List<string> CurrentStateList = new List<string>();

        [Header("人物逻辑属性")]
        public float WorkRange = 1;
        public float AttackRange = 1;
        public float AttackSpeedWait = 2.5f;
        public float AttackDamage = 0;

        [Header("人物定义")]
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

        [Header("人物能力属性")]
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
        [Header("人物状态信息")]
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

        // 细节视图
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
        /// 暂停当前工作，并停止接受工作一段时间
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
            // 无敌帧返回
            if (!canDamaged) return;

            // 中断当前工作,然后50概率反击，50概率逃跑
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

            // 血条为空则死亡
            if (_pawnInfo.HP.IsSpace)
            {
                Info.IsDead = true;
                Destroy(gameObject);
                return;
            }

            // 触发动画和无敌帧
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
            /* 添加这个人物的移动组件 */
            MoveController = GetComponent<PawnMoveController>();

            /* 添加这个人物的动画组件 */
            Animator = GetComponent<Animator>();

            /* 配置状态机 */
            StateMachine = new StateMachine(this.gameObject, new PawnJob_Idle(this));

            /* 设置图层Pawn和标签 */
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
            任务列表Debug();
#endif
            if (!Def.StopUpdate)
            {
                StateMachine.Update();
                if (!Info.IsOnWork && Def.CanGetJob) TryToGetJob();
            }
        }

        protected void 任务列表Debug()
        {
            CurrentStateList.Clear();
            if (Def.StopUpdate)
            {
                CurrentStateList.Add("StopUpdate");
                return;
            }
            CurrentStateList.Add("正在：" + StateMachine.CurState?.ToString());
            CurrentStateList.Add("下一个：" + StateMachine.NextState?.ToString());
            foreach (var task in StateMachine.StateQueue)
            {
                CurrentStateList.Add("准备" + task.ToString());
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.PawnGeneratorTool.RemovePawn(this);
        }
    }
}