using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveController))]
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
    public bool IsDead;
    public bool IsSelect;
    public bool IsOnWork;   
    public bool IsOnBattle;
    public bool IsDrafted;

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

    public bool TryToEnterBattle(Pawn battleTarget)
    {
        IsOnBattle = true;
        CurJobTarget = battleTarget.gameObject;

        // ����λ��
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
