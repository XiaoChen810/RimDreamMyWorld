using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_UISystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using System;
using ChenChen_AI;

public class GameManager : SingletonMono<GameManager>
{
    public PanelManager PanelManager { get; private set; }
    public SelectTool SelectTool { get; private set; }
    public PawnGeneratorTool PawnGeneratorTool { get; private set; }
    public AnimatorTool AnimatorTool { get; private set; }

    private bool isMenuPanelCreated;


    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// ��Ϸ��ȫ����Pawn��GameObject�б�
    /// </summary>
    public List<GameObject> PawnsList   
    {
        get { return _pawnsList; }
    }

    [SerializeField] private List<PawnKindDef> _totalPawnDefList = new List<PawnKindDef>();
    /// <summary>
    /// ȫ��Pawn������б���Ϸ��ʼʱ����
    /// </summary>
    public List<PawnKindDef> TotalPawnDefList
    {
        get { return _totalPawnDefList; }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// ������������ѡ��ʱʹ�õĽ�ɫ�б�
    /// </summary>
    public List<Pawn> PawnWhenStartList
    {
        get { return _pawnWhenStartList; }
    }

    public GameObject CharacterTest;
    public GameObject GoblinPrefab;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        PanelManager = new PanelManager();
        SelectTool = GetComponent<SelectTool>();
        PawnGeneratorTool = new PawnGeneratorTool(this);
        AnimatorTool = GetComponent<AnimatorTool>();
        _totalPawnDefList.Add(StaticPawnDef.s_Bald);
        _totalPawnDefList.Add(StaticPawnDef.s_SinglePonytail);
        _totalPawnDefList.Add(StaticPawnDef.s_RedHair);
        _totalPawnDefList.Add(StaticPawnDef.s_YellowHair);
        _totalPawnDefList.Add(StaticPawnDef.s_CrewCut);
        _totalPawnDefList.Add(StaticPawnDef.s_Boy);
    }

    private void Update()
    {
        OpenBuildingMenuPanel();
    }


    private void OpenBuildingMenuPanel()
    {
        if (Input.GetKeyDown(KeyCode.U) && !isMenuPanelCreated)
        {
            // �������OnEnterʱ�Ļص�����������isPanelCreatedΪtrue
            PanelBase.Callback onEnterCallback = () =>
            {
                isMenuPanelCreated = true;
            };

            // �������OnExitʱ�Ļص�����������isPanelCreatedΪfalse
            PanelBase.Callback onExitCallback = () =>
            {
                isMenuPanelCreated = false;
            };

            PanelManager.TogglePanel(new BuildingMenuPanel(onEnterCallback, onExitCallback), SceneType.Main);
        }
    }

#if UNITY_EDITOR

    public void ����һ������()
    {

    }

    public void �˻ؿ�ʼ����()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

#endif
}
