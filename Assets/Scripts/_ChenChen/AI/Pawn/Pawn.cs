using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_UI;
using ChenChen_Core;
using ChenChen_Thing;
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
        public Animator Anim => anim;
        public void SetAnimator(string animation, bool value)
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(param.name, false);
                }
            }

            if (Info.IsDead)
            {
                anim.SetBool("Die", true);
                return;
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

        [Header("Weapon")]
        [SerializeField] private SpriteRenderer SR_Weapon;
        [SerializeField] private Transform weaponHand;

        private WeaponDef weaponDef = null;
        public WeaponDef Weapon
        {
            get
            {
                if (weaponDef == null)
                {
                    weaponDef = WeaponDef.Fist;
                }
                return weaponDef;
            }
        }
        public float WeaponAccuracy => Weapon.accuracy;
        public float WeaponDemage => Weapon.isMelee ? Weapon.meleeDamage : Weapon.rangeDamage;
        public float Armor => 0;

        private Dictionary<string, SpriteRenderer> bodyPartRenderers;
        private Dictionary<string, ApparelDef> bodyPartApparelDefs;
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

                if (bodyPartApparelDefs.TryGetValue(part, out var apparelDef))
                {
                    if(apparelDef != null)
                    {
                        UnloadBody<ApparelDef>(apparelDef);
                    }

                    bodyPartApparelDefs[part] = null;
                }
            }

            // 再穿
            if (bodyPartRenderers.TryGetValue(mainPart, out var mainSpriteRenderer))
            {
                mainSpriteRenderer.sprite = apparel.sprite;

                bodyPartApparelDefs[mainPart] = apparel;
            }
            else
            {
                Debug.LogError($"不存在字段 {mainPart}");
            }            
        }

        public void SetWeapon(WeaponDef def)
        {
            // 先卸下本来的装备
            if (this.weaponDef != null)
            {
                UnloadBody<WeaponDef>(this.weaponDef);
            }
            
            // 装上新的装备
            this.weaponDef = def;

            if (this.weaponDef != null)
            {
                SR_Weapon.sprite = def.sprite;
                SR_Weapon.transform.localRotation = Quaternion.Euler(0f, 0f, def.equippedAngleOffset);
                EndBattle();
            }                    
        }

        private void UnloadBody<T>(T def) where T : Def
        {
            ThingSystemManager.Instance.GenerateItem(def, transform.position, 1);
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
            Def.CanGetJob = false;
            Info.IsInWork = false;
            CurJobTarget = job;
        }

        /// <summary>
        /// IsOnWork => ture
        /// </summary>
        public void JobDoing()
        {
            Info.IsInWork = true;
        }

        /// <summary>
        /// Complete Job, CanGetJob => true, IsOnWork => false
        /// </summary>
        public void JobDone()
        {
            Def.CanGetJob = true;
            Info.IsInWork = false;
            CurJobTarget = null;
        }

        /// <summary>
        /// CanGetJob => true
        /// </summary>
        public void JobCanGet()
        {
            Def.CanGetJob = true;
        }

        /// <summary>
        /// CanGetJob => false;
        /// </summary>
        public void JobCannotGet()
        {
            Def.CanGetJob = false;
        }

        #endregion

        #region - Battle -

        [Header("Battle")]
        public GameObject bulletPrefab = null;

        /// <summary>
        /// 调整武器角度
        /// </summary>
        /// <param name="target"></param>
        public void SetWeaponAngle(Vector3 target)
        {
            Vector3 dir = target - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            weaponHand.localRotation = Quaternion.Euler(0f, 0f, angle);
            weaponHand.localScale = new Vector3(1, (transform.position.x <= target.x) ? 1 : -1, 1);
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="to">伤害目标</param>
        public void SetDamage(GameObject to, bool isMelee)
        {
            Pawn toPawn = to.GetComponent<Pawn>();
            if(toPawn != null)
            {
                if (weaponDef.isMelee)
                {
                    anim.SetTrigger("Attack_Melee");
                }
                else
                {
                    GameObject obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    Bullet bullet = obj.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        bullet.Shot(to.transform.position);
                    }
                    anim.SetTrigger("Attack_Ranged");
                }

                toPawn.GetDamage(gameObject, DamageJoddge(this, toPawn), isMelee);
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="from"> 伤害来源 </param>
        /// <param name="damage"></param>
        public void GetDamage(GameObject from, float damage, bool isMelee)
        {
            if (StateMachine.CurStateType != typeof(PawnJob_Battle))
            {
                if (!isMelee && !weaponDef.isMelee)
                {
                    var list = ThingSystemManager.Instance.GetThingsInstance<Building>()
                        .Where(x => x.Def.Isbunker)
                        .OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position))
                        .ToList();

                    Building bunker = list.FirstOrDefault(); // 找到最近的掩体

                    if (bunker != null)
                    {
                        StateMachine.TryChangeState(new PawnJob_HideInBunker(this, new TargetPtr(bunker.gameObject, from)));
                    }
                }
                else
                {
                    StateMachine.TryChangeState(new PawnJob_Battle(this, new TargetPtr(from)));
                }
            }
            Info.HP.CurValue -= damage;
        }


        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle()
        {
            weaponHand.localRotation = Quaternion.Euler(0f, 0f, 60);
        }

        private static float DamageJoddge(Pawn attacker, Pawn defender)
        {
            if (UnityEngine.Random.value > attacker.WeaponAccuracy)
            {
                return 0;
            }

            return attacker.AttackDamage - defender.Armor;
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

            bodyPartApparelDefs = new Dictionary<string, ApparelDef>
            {
                { "UpperHead", null },
                { "Torso", null },
                { "Shoulders", null },
                { "Legs", null },
                { "Eyes", null },
                { "Neck", null }
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
                SetAnimator("Die", true);
                ChangeMyBar(0);
            }

            StateMachine.Update();

            if (Faction != GameManager.PLAYER_FACTION)
            {
                enemyPawnBehavior.Behavior();
                return;
            }

            if (!Info.IsInWork && Def.CanGetJob)
            {
                TryToGetJob();
            }
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