using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 建筑系统;

/// <summary>
///  管理人物的逻辑
/// </summary>
public class CharacterLogicController : MonoBehaviour
{
    public CharacterStateMachine StateMachine {  get; private set; }
    public CharacterMoveController MoveControl { get; private set; }

    [Header("一些逻辑布尔值")]
    // 是否被选中
    public bool IsSelect;

    // 可否建造
    [SerializeField] private bool _canBuild;
    public bool CanBuild
    {
        get { return _canBuild; }
        set
        {
            if(_canBuild != value)
            {
                if (value == true)
                {
                    BuildingSystemManager.Instance.OnTaskQueueAdded += OnTaskQueueAdded;
                }
                if (value == false)
                {
                    BuildingSystemManager.Instance.OnTaskQueueAdded -= OnTaskQueueAdded;
                }
                _canBuild = value;
            }
        }
    }
    public bool CanGetTask = false;
    public bool IsGetTask = false;
    public BuildingBlueprintBase currentBuiltObject;

    private void Start()
    {
        if (_canBuild) 
            BuildingSystemManager.Instance.OnTaskQueueAdded += OnTaskQueueAdded;
        StateMachine = GetComponent<CharacterStateMachine>();
        MoveControl = GetComponent<CharacterMoveController>();
    }

    private void Update()
    {
        // 接受一个新的建造任务
        if (CanGetTask && currentBuiltObject == null)
        {
            currentBuiltObject = BuildingSystemManager.Instance.GetTask();
            if (currentBuiltObject == null)
            {
                // 并没有可以接收的任务
                CanGetTask = false;
            }
            else
            {
                // 已经接受了任务
                IsGetTask = true;
            }
            
        }
        // 添加一个新的状态
        if (IsGetTask && currentBuiltObject != null)
        {
            IsGetTask = false;
            Vector2 buildPos = currentBuiltObject.transform.position;
            StateMachine.SM.AddState(new CharacterStates.BuildState(this, buildPos));
        }
    }

    private void OnTaskQueueAdded()
    {
        CanGetTask = true;
    }

}
