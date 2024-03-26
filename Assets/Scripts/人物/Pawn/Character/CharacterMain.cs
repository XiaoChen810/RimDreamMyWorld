using UnityEngine;
using ChenChen_BuildingSystem;


/// <summary>
///  ����������߼�������
/// </summary>
public class CharacterMain : Pawn
{
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

    private void Start()
    {
        /* ����Ϸ��ʼ������������Դ��ڵ�״̬ */
        StateMachine = new StateMachine(new PawnStates.PawnState_Idle(this));

        /* ����Ϸ��ʼ������������ƶ���� */
        MoveControl = GetComponent<CharacterMoveController>();

        /* ����Ϸ��ʼ����������Ķ������ */
        Animator = GetComponent<Animator>();

        /* ѡ��ʱ��ʾ�Ĺ�� */
        _selectIndicator = transform.Find("SelectIndicator").gameObject;

        /* ��������ֵ */
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

            job = BuildingSystemManager.Instance.GetBuildingObjFromListCompleted("�����");
            if (job != null)
            {
                StateMachine.SetNextState(new PawnStates.PawnState_Fishing(this, job));
                return;
            }
        }
        return;
    }
}
