using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
///  管理人物的逻辑，顶层
/// </summary>
public class CharacterMain : MonoBehaviour
{
    /// <summary>
    ///  人物的状态机
    /// </summary>
    public StateMachine StateMachine { get; private set; }

    /// <summary>
    ///  人物移动的控制
    /// </summary>
    public CharacterMoveController MoveControl { get; private set; }

    /// <summary>
    ///  人物动画状态控制
    /// </summary>
    public Animator Animator { get; private set; }

    /// <summary>
    /// 人物的属性值
    /// </summary>
    public CharacterAttribute Attribute {  get; private set; }


    [Header("一些逻辑布尔值")]
    // 是否被选中有关
    [SerializeField] private bool _isSelect;
    public bool IsSelect
    {
        get { return _isSelect; }
        set
        {
            if (_isSelect != value)
            {
                _selectIndicator.SetActive(value);
                Animator.SetBool("IsSelected", value);
                _isSelect = value;
            }
        }
    }
    private GameObject _selectIndicator;

    // 当前能否工作
    public bool CanGetTask;

    // 当前是否正在工作
    public bool IsOnWork;

    // 能否建造
    public bool CanBuild;
    [HideInInspector] public BlueprintBase CurrentBuiltObject;

    // 能否去钓鱼
    public bool CanFishing;


    [Header("一些浮点数逻辑值")]
    [Header("人物能力属性")]
    public float buildSpeed;
    public float combatEffect;

    [Header("人物逻辑属性")]
    public float WorkRange;
    public float AttackRange;

    [Header("当前任务")]
    public List<string> CurrentTaskList = new List<string>();

    private void Start()
    {
        /* 在游戏开始添加这个人物可以存在的状态 */
        StateMachine = new StateMachine();
        StateMachine.AddState(new CharacterStates.IdleState(StateMachine));

        /* 在游戏开始添加这个人物的移动组件 */
        MoveControl = GetComponent<CharacterMoveController>();

        /* 在游戏开始添加这个人物的动画组件 */
        Animator = GetComponent<Animator>();

        /* 选中时显示的光标 */
        _selectIndicator = transform.Find("SelectIndicator").gameObject;

        /* 人物能力值 */
        Attribute = new CharacterAttribute();
    }

    private void Update()
    {
        StateMachine.Update();
        GetTask();
        UpdateAttributeFloat();
        任务列表Debug();
    }

    #region Attribute
    
    /// <summary>
    /// 更新个人属性能力值，浮点数类型的
    /// </summary>
    private void UpdateAttributeFloat()
    {
        float baseValue = 1f;
        buildSpeed = baseValue + Attribute.A_Construction.EXP;
        combatEffect = baseValue + Attribute.A_Combat.EXP;
    }

    #endregion

    #region Task

    private void GetTask()
    {
        // 如果当前不能接受任务，则返回
        if (!CanGetTask) return;

        // 如果正在工作，则返回
        if (IsOnWork) return;

        // 根据优先级判断先接受什么任务
        AcceptBuildingTask();
    }

    /// <summary>
    /// 获取建造任务
    /// </summary>
    private void AcceptBuildingTask()
    {
        // 获取一个任务
        CurrentBuiltObject = BuildingSystemManager.Instance.GetTask();

        // 如果成功接受了任务，添加这个任务的状态
        if (CurrentBuiltObject != null && !IsOnWork)
        {
            IsOnWork = true;
            Vector2 buildPos = CurrentBuiltObject.transform.position;
            StateMachine.AddState(new CharacterStates.BuildState(this, buildPos));
        }
    }

    #endregion

    private void 任务列表Debug()
    {
        CurrentTaskList.Clear();
        CurrentTaskList.Add("正在：" + StateMachine.currentState?.ToString());
        foreach (var task in StateMachine.StateQueue)
        {
            CurrentTaskList.Add("准备" + task.ToString());
        }
    }
}
