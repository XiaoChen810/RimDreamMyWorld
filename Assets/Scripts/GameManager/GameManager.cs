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
        测试();
    }

    #region Pwan

    /// <summary>
    /// 生成Pawn
    /// </summary>
    /// <param name="kindDef"></param>
    /// <param name="position"></param>
    /// <param name="attribute"></param>
    /// <param name="prefab"></param>
    public Pawn GeneratePawn(PawnKindDef kindDef, Vector3 position, PawnAttribute attribute = null, GameObject prefab = null)
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
        }
        if (prefab == null)
        {
            prefab = CharacterTest;
        }
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null");
            return null;
        }
        GameObject newPawn = Instantiate(prefab, position, Quaternion.identity);
        newPawn.transform.SetParent(transform, false);
        if (newPawn.TryGetComponent<Pawn>(out Pawn pawn))
        {
            newPawn.name = kindDef.PawnName;
            InitPawn(pawn, kindDef, attribute);
            _pawnsList.Add(newPawn);
            return pawn;
        }
        else
        {
            Debug.LogError("Pawn prefab lost or don't have component in need ");
            return null;
        }
    }

    public void LoadScenePawnFromSave(Data_GameSave data_GameSave)
    {
        foreach(var pawnSave in data_GameSave.SavePawns)
        {
            Pawn newPawn = GeneratePawn(pawnSave.PawnKindDef, pawnSave.Position, pawnSave.PawnAttribute);
            newPawn.PawnInfo = pawnSave.PawnInfo;
        }
    }


    private static void InitPawn(Pawn pawn, PawnKindDef def, PawnAttribute attribute)
    {
        pawn.PawnName = def.PawnName;
        pawn.FactionName = def.PawnFaction;
        pawn.CanSelect = def.CanSelect;
        pawn.CanGetJob = def.CanGetJob;
        pawn.CanBattle = def.CanBattle;
        pawn.CanDrafted = def.CanDrafted;
        pawn.Attribute = (attribute == null) ? pawn.Attribute.InitPawnAttribute() : attribute;
    }

    private void GeneratePawn(Vector3 position, GameObject prefab, string pawnName, string factionName,
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

            PanelManager.TogglePanel(new BuildingMenuPanel(onEnterCallback, onExitCallback), SceneType.Main);
        }
    }



#if UNITY_EDITOR

    private static int nameIndex = 0;
    private bool isOnSelectPawnCreatePos_WhenTest;
    private bool isOnSelectEnenyPawnCreatePos_WhenTest;

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

    public void 测试()
    {
        if (isOnSelectPawnCreatePos_WhenTest && Input.GetMouseButtonDown(0))
        {
            isOnSelectPawnCreatePos_WhenTest = false;
            // new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            Vector3 createPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            createPos.z = 0;
            GeneratePawn(createPos, CharacterTest, "小光头" + nameIndex++, "殖民地");
        }
        if (isOnSelectEnenyPawnCreatePos_WhenTest && Input.GetMouseButtonDown(0))
        {
            isOnSelectEnenyPawnCreatePos_WhenTest = false;
            // new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            Vector3 createPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            createPos.z = 0;
            GeneratePawn(createPos, GoblinPrefab, "哥布林", "哥布林", canGetJob: false, canSelect: false);
        }
    }

#endif
}
