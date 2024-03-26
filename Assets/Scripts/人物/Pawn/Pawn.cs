using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// 人物的状态机
    /// </summary>
    public StateMachine StateMachine {  get; protected set; }

    /// <summary>
    /// 人物移动的控制
    /// </summary>
    public CharacterMoveController MoveControl { get; protected set; }

    /// <summary>
    /// 人物动画状态控制
    /// </summary>
    public Animator Animator { get; protected set; }

    [Header("当前状态")]
    public List<string> CurrentStateList = new List<string>();

    [Header("人物逻辑属性")]
    public float WorkRange;
    public float AttackRange;

    [Header("人物能力属性")]
    public PawnAttribute Attribute;

    [Header("人物状态属性")]
    [SerializeField] protected bool CanGetJob;    // 当前能否工作
    [SerializeField] protected bool IsOnWork;    // 当前是否正在工作
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

    protected void 任务列表Debug()
    {
        CurrentStateList.Clear();
        CurrentStateList.Add("正在：" + StateMachine.currentState?.ToString());
        CurrentStateList.Add("下一个：" + StateMachine.GetNextState()?.ToString());
        foreach (var task in StateMachine.GetStateQueue())
        {
            CurrentStateList.Add("准备" + task.ToString());
        }
    }

    protected virtual void Update()
    {
        StateMachine.Update();

#if UNITY_EDITOR
        任务列表Debug();
#endif

    }
}
