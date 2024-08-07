using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_UI;
using ChenChen_Core;
using ChenChen_Thing;
using System;
using System.Linq;

namespace ChenChen_AI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(MoveController))]
    public abstract class Pawn : MonoBehaviour, IDetailView, IStorage
    {
        public StateMachine StateMachine { get; protected set; }

        public PawnMoveController MoveController { get; protected set; }

        public EmotionController EmotionController { get; protected set; }

        [Header("当前任务")]
        public TargetPtr CurJobTarget;
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

        [SerializeField] private Vector3 _hand;
        public Vector3 hand => _hand;

        #region - IStorage -
        private Dictionary<string, int> bag = new Dictionary<string, int>();

        public Dictionary<string, int> Bag => bag;

        public void Put(string name, int num)
        {
            if (bag.ContainsKey(name))
            {
                bag[name] += num;
            }
            else
            {
                bag.Add(name, num);
            }
        }

        public int Get(string name, int num)
        {
            if (bag.ContainsKey(name))
            {
                int store = bag[name];
                if(store - num > 0)
                {
                    store -= num;
                    return num;
                }
                else
                {
                    return store;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region - Body -

        [Header("Body")]
        [SerializeField] private SpriteRenderer SR_Hair;
        [SerializeField] private SpriteRenderer SR_Head;      
        [SerializeField] private SpriteRenderer SR_Body;

        [Header("Apparel Part")]
        [SerializeField] private SpriteRenderer SR_UpperHead;
        [SerializeField] private SpriteRenderer SR_Torso;
        [SerializeField] private SpriteRenderer SR_Shoulders;
        [SerializeField] private SpriteRenderer SR_Legs;
        [SerializeField] private SpriteRenderer SR_Eyes;
        [SerializeField] private SpriteRenderer SR_Neck;
        private Dictionary<string, SpriteRenderer> bodyPartRenderers;

        public void SetDressed(ApparelDef apparel)
        {
            List<string> partGroup = apparel.bodyPartGroups;
            string mainPart = partGroup[0];

            // 先脱
            foreach (string part in partGroup)
            {
                if (bodyPartRenderers.TryGetValue(part, out var spriteRenderer))
                {
                    spriteRenderer.sprite = null;
                }
                else
                {
                    Debug.LogError($"不存在字段 {part}");
                }
            }

            // 再穿
            if (bodyPartRenderers.TryGetValue(mainPart, out var mainSpriteRenderer))
            {
                mainSpriteRenderer.sprite = apparel.sprite;
            }
            else
            {
                Debug.LogError($"不存在字段 {mainPart}");
            }
        }

        public void SetHair(HairDef hairDef, bool changeColor)
        {
            SR_Hair.sprite = hairDef.sprite;

            if (changeColor)
            {
                Color randomColor = new Color(
                    UnityEngine.Random.Range(0.2f, 0.6f), 
                    UnityEngine.Random.Range(0.1f, 0.4f), 
                    UnityEngine.Random.Range(0.05f, 0.2f) 
                );

                SR_Hair.color = randomColor;
            }
        }
        public void SetHead(HeadDef headDef)
        {
            SR_Head.sprite = headDef.sprite;
        }
        public void SetBody(BodyDef bodyDef, bool changeColor)
        {
            SR_Body.sprite = bodyDef.sprite;

            if (changeColor)
            {
                // 常用的皮肤颜色
                Color[] skinColors = new Color[]
                {
                new Color(0.99f, 0.89f, 0.77f), // 浅肤色1
                new Color(0.95f, 0.80f, 0.68f), // 浅肤色2
                new Color(0.87f, 0.72f, 0.53f), // 中等肤色1
                new Color(0.78f, 0.58f, 0.36f), // 中等肤色2
                new Color(0.65f, 0.50f, 0.39f), // 深肤色1
                new Color(0.55f, 0.41f, 0.33f), // 深肤色2
                new Color(0.43f, 0.34f, 0.27f)  // 深肤色3
                };

                // 从数组中随机选择一个颜色
                Color randomColor = skinColors[UnityEngine.Random.Range(0, skinColors.Length)];

                SR_Body.color = randomColor;
                SR_Head.color = randomColor;
            }
        }

        #endregion

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
        public void JobToDo(TargetPtr job)
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
                StateMachine.TryChangeState(new PawnJob_Chase(this, new TargetPtr(from)));
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

        private void Awake()
        {
            // 初始化字典映射
            bodyPartRenderers = new Dictionary<string, SpriteRenderer>
            {
                { "UpperHead", SR_UpperHead },
                { "Torso", SR_Torso },
                { "Shoulders", SR_Shoulders },
                { "Legs", SR_Legs },
                { "Eyes", SR_Eyes },
                { "Neck", SR_Neck }
            };
        }

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