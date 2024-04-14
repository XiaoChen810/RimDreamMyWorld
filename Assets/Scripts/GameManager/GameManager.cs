using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_UISystem;
using ChenChen_MapGenerator;
using ChenChen_Scene;

public class GameManager : SingletonMono<GameManager>
{
    public PanelManager PanelManager { get; private set; }

    private bool isMenuPanelCreated;

    [SerializeField]
    private List<GameObject> _pawnsList = new List<GameObject>();

    /// <summary>
    /// ��Ϸ��ȫ����Pawn��GameObject�б�
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
            GeneratePawn(createPos, CharacterTest, "С��ͷ" + nameIndex++, "ֳ���");
        }
        if(isOnSelectEnenyPawnCreatePos_WhenTest && Input.GetMouseButtonDown(0))
        {
            isOnSelectEnenyPawnCreatePos_WhenTest= false;
            // new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
            Vector3 createPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            createPos.z = 0;
            GeneratePawn(createPos, GoblinPrefab, "�粼��", "�粼��", canGetJob: false, canSelect: false);
        }
    }

    #region Pwan

    public bool CanSelect = true;
    public bool CanGetJob = true;
    public bool CanBattle = true;
    public bool CanDrafted = true;
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

            PanelManager.AddPanel(new BuildingMenuPanel(onEnterCallback, onExitCallback));
        }
    }

    private static int nameIndex = 0;
    private bool isOnSelectPawnCreatePos_WhenTest;
    private bool isOnSelectEnenyPawnCreatePos_WhenTest;

#if UNITY_EDITOR

    public void ����һ������С��()
    {
        if(!isOnSelectPawnCreatePos_WhenTest)
        {
            isOnSelectPawnCreatePos_WhenTest = true;
        }
    }

    public void ����һ������()
    {
        if(!isOnSelectEnenyPawnCreatePos_WhenTest)
        {
            isOnSelectEnenyPawnCreatePos_WhenTest= true;
        }
    }

    public void �˻ؿ�ʼ����()
    {
        MapManager.Instance.CloseSceneMap(MapManager.Instance.CurrentMapName);
        SceneSystem.Instance.SetScene(new StartScene());
    }

    public void ���ɲ�������()
    {
        if (!isOnTestMode) return;
        GeneratePawn(new Vector3(30, 30), CharacterTest, "С��ͷ" + nameIndex++, "ֳ���");
        GeneratePawn(new Vector3(45, 30), GoblinPrefab, "�粼��", "�粼��");
    }

#endif
}
