using UnityEngine;
using ChenChen_BuildingSystem;


/// <summary>
///  管理人物的逻辑，顶层
/// </summary>
public class CharacterMain : Pawn
{
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

    private void Start()
    {
        /* 在游戏开始添加这个人物可以存在的状态 */
        StateMachine = new StateMachine(new PawnStates.PawnState_Idle(this));

        /* 在游戏开始添加这个人物的移动组件 */
        MoveControl = GetComponent<CharacterMoveController>();

        /* 在游戏开始添加这个人物的动画组件 */
        Animator = GetComponent<Animator>();

        /* 选中时显示的光标 */
        _selectIndicator = transform.Find("SelectIndicator").gameObject;

        /* 人物能力值 */
        Attribute.InitPawnAttribute();

    }

    protected override void Update()
    {
        base.Update();
        TryToGetJob();
    }

    private void TryToGetJob()
    {
        GameObject job = null;
        if (!IsOnWork && CanGetJob)
        {
            job = BuildingSystemManager.Instance.GetTask();
            if (job != null)
            {
                StateMachine.SetNextState(new PawnStates.PawnState_Build(this, job));
                return;
            }

            job = BuildingSystemManager.Instance.GetBuildingObjFromListCompleted("钓鱼点");
            if (job != null)
            {
                StateMachine.SetNextState(new PawnStates.PawnState_Fishing(this, job));
                return;
            }
        }
        return;
    }
}
