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
    public WorkSpaceTool WorkSpaceTool { get; private set; }

    private bool isMenuPanelCreated;

    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// 游戏内全部的Pawn的GameObject列表
    /// </summary>
    public List<GameObject> PawnsList   
    {
        get { return _pawnsList; }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// 仅当进行人物选择时使用的角色列表
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
        WorkSpaceTool = GetComponent<WorkSpaceTool>();
    }

    private void Update()
    {
        OpenBuildingMenuPanel();
    }


    private void OpenBuildingMenuPanel()
    {
        // 定义面板OnEnter时的回调函数，设置isPanelCreated为true
        PanelBase.Callback onEnterCallback = () =>
        {
            isMenuPanelCreated = true;
        };

        // 定义面板OnExit时的回调函数，重置isPanelCreated为false
        PanelBase.Callback onExitCallback = () =>
        {
            isMenuPanelCreated = false;
        };
        if(Input.GetKeyDown(KeyCode.U))
        {
            if (!isMenuPanelCreated)
            {
                PanelManager.TogglePanel(new BuildingMenuPanel(onEnterCallback, onExitCallback), SceneType.Main);
                return;
            }
            if (isMenuPanelCreated)
            {
                if (PanelManager.GetTopPanel().GetType() == typeof(BuildingMenuPanel))
                {
                    PanelManager.TogglePanel(new BuildingMenuPanel(onEnterCallback, onExitCallback), SceneType.Main);
                    return;
                }
            }
        }
    }

#if UNITY_EDITOR

    public void 生成一个敌人()
    {

    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

#endif
}
