using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveController))]
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
    public bool IsDead;
    public bool IsSelect;
    public bool IsOnWork;   
    public bool IsOnBattle;
    public bool IsDrafted;

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

    public bool TryToEnterBattle(Pawn battleTarget)
    {
        IsOnBattle = true;
        CurJobTarget = battleTarget.gameObject;

        // 设置位置
        Vector3 me = transform.position;
        Vector3 him = battleTarget.gameObject.transform.position;
        Vector3 min = (me + him) / 2;
        if (me.x < min.x)
        {
            MoveControl.FilpRight();
        }
        else
        {
            MoveControl.FilpLeft();
        }

        Animator.SetTrigger("IsAttack");
        return true;
    }

    public bool TryToEndBattle()
    {
        IsOnBattle = false;
        CurJobTarget = null;
        return true;
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

    #region Select

    public void TrySelect()
    {
        if (!IsSelect)
        {
            WhenPawnSelected();
        }
        else
        {
            WhenPawnCanelSelected();
        }
    }

    private void WhenPawnSelected()
    {
        if (!CanSelect) return;
        GameObject indicator = null;
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
            Debug.LogError("No Find the SelectionBox");
            return;
        }
        SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
        sr.DOFade(1, 1);
        IsSelect = true;
    }

    private void WhenPawnCanelSelected()
    {
        if (!CanSelect) return;
        GameObject indicator = null;
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
            Debug.LogError("No Find the SelectionBox");
            return;
        }
        SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
        sr.DOFade(0, 1);
        IsSelect = false;
    }

    #endregion

    #region Draft

    public void TryDraft()
    {
        if(CanDrafted)
        {
            IsDrafted = true;
        }
    }

    public void CaneclDraft()
    {
        IsDrafted = false;
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
