using ChenChen_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(MoveController))]
    public abstract class Pawn : MonoBehaviour, IDetailView
    {
        public StateMachine StateMachine { get; protected set; }

        public PawnMoveController MoveController { get; protected set; }

        public EmotionController EmotionController { get; protected set; }

        [Header("当前任务")]
        public GameObject CurJobTarget;
        public List<string> CurrentStateList = new List<string>();

        [Header("人物逻辑属性")]
        public float WorkRange = 1.35f;
        public float AttackRange = 2;
        public float AttackSpeedWait = 2f;
        public float AttackDamage => Attribute.A_Combat.Value;

        private Slider workProgressSlider;
        public void ChangeMyBar(float value)
        {
            Mathf.Clamp01(value);
            if(workProgressSlider == null)
            {
                workProgressSlider = transform.Find("Canvas").Find("WorkProgressSlider").GetComponent<Slider>();
            }
            if (workProgressSlider != null)
            {
                if(value > 0)
                {
                    workProgressSlider.gameObject.SetActive(true);
                    workProgressSlider.value = value;
                }
                else
                {
                    workProgressSlider.gameObject.SetActive(false);
                    workProgressSlider.value = 0f;
                }       
            }
            else
            {
                Debug.LogError("未找到对应物体");
            }  
        }

        [Header("Body")]
        public SpriteRenderer SR_Hair;
        public SpriteRenderer SR_Head;
        public SpriteRenderer SR_Appeal;
        public SpriteRenderer SR_Body;

        private Animator anim;

        public void SetAnimator(string animation, bool value)
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(param.name, false);
                }
            }

            anim.SetBool(animation, value);
        }

        #region - 属性 - 

        [Header("人物定义")]
        [SerializeField] private PawnKindDef _pawnKindDef;
        public PawnKindDef Def
        {
            get 
            { 
                if(_pawnKindDef == null)
                {
                    Debug.LogError("Def is Null");
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

        private EnemyPawnBehavior _enemyPawnBehavior = null;
        protected EnemyPawnBehavior enemyPawnBehavior
        {
            get
            {
                if (_enemyPawnBehavior == null)
                {
                    _enemyPawnBehavior = new EnemyPawnBehavior(this);
                }
                return _enemyPawnBehavior;
            }
        }

        public string Faction => Info.faction; 

        #endregion

        #region - Job -

        protected abstract void TryToGetJob();

        /// <summary>
        /// Going to work for job, but not in work now;
        /// </summary>
        /// <param name="job"></param>
        public void JobToDo(GameObject job)
        {
            _pawnKindDef.CanGetJob = false;
            _pawnInfo.IsInWork = false;
            CurJobTarget = job;
        }

        /// <summary>
        /// IsOnWork => ture
        /// </summary>
        public void JobDoing()
        {
            _pawnInfo.IsInWork = true;
        }

        /// <summary>
        /// Complete Job, CanGetJob => true, IsOnWork => false
        /// </summary>
        public void JobDone()
        {
            _pawnKindDef.CanGetJob = true;
            _pawnInfo.IsInWork = false;
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

        #region - Battle -

        private bool canDamaged = true;

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="from"> 伤害来源 </param>
        /// <param name="damage"></param>
        public void GetDamage(GameObject from, float damage)
        {
            if (!canDamaged) return;

            StartCoroutine(AvoidDamage(2));

            StopJob(10);
            if(StateMachine.CurStateType != typeof(PawnJob_Attack))
            {
                StateMachine.TryChangeState(new PawnJob_Chase(this, from));
            }

            Info.HP.CurValue -= damage;
        }

        IEnumerator AvoidDamage(float time)
        {
            canDamaged = false;
            yield return new WaitForSeconds(time);
            canDamaged = true;
        }

        #endregion

        #region - Life -

        protected virtual void Start()
        {
            MoveController = GetComponent<PawnMoveController>();
            anim = GetComponent<Animator>();
            EmotionController = GetComponentInChildren<EmotionController>();
            StateMachine = new StateMachine(this.gameObject, new PawnJob_Idle(this));

            gameObject.layer = 7;
            gameObject.tag = "Pawn";       
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            任务列表Debug();
#endif
            if (Def.StopUpdate) return;

            if (Info.IsDead) return;

            if (Info.HP.IsSpace)
            {
                Info.IsDead = true;          
            }

            StateMachine.Update();

            if (Faction != GameManager.PLAYER_FACTION)
            {
                enemyPawnBehavior.Behavior();
                return;
            }

            if (!Info.IsInWork && Def.CanGetJob) TryToGetJob();
        }

        private void OnEnable()
        {
            GameManager.Instance.OnTimeAddOneMinute += UpdatePawnInfo;
            GameManager.Instance.OnGameStart += Instance_OnGameStart;
        }

        private void UpdatePawnInfo()
        {
            if (StateMachine.CurStateType != typeof(PawnJob_Sleep))
            {
                Info.Sleepiness.CurValue -= 0.07f;
            }
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

        #endregion
    }
}