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
        StateMachine = new StateMachine(new ChenChen_AI.PawnJob_Idle(this));

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
            job = BuildingSystemManager.Instance.GetBuildingObj(BuildingStateType.WaitingBuilt, needFree: true);
            if (job != null)
            {
                StateMachine.SetNextState(new ChenChen_AI.PawnJob_Build(this, job));
                return;
            }

            job = BuildingSystemManager.Instance.GetBuildingObj("�����", needFree: true);
            if (job != null)
            {
                StateMachine.SetNextState(new ChenChen_AI.PawnJob_Fishing(this, job));
                return;
            }

            job = BuildingSystemManager.Instance.GetBuildingObj(BuildingStateType.WaitingDemolished, needFree: true);
            if (job != null)
            {
                StateMachine.SetNextState(new ChenChen_AI.PawnJob_Demolished(this, job));
                return;
            }
        }
        return;
    }
}
