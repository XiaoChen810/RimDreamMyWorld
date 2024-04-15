using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_UISystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;
using System;

public class GameManager : SingletonMono<GameManager>
{
    public PanelManager PanelManager { get; private set; }

    private bool isMenuPanelCreated;

    [SerializeField]
    private List<GameObject> _pawnsList = new List<GameObject>();

    /// <summary>
    /// 游戏内全部的Pawn的GameObject列表
    /// </summary>
    public IReadOnlyList<GameObject> PawnsList   
    {
        get { return _pawnsList.AsReadOnly(); }
    }

    public GameObject CharacterTest;
    public GameObject GoblinPrefab;

    public bool isOnTestMode;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        PanelManager = new PanelManager();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        OpenBuildingMenuPanel();
        if (isOnSelectPawnCreatePos_WhenTest && Input.GetMouseButtonDown(0))
        {
            isOnSelectPawnCreatePos_WhenTest = false;
            // new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            Vector3 createPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            createPos.z = 0;
            GeneratePawn(createPos, CharacterTest, "小光头" + nameIndex++, "殖民地");
        }
        if(isOnSelectEnenyPawnCreatePos_WhenTest && Input.GetMouseButtonDown(0))
        {
            isOnSelectEnenyPawnCreatePos_WhenTest= false;
            // new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            Vector3 createPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            createPos.z = 0;
            GeneratePawn(createPos, GoblinPrefab, "哥布林", "哥布林", canGetJob: false, canSelect: false);
        }
    }

    #region Pwan

    public void GeneratePawn(PawnKindDef kindDef, Vector3 position, PawnAttribute attribute, GameObject prefab = null)
    {
        if (prefab == null)
        {
            if(kindDef.PrefabPath == null)
            {
                prefab = CharacterTest;
            }
            else
            {
                prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
            }
        }
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null");
        }
        GameObject newPawn = Instantiate(prefab, position, Quaternion.identity);
        if (newPawn.TryGetComponent<Pawn>(out Pawn pawn))
        {
            newPawn.name = kindDef.PawnName;
            InitPawn(pawn, kindDef, attribute);
            _pawnsList.Add(newPawn);
        }
        else
        {
            Debug.LogError("Pawn prefab lost or don't have component in need ");
        }
    }

    public static void InitPawn(Pawn pawn, PawnKindDef def, PawnAttribute attribute)
    {
        pawn.PawnName = def.PawnName;
        pawn.FactionName = def.PawnFaction;
        pawn.CanSelect = def.CanSelect;
        pawn.CanGetJob = def.CanGetJob;
        pawn.CanBattle = def.CanBattle;
        pawn.CanDrafted = def.CanDrafted;
        if (attribute == null)
        {
            pawn.Attribute.InitPawnAttribute();
        }
        else
        {
            pawn.Attribute = attribute;
        }
    }

    public void GeneratePawn(Vector3 position, GameObject prefab, string pawnName, string factionName,
        bool canSelect = true, bool canGetJob = true, bool canBattle = true, bool canDrafted = true)
    {

        GameObject newPawn = Instantiate(prefab, position, Quaternion.identity);
        if (newPawn.TryGetComponent<Pawn>(out Pawn pawn))
        {
            newPawn.name = pawnName;
            pawn.PawnName = pawnName;
            pawn.FactionName = factionName;
            pawn.CanSelect = canSelect;
            pawn.CanGetJob = canGetJob;
            pawn.CanBattle = canBattle;
            pawn.CanDrafted = canDrafted;
            _pawnsList.Add(newPawn);
        }
        else
        {
            Debug.LogError("Pawn prefab lost or don't have component in need ");
        }
    }

    #endregion

    private void OpenBuildingMenuPanel()
    {
        if (Input.GetKeyDown(KeyCode.U) && !isMenuPanelCreated)
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

            PanelManager.AddPanel(new BuildingMenuPanel(onEnterCallback, onExitCallback));
        }
    }

    private static int nameIndex = 0;
    private bool isOnSelectPawnCreatePos_WhenTest;
    private bool isOnSelectEnenyPawnCreatePos_WhenTest;

#if UNITY_EDITOR

    public void 生成一个基础小人()
    {
        if(!isOnSelectPawnCreatePos_WhenTest)
        {
            isOnSelectPawnCreatePos_WhenTest = true;
        }
    }

    public void 生成一个敌人()
    {
        if(!isOnSelectEnenyPawnCreatePos_WhenTest)
        {
            isOnSelectEnenyPawnCreatePos_WhenTest= true;
        }
    }

    public void 退回开始场景()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

    public void 生成测试棋子()
    {
        if (!isOnTestMode) return;
        GeneratePawn(new Vector3(30, 30), CharacterTest, "小光头" + nameIndex++, "殖民地");
        GeneratePawn(new Vector3(45, 30), GoblinPrefab, "哥布林", "哥布林");
    }

#endif
}
