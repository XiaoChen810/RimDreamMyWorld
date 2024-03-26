using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// �����״̬��
    /// </summary>
    public StateMachine StateMachine {  get; protected set; }

    /// <summary>
    /// �����ƶ��Ŀ���
    /// </summary>
    public CharacterMoveController MoveControl { get; protected set; }

    /// <summary>
    /// ���ﶯ��״̬����
    /// </summary>
    public Animator Animator { get; protected set; }

    [Header("��ǰ״̬")]
    public List<string> CurrentStateList = new List<string>();

    [Header("�����߼�����")]
    public float WorkRange;
    public float AttackRange;

    [Header("������������")]
    public PawnAttribute Attribute;

    [Header("����״̬����")]
    [SerializeField] protected bool CanGetJob;    // ��ǰ�ܷ���
    [SerializeField] protected bool IsOnWork;    // ��ǰ�Ƿ����ڹ���
    [SerializeField] protected GameObject CurJob;

    /// <summary>
    /// Go to work for job
    /// </summary>
    /// <param name="job"></param>
    public void JobToDo(GameObject job)
    {
        CanGetJob = false;
        IsOnWork = false;
        CurJob = job;
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
        CurJob = null;
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
    public void JobDontGet()
    {
        CanGetJob = false;
    }

    protected void �����б�Debug()
    {
        CurrentStateList.Clear();
        CurrentStateList.Add("���ڣ�" + StateMachine.currentState?.ToString());
        CurrentStateList.Add("��һ����" + StateMachine.GetNextState()?.ToString());
        foreach (var task in StateMachine.GetStateQueue())
        {
            CurrentStateList.Add("׼��" + task.ToString());
        }
    }

    protected virtual void Update()
    {
        StateMachine.Update();

#if UNITY_EDITOR
        �����б�Debug();
#endif

    }
}
