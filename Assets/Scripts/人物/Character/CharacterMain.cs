using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBuildingSystem;

/// <summary>
///  ����������߼�������
/// </summary>
public class CharacterMain : MonoBehaviour
{
    /// <summary>
    ///  �����״̬��
    /// </summary>
    public StateMachine StateMachine { get; private set; }

    /// <summary>
    ///  �����ƶ��Ŀ���
    /// </summary>
    public CharacterMoveController MoveControl { get; private set; }

    /// <summary>
    ///  ���ﶯ��״̬����
    /// </summary>
    public Animator Animator { get; private set; }

    /// <summary>
    /// ���������ֵ
    /// </summary>
    public CharacterAttribute Attribute {  get; private set; }


    [Header("һЩ�߼�����ֵ")]
    // �Ƿ�ѡ���й�
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

    // ��ǰ�ܷ���
    public bool CanGetTask;

    // ��ǰ�Ƿ����ڹ���
    public bool IsOnWork;

    // �ܷ���
    public bool CanBuild;
    [HideInInspector] public BlueprintBase CurrentBuiltObject;

    // �ܷ�ȥ����
    public bool CanFishing;


    [Header("һЩ�������߼�ֵ")]
    [Header("������������")]
    public float buildSpeed;
    public float combatEffect;

    [Header("�����߼�����")]
    public float WorkRange;
    public float AttackRange;

    [Header("��ǰ����")]
    public List<string> CurrentTaskList = new List<string>();

    private void Start()
    {
        /* ����Ϸ��ʼ������������Դ��ڵ�״̬ */
        StateMachine = new StateMachine();
        StateMachine.AddState(new CharacterStates.IdleState(StateMachine));

        /* ����Ϸ��ʼ������������ƶ���� */
        MoveControl = GetComponent<CharacterMoveController>();

        /* ����Ϸ��ʼ����������Ķ������ */
        Animator = GetComponent<Animator>();

        /* ѡ��ʱ��ʾ�Ĺ�� */
        _selectIndicator = transform.Find("SelectIndicator").gameObject;

        /* ��������ֵ */
        Attribute = new CharacterAttribute();
    }

    private void Update()
    {
        StateMachine.Update();
        GetTask();
        UpdateAttributeFloat();
        �����б�Debug();
    }

    #region Attribute
    
    /// <summary>
    /// ���¸�����������ֵ�����������͵�
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
        // �����ǰ���ܽ��������򷵻�
        if (!CanGetTask) return;

        // ������ڹ������򷵻�
        if (IsOnWork) return;

        // �������ȼ��ж��Ƚ���ʲô����
        AcceptBuildingTask();
    }

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    private void AcceptBuildingTask()
    {
        // ��ȡһ������
        CurrentBuiltObject = BuildingSystemManager.Instance.GetTask();

        // ����ɹ����������������������״̬
        if (CurrentBuiltObject != null && !IsOnWork)
        {
            IsOnWork = true;
            Vector2 buildPos = CurrentBuiltObject.transform.position;
            StateMachine.AddState(new CharacterStates.BuildState(this, buildPos));
        }
    }

    #endregion

    private void �����б�Debug()
    {
        CurrentTaskList.Clear();
        CurrentTaskList.Add("���ڣ�" + StateMachine.currentState?.ToString());
        foreach (var task in StateMachine.StateQueue)
        {
            CurrentTaskList.Add("׼��" + task.ToString());
        }
    }
}
