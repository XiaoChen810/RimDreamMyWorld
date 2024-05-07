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


    [SerializeField] private List<GameObject> _pawnsList = new List<GameObject>();
    /// <summary>
    /// 游戏内全部的Pawn的GameObject列表
    /// </summary>
    public IReadOnlyList<GameObject> PawnsList   
    {
        get { return _pawnsList.AsReadOnly(); }
    }

    [SerializeField] private List<PawnKindDef> _totalPawnDefList = new List<PawnKindDef>();
    /// <summary>
    /// 全部Pawn定义的列表，游戏开始时加载
    /// </summary>
    public IReadOnlyList<PawnKindDef> TotalPawnDefList
    {
        get { return _totalPawnDefList.AsReadOnly(); }
    }

    [SerializeField] private List<Pawn> _pawnWhenStartList = new List<Pawn>();
    /// <summary>
    /// 仅当进行人物选择时使用的角色列表
    /// </summary>
    public IReadOnlyList<Pawn> PawnWhenStartList
    {
        get { return _pawnWhenStartList.AsReadOnly(); }
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
    public Pawn GeneratePawn(PawnKindDef kindDef, Vector3 position, PawnAttribute attribute = null)
    {
        GameObject prefab = null;
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
        }
        if (prefab == null)
        {
            PawnKindDef ramdomPawnKindDef = TotalPawnDefList[UnityEngine.Random.Range(0, TotalPawnDefList.Count)];
            kindDef.PrefabPath = ramdomPawnKindDef.PrefabPath;
            kindDef.PawnDescription = ramdomPawnKindDef.PawnDescription;
            prefab = Resources.Load<GameObject>(kindDef.PrefabPath);
            Debug.Log("从现有Prefab中随机挑选了一个");
        }
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null");
            return null;
        }
        GameObject newPawnObject = Instantiate(prefab, position, Quaternion.identity);
        newPawnObject.transform.SetParent(transform, false);
        if (newPawnObject.TryGetComponent<Pawn>(out Pawn pawn))
        {
            newPawnObject.name = kindDef.PawnName;
            InitPawn(pawn, kindDef, attribute);
            _pawnsList.Add(newPawnObject);
            return pawn;
        }
        else
        {
            Debug.LogError("Pawn prefab lost or don't have component in need ");
            return null;
        }

        void InitPawn(Pawn pawn, PawnKindDef def, PawnAttribute attribute)
        {
            pawn.PawnName = def.PawnName;
            pawn.FactionName = def.PawnFaction;
            pawn.Description = def.PawnDescription;
            pawn.PrefabPath = def.PrefabPath;
            pawn.CanSelect = def.CanSelect;
            pawn.CanGetJob = def.CanGetJob;
            pawn.CanBattle = def.CanBattle;
            pawn.CanDrafted = def.CanDrafted;
            pawn.Attribute = (attribute == null) ? pawn.Attribute.InitPawnAttribute() : attribute;
        }
    }

    public PawnKindDef GetRandomPawnKindDef()
    {
        return TotalPawnDefList[UnityEngine.Random.Range(0, TotalPawnDefList.Count)];
    }

    public void LoadScenePawnFromSave(Data_GameSave data_GameSave)
    {
        foreach(var pawnSave in data_GameSave.SavePawns)
        {
            Pawn newPawn = GeneratePawn(pawnSave.PawnKindDef, pawnSave.Position, pawnSave.PawnAttribute);
            newPawn.PawnInfo = pawnSave.PawnInfo;
        }
    }

    public bool RemovePawn(Pawn pawn)
    {
        for(int i = 0; i < _pawnsList.Count; i++)
        {
            if (_pawnsList[i] == pawn.gameObject)
            {
                Destroy(_pawnsList[i].gameObject);
                _pawnsList.RemoveAt(i);
                return true;
            }
        }
        return false;
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

    #region 人物选择界面

    public void StartSelect()
    {
        _pawnWhenStartList.Add(GeneratePawn(GetRandomPawnKindDef(), new Vector3(-5, 1.3f, 0)));
        _pawnWhenStartList.Add(GeneratePawn(GetRandomPawnKindDef(), new Vector3(0, 1.3f, 0)));
        _pawnWhenStartList.Add(GeneratePawn(GetRandomPawnKindDef(), new Vector3(5, 1.3f, 0)));
        foreach(var pawn in _pawnWhenStartList)
        {
            pawn.StopUpdate = true;
        }
    }

    public void EndSelect()
    {
        for (int i = 0; i < _pawnWhenStartList.Count; i++)
        {
            RemovePawn(_pawnWhenStartList[i]);
        }
        _pawnWhenStartList.Clear();
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
            GeneratePawn(createPos, Resources.Load<GameObject>(TotalPawnDefList[UnityEngine.Random.Range(0, TotalPawnDefList.Count)].PrefabPath), "小光头" + nameIndex++, "殖民地");
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
