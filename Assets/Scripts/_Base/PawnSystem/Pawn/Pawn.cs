using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// �����״̬��
    /// </summary>
    public StateMachine StateMachine {  get; protected set; }

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

    [Header("����״̬ Can")]
    public bool CanSelect = true;
    public bool CanGetJob = true;
    public bool CanBattle = true;
    public bool CanDrafted = true;

    [Header("����״̬ Is")]
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
        StateMachine = new StateMachine(new ChenChen_AI.PawnJob_Idle(this), this);

        /* ����ͼ��Pawn�ͱ�ǩ */
        gameObject.layer = 7;
        gameObject.tag = "Pawn";
    }

    protected virtual void Update()
    {
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
