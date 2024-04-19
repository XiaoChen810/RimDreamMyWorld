using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// 人物的状态机
    /// </summary>
    public StateMachine StateMachine {  get; protected set; }

    /// <summary>
    /// 人物移动的控制
    /// </summary>
    public PawnMoveController MoveControl { get; protected set; }

    /// <summary>
    /// 人物动画状态控制
    /// </summary>
    public Animator Animator { get; protected set; }

    [Header("当前状态")]
    public List<string> CurrentStateList = new List<string>();

    [Header("人物逻辑属性")]
    public float WorkRange = 1;
    public float AttackRange = 1;
    public float AttackSpeed = 0.76f;
    public float AttackSpeedWait = 0.5f;

    [Header("人物状态属性")]
    public int Hp = 100; 

    [Header("人物能力属性")]
    public PawnAttribute Attribute;

    [Header("人物信息")]
    public string PawnName;
    public string FactionName;

    [Header("人物状态 Can")]
    public bool CanSelect = true;
    public bool CanGetJob = true;
    public bool CanBattle = true;
    public bool CanDrafted = true;

    [Header("人物状态 Is")]
    [SerializeField] protected bool _isDead;
    [SerializeField] protected bool _isSelect;
    [SerializeField] protected bool _isOnWork;
    [SerializeField] protected bool _isOnBattle;
    [SerializeField] protected bool _isDrafted;
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
        set
        {
            _isDead = value;
        }
    }
    public bool IsSelect
    {
        get 
        { 
            return _isSelect;
        }
        set
        {
            if (_isSelect == value) return;
            if(!CanSelect) return;
            if(value)
            {
                Indicator_DOFadeOne();
            }
            else
            {
                Indicator_DOFadeZero();
                IsDrafted = false;              
            }
            _isSelect = value;
        }
    }
    public bool IsOnWork
    {
        get 
        { 
            return _isOnWork; 
        }
        set
        {
            _isOnWork = value;
        }
    }
    public bool IsOnBattle
    {
        get
        {
            return _isOnBattle;
        }
        set
        {
            _isOnBattle = value;
        }
    }
    public bool IsDrafted
    {
        get
        {
            return _isDrafted;
        }
        set
        {
            if (_isDrafted == value) return;
            if (!CanDrafted) return;

            if (value)
            {
                Indicator_DOColorRed();
            }
            else
            {
                Indicator_DOColorWhite();
                IsSelect = false;
            }
            _isDrafted = value;
        }
    }

    [Header("人物指向")]
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

        // 设置位置
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
        while(IsOnBattle)
        {
            yield return null;  
            if(!isTriggerAttack)
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
        if(Hp <= 0)
        {
            IsDead = true;
            this.gameObject.SetActive(false);
            return;           
        }
        Animator.SetTrigger("IsHurted");
    }

    #endregion

    #region Indicator

    // 获取人物的指示器，如果不存在则创建
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
        /* 添加这个人物的移动组件 */
        MoveControl = GetComponent<PawnMoveController>();

        /* 添加这个人物的动画组件 */
        Animator = GetComponent<Animator>();

        /* 配置状态机 */
        StateMachine = new StateMachine(new ChenChen_AI.PawnJob_Idle(this), this);

        /* 设置图层Pawn和标签 */
        gameObject.layer = 7;
        gameObject.tag = "Pawn";
    }

    protected virtual void Update()
    {
        StateMachine.Update();
        TryToGetJob();

#if UNITY_EDITOR
        任务列表Debug();
#endif
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
