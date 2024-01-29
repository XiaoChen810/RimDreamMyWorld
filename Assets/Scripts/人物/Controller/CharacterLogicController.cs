using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ����ϵͳ;

/// <summary>
///  ����������߼�
/// </summary>
public class CharacterLogicController : MonoBehaviour
{
    public CharacterStateMachine StateMachine {  get; private set; }
    public CharacterMoveController MoveControl { get; private set; }

    [Header("һЩ�߼�����ֵ")]
    // �Ƿ�ѡ��
    public bool IsSelect;

    // �ɷ���
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
        // ����һ���µĽ�������
        if (CanGetTask && currentBuiltObject == null)
        {
            currentBuiltObject = BuildingSystemManager.Instance.GetTask();
            if (currentBuiltObject == null)
            {
                // ��û�п��Խ��յ�����
                CanGetTask = false;
            }
            else
            {
                // �Ѿ�����������
                IsGetTask = true;
            }
            
        }
        // ���һ���µ�״̬
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
